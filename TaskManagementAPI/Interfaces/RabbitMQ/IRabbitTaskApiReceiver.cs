namespace TaskManagementAPI.Interfaces.RabbitMQ
{
    public interface IRabbitTaskApiReceiver
    {
        public Task InitializeReceiverService();

        public Task ReceiveGetAllTaskMessageAndAnswer();

        public Task ReceiveCreateTaskMessageAndAnswer();

        public Task ReceiveUpdateTaskMessageAndAnswer();

        public Task ReceiveDeleteTaskMessageAndAnswer();
    }
}
