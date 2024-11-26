namespace UserManagementAPI.DTO.RabbitMQ
{
    public class AllTasksReceive
    {
        public string userId {  get; set; }
        public List<TaskReceive> tasks { get; set; }
    }
}
