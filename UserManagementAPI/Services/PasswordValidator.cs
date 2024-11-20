using Microsoft.AspNetCore.Identity;

namespace UserManagementAPI.Services
{
    public class PasswordValidator : IPasswordValidator<User>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<User> manager, User user, string? password)
        {
            if (password.Contains(user.UserName, StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(IdentityResult.Failed(new IdentityError
                {
                    Description = "Password cannot contain the username."
                }));
            }

            return Task.FromResult(IdentityResult.Success);
        }
    }
}
