using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using UserManagementAPI.Interface;
using UserManagementAPI.DTO.RabbitMQ;
using Microsoft.AspNetCore.Authorization;

namespace UserManagementAPI.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        IRabbitTaskSender _rabbitTaskSender;
        IRabbitTaskReceiver _rabbitTaskReceiver;

        public TaskController(IRabbitTaskSender rabbitTaskSender, IRabbitTaskReceiver rabbitTaskReceiver)
        {
            _rabbitTaskSender = rabbitTaskSender;
            _rabbitTaskReceiver = rabbitTaskReceiver;
        }

        [HttpPost("{userId}/GetAllTasks")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> RequestAllTasks(string userId)
        {
            try
            {
                await _rabbitTaskSender.RequestGetTasks(userId);
                return Accepted("Task Request sent successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{userId}/GetAllTasksResponse")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AllTasksReceive))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> ReceiveAllTasks(string userId)
        {
            AllTasksReceive allTasks = await _rabbitTaskReceiver.RetrieveAllTasks(userId);
            if (allTasks == null)
            {
                return BadRequest("Rabbit Message couldn't be retrieved");
            }
            return Ok(allTasks);
        }

        [HttpPost("{userId}/CreateTask")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> RequestCreateTask(string userId, [FromBody] TaskCreationSend task)
        {
            try
            {
                task.UserId = userId;
                await _rabbitTaskSender.RequestCreateTask(JsonSerializer.Serialize(task));
                return Accepted("Task Creation Request sent successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{userId}/CreateTaskResponse")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TaskReceive))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> ReceiveCreateTask(string userId)
        {
            TaskReceive task = await _rabbitTaskReceiver.RetrieveCreatedTask(userId);
            if (task == null)
            {
                return BadRequest("Couldn't Retrieve Created Task");
            }
            return Ok(task);
        }

        [HttpPut("{userId}/RequestUpdate/{taskId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> RequestUpdateTask(string userId, int taskId, [FromBody] TaskUpdateSend taskUpdate)
        {
            try
            {
                taskUpdate.UserId = userId;
                taskUpdate.TaskId = taskId;
                await _rabbitTaskSender.RequestUpdateTask(JsonSerializer.Serialize(taskUpdate));
                return Accepted("Task Update Request sent successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{userId}/RequestUpdateResponse/{taskId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TaskReceive))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> ReceiveUpdateTask(string userId, int taskId)
        {
            TaskReceive task = await _rabbitTaskReceiver.RetrieveUpdatedTask(userId, taskId);
            if (task == null)
            {
                return BadRequest("Couldn't Retrieve Updated Task");
            }
            return Ok(task);
        }

        [HttpDelete("{userId}/RequestDeleteTask/{taskId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> RequestDeleteTask(string userId, int taskId)
        {
            try
            {
                await _rabbitTaskSender.RequestDeleteTask(taskId.ToString());
                return Accepted("Task Delete Request sent successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{userId}/RequestDeleteTaskResponse/{taskId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> ReceiveDeleteTask(string userId, int taskId)
        {
            TaskReceive task = await _rabbitTaskReceiver.RetrieveDeleteTaskResult(userId);
            if (task == null)
            {
                return BadRequest("Couldn't Retrieve Deleted Task");
            }

            return Ok(task);
        }
    }
}
