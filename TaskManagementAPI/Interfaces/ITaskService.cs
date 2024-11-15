namespace TaskManagementAPI.Interfaces
{
    public interface ITaskService
    {
        Task<IEnumerable<Models.Task>> GetAllTasksAsync(int userId);
        Task<Models.Task> GetSingleTaskAsync(int userId, int taskId);
        Task<Models.Task> CreateTaskAsync(Models.Task userTask);
        Task<Models.Task> UpdateTaskAsync(Models.Task updatedTask);
        Task<bool> DeleteTaskAsync(int deletedTaskId);
    }
}
