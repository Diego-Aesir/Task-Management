using System.ComponentModel.DataAnnotations;

namespace UserManagementAPI.DTO.RabbitMQ
{
    public class TaskCreationSend
    {
        public string User_Id { get; set; }
        
        [Required]
        public string Title { get; set; }

        public string? Description { get; set; }

        [Required]
        public DateTime CreationDate = DateTime.UtcNow;

        [Required]
        public string Status { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        [Required]
        public int Priority { get; set; }

        [Required]
        public bool IsCompleted { get; set; }
    }
}
