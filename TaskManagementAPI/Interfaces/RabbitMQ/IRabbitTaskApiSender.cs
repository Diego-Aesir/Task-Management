namespace TaskManagementAPI.Interfaces.RabbitMQ
{
    public interface IRabbitTaskApiSender
    {
        public Task InitializeSenderService();

        public Task SendGetAllTaskMessage(string message);

        public Task SendCreateTaskMessage(string message);

        public Task SendUpdateTaskMessage(string message);

        public Task SendDeleteTaskMessage(string message);

        public Task SendErrorMessage(string routingKey, string queueError);
    }
}
