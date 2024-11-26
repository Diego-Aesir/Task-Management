namespace UserManagementAPI.Interface
{
    public interface IRabbitTaskSender
    {
        Task InitializeSenderServiceAsync();
        Task RequestGetTasks(string message);
        Task RequestCreateTask(string message);
        Task RequestUpdateTask(string message);
        Task RequestDeleteTask(string message);
        Task RequestDueDate(string message);
        Task CloseConnection();
    }
}
