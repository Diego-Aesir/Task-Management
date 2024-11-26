using UserManagementAPI.DTO.RabbitMQ;

namespace UserManagementAPI.Interface
{
    public interface IRabbitTaskReceiver
    {
        Task InitializeReceiverServiceAsync();

        public Task<AllTasksReceive> RetrieveAllTasks(string userId);

        public Task<TaskReceive> RetrieveCreatedTask(string userId);

        public Task<TaskReceive> RetrieveUpdatedTask(string userId, int taskId);

        public Task<TaskReceive> RetrieveDeleteTaskResult(string userId);

        Task CloseConnection();
    }
}
