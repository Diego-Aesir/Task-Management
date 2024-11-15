using System.ComponentModel.DataAnnotations;

namespace TaskManagementAPI.DTO
{
    public class CreateTaskRequest
    {
        [Required]
        public string Title { get; set; }
        public string? Description { get; set; }
        [Required]
        public string Status { get; set; }
        [Required]
        public int Priority { get; set; }
        [Required]
        public DateTime DueDate { get; set; }
    }
}
