namespace UserManagementAPI.DTO.RabbitMQ
{
    public class AllTasksReceive
    {
        public required string User_Id { get; set; }

        public List<TaskReceive>? Tasks { get; set; }

        public string? Error { get; set; }
    }
}
