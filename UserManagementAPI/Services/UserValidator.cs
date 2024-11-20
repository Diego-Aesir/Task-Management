using Microsoft.AspNetCore.Identity;

namespace UserManagementAPI.Services
{
    public class UserValidator : IUserValidator<User>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<User> manager, User user)
        {
            var forbiddenUserNames = new List<string> { "admin", "root", "user" };
            if (forbiddenUserNames.Contains(user.UserName.ToLower()))
            {
                return Task.FromResult(IdentityResult.Failed(new IdentityError
                {
                    Description = "This username is not allowed."
                }));
            }

            return Task.FromResult(IdentityResult.Success);
        }
    }
}
