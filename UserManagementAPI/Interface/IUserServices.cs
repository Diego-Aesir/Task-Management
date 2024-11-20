using UserManagementAPI.DTO; 

namespace UserManagementAPI.Interface
{
    public interface IUserServices
    {
        public Task<User> GetUserASync(string userId);
        Task<UserResponse> RegisterUserAsync(User user, string password);
        Task<UserResponse> AuthenticateUserAsync(string userName, string password);
        public Task<User> UpdateUserAsync(User user, string? password);
        public Task<bool> DeleteUserAsync(User user);
    }
}