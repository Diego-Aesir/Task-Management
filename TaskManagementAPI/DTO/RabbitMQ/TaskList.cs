namespace TaskManagementAPI.DTO.RabbitMQ
{
    public class TaskList
    {
        public required string User_Id { get; set; }
        public List<Models.Task>? Tasks { get; set; }
    }
}
