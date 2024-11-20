using System.ComponentModel.DataAnnotations;

namespace UserManagementAPI.DTO
{
    public class DeleteUserRequest
    {
        [Required]
        public string Id { get; set; }
    }
}
