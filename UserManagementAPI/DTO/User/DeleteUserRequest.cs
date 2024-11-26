using System.ComponentModel.DataAnnotations;

namespace UserManagementAPI.DTO.User
{
    public class DeleteUserRequest
    {
        [Required]
        public string Id { get; set; }
    }
}
