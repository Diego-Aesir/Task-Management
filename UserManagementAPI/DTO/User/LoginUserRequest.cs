using System.ComponentModel.DataAnnotations;

namespace UserManagementAPI.DTO.User
{
    public class LoginUserRequest
    {
        [Required]
        public string username { get; set; }

        [Required]
        public string password { get; set; }
    }
}
