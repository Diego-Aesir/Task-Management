using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagementAPI.DTO;
using TaskManagementAPI.Interfaces;

namespace TaskManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController(ITaskService TaskService) : ControllerBase
    {
        private readonly ITaskService taskService = TaskService;

        [HttpGet("{user_id}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Models.Task>>> GetAllTasksAsync(int user_id)
        {
            IEnumerable<Models.Task> tasks = await taskService.GetAllTasksAsync(user_id);
            if (tasks == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(tasks);
            }
        }

        [HttpGet("{user_id}/{id}")]
        [Authorize]
        public async Task<ActionResult<Models.Task>> GetSingleTaskAsync(int user_id, int id)
        {
            var task = await taskService.GetSingleTaskAsync(user_id, id);
            if (task != null)
            {
                return Ok(task);
            }
            else
            {
                return BadRequest("Task not found");
            }
        }

        [HttpPost("{user_id}")]
        [Authorize]
        public async Task<ActionResult<Models.Task>> PostTaskAsync(int user_id, [FromBody] CreateTaskRequest request)
        {
            if (request.Title == null || request.Status == null)
            {
                return BadRequest("Needed information: Task Title and Task Status");
            }

            Models.Task task = new()
            {
                Title = request.Title,
                Description = request.Description ?? "",
                Status = request.Status,
                DueDate = request.DueDate,
                Priority = request.Priority,
                IsCompleted = false,
                User_Id = user_id
            };

            Models.Task createdTask = await taskService.CreateTaskAsync(task);

            if (createdTask != null)
            {
                return Ok(task);
            }
            else
            {
                return BadRequest("Couldn't create a new Task");
            }
        }

        [HttpPut("{user_id}/{id}")]
        [Authorize]
        public async Task<ActionResult<Models.Task>> PutTask(int user_id, int id, [FromBody] CompletedTask request)
        {
            if (request.Title == null || request.Status == null)
            {
                return BadRequest("Needed information: Task Title and Task Status");
            }
            else if (request.Id != id || request.User_Id != user_id)
            {
                return BadRequest("User Id or Task Id isn't the same from the request");
            }

            Models.Task task = new()
            {
                Id = request.Id,
                Title = request.Title,
                Description = request.Description,
                Status = request.Status,
                DueDate = request.DueDate,
                Priority = request.Priority,
                IsCompleted = request.IsCompleted,
                User_Id = request.User_Id
            };

            try
            {
                task = await taskService.UpdateTaskAsync(task);
                return Ok(task);
            }
            catch (Exception ex)
            {
                throw new Exception($"Wasn't possible to update this Task: {request.Title}", ex);
            }
        }

        [HttpDelete("{user_id}/{id}")]
        [Authorize]
        public async Task<ActionResult<bool>> DeleteTask(int user_id, int id)
        {
            try
            {
                return Ok(await taskService.DeleteTaskAsync(id));
            }
            catch (Exception)
            {
                return BadRequest($"Wasn't possible to delete this task: {id}");
            }
        }
    }
}