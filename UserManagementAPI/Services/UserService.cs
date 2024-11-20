using Microsoft.AspNetCore.Identity;
using UserManagementAPI.Interface;
using UserManagementAPI.DTO;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;

namespace UserManagementAPI.Services
{
    public class UserService : IUserServices
    {
        UserManager<User> _manager;
        SignInManager<User> _signInManager;
        IConfiguration _config;

        public UserService(UserManager<User> manager, SignInManager<User> signInManager, IConfiguration config)
        {
            _manager = manager;
            _signInManager = signInManager;
            _config = config;
        }
    
        public async Task<UserResponse> AuthenticateUserAsync(string userName, string password)
        {
            var user = await _manager.FindByNameAsync(userName);
            if (user == null)
            {
                throw new Exception("Invalid User");
            }

            var result = await _signInManager.PasswordSignInAsync(userName, password, false, false);
            if (!result.Succeeded)
            {
                throw new Exception("Invalid credentials");
            }

            return new UserResponse
            {
                Id = user.Id,
                UserName = user.UserName,
                JWT = GenerateJwt(user),
            };
        }

        public async Task<bool> DeleteUserAsync(User user)
        {
            var foundUser = await _manager.FindByIdAsync(user.Id);
            if (foundUser == null)
            {
                throw new Exception("User not found");
            }

            var result = await _manager.DeleteAsync(foundUser);
            return result.Succeeded;
        }

        public async Task<User> GetUserASync(string userId)
        {
            var user = await _manager.FindByIdAsync(userId);

            if(user == null)
            {
                throw new Exception("Invalid user Id");
            }

            return user;
        }

        public async Task<UserResponse> RegisterUserAsync(User user, string password)
        {
            var result = await _manager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                throw new Exception("Registration failed: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            return new UserResponse
            {
                UserName = user.UserName,
                Id = user.Id,
                JWT = GenerateJwt(user)
            };
        }

        public async Task<User> UpdateUserAsync(User user, string? newPassword)
        {
            var existingUser = await _manager.FindByIdAsync(user.Id);
            if (existingUser == null)
            {
                throw new Exception("User not found.");
            }

            existingUser.UserName = user.UserName;
            existingUser.Email = user.Email;
            existingUser.PhoneNumber = user.PhoneNumber;
            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;

            var updateResult = await _manager.UpdateAsync(existingUser);
            if (!updateResult.Succeeded)
            {
                throw new Exception("Failed to update user: " + string.Join(", ", updateResult.Errors.Select(e => e.Description)));
            }

            if (!string.IsNullOrEmpty(newPassword))
            {
                var resetToken = await _manager.GeneratePasswordResetTokenAsync(existingUser);

                var passwordResult = await _manager.ResetPasswordAsync(existingUser, resetToken, newPassword);
                if (!passwordResult.Succeeded)
                {
                    throw new Exception("Failed to update password: " + string.Join(", ", passwordResult.Errors.Select(e => e.Description)));
                }
            }

            return existingUser;
        }

        private string GenerateJwt(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(issuer: _config["Jwt:Issuer"], audience: _config["Jwt:Audience"], claims: claims, expires: DateTime.Now.AddHours(1), signingCredentials: credential);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
