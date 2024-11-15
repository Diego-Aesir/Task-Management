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
        public string Title { set; get; }
        public string? Description { set; get; }
        [Required]
        public DateTime CreationDate = DateTime.UtcNow;
        [Required]
        public string Status { set; get; }
        [Required]
        public DateTime DueDate { set; get; }
        [Required]
        public int Priority { set; get; }
        [Required]
        public bool IsCompleted { set; get; }
        [Required]
        public int User_Id { set; get; }
    }
}
