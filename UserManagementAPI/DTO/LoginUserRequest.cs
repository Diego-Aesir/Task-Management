using System.ComponentModel.DataAnnotations;

namespace UserManagementAPI.DTO
{
    public class LoginUserRequest
    {
        [Required]
        public string username { get; set; }
        
        [Required]
        public string password { get; set; }
    }
}
