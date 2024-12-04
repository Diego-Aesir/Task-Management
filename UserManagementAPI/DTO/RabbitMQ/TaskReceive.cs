using System.ComponentModel.DataAnnotations;

namespace UserManagementAPI.DTO.RabbitMQ
{
    public class TaskReceive
    {
        public int? Id { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        public DateTime? CreationDate { get; set; }

        public string? Status { get; set; }

        public DateTime? DueDate { get; set; }

        public int? Priority { get; set; }

        public bool? IsCompleted { get; set; }

        public required string User_Id { get; set; }

        public string? Error { get; set; }
    }
}
