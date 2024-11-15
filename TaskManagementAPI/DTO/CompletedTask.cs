namespace TaskManagementAPI.DTO
{
    public class CompletedTask
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; }
        public int Priority { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime CreationDate { get; set; }
        public bool IsCompleted { get; set; }
        public int User_Id { get; set; }
    }
}
