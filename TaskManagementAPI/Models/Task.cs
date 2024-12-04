using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagementAPI.Models
{
    public class Task
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string? Description { get; set; }
        [Required]
        public DateTime CreationDate { get; set; }
        [Required]
        public string Status { get; set; }
        [Required]
        public DateTime DueDate { get; set; }
        [Required]
        public int Priority { get; set; }
        [Required]
        public bool IsCompleted { get; set; }
        [Required]
        public string User_Id { get; set; }
    }
}
