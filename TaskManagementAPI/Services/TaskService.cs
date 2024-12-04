using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Data;
using TaskManagementAPI.Interfaces;

namespace TaskManagementAPI.Services
{
    public class TaskService : ITaskService
    {
        private readonly TaskDbContext context;

        public TaskService(TaskDbContext _context)
        {
            context = _context;
        }

        public async Task<Models.Task> CreateTaskAsync(Models.Task userTask)
        {
            await context.AddAsync(userTask);
            var rowsAffected = await context.SaveChangesAsync();
            if (rowsAffected > 0)
            {
                return userTask;
            }
            else
            {
                throw new Exception("Couldn't create a new Task");
            }
        }

        public async Task<bool> DeleteTaskAsync(int deletedTaskId)
        {
            var taskFound = await context.Tasks.FindAsync(deletedTaskId);
            if (taskFound == null)
            {
                return false;
            }

            try
            {
                context.Tasks.Remove(taskFound);
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while trying to delete task with ID {deletedTaskId}: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<Models.Task>> GetAllTasksAsync(string userId)
        {
            try
            {
                var tasks = await context.Tasks.Where(task => task.User_Id == userId).ToListAsync();
                return tasks ?? new List<Models.Task>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Wasn't possible to find the tasks for the user with id: {userId}", ex);
            }
        }

        public async Task<Models.Task> GetSingleTaskAsync(string userId, int taskId)
        {
            try
            {
                var userTask = await context.Tasks.Where(task => task.Id == taskId && task.User_Id == userId).FirstOrDefaultAsync();
                return userTask == null ? throw new KeyNotFoundException($"No task found with taskId: {taskId} for userId: {userId}") : userTask;
            }
            catch (Exception ex)
            {
                throw new Exception($"Wasn't possible to find this task: {taskId} with this user id: {userId}", ex);
            }
        }

        public async Task<Models.Task> UpdateTaskAsync(Models.Task updatedTask)
        {
            try
            {
                var taskToUpdate = await GetSingleTaskAsync(updatedTask.User_Id, updatedTask.Id);
                if (taskToUpdate != null)
                {
                    taskToUpdate.Title = updatedTask.Title;
                    taskToUpdate.Description = updatedTask.Description == null ? "" : updatedTask.Description;
                    taskToUpdate.Status = updatedTask.Status;
                    taskToUpdate.DueDate = updatedTask.DueDate;
                    taskToUpdate.Priority = updatedTask.Priority;
                    taskToUpdate.IsCompleted = updatedTask.IsCompleted;

                    await context.SaveChangesAsync();
                    return taskToUpdate;
                }
                else
                {
                    throw new Exception("Task not found");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Wasn't possible to update the values", ex);
            }
        }
    }
}
