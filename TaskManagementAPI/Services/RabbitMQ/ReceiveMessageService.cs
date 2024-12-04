using TaskManagementAPI.Interfaces.RabbitMQ;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using TaskManagementAPI.Interfaces;
using TaskManagementAPI.DTO.RabbitMQ;
using System.Reflection;

namespace TaskManagementAPI.Services.RabbitMQ
{
    public class ReceiveMessageService : IRabbitTaskApiReceiver, IAsyncDisposable
    {
        private IConnection _connection;
        private IChannel _channel;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IRabbitTaskApiSender _taskApiSender;

        public ReceiveMessageService(IRabbitTaskApiSender apiSender, IServiceScopeFactory serviceScopeFactory)
        {
            _taskApiSender = apiSender;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task InitializeReceiverService()
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "task_api",
                Password = "api123",
            };

            try
            {
                _connection = await factory.CreateConnectionAsync();
                _channel = await _connection.CreateChannelAsync();

                await _channel.QueueDeclareAsync("get_tasks", durable: true, exclusive: false, autoDelete: false);
                await _channel.QueueDeclareAsync("create_task", durable: true, exclusive: false, autoDelete: false);
                await _channel.QueueDeclareAsync("update_task", durable: true, exclusive: false, autoDelete: false);
                await _channel.QueueDeclareAsync("delete_task", durable: true, exclusive: false, autoDelete: false);
            }
            catch (Exception)
            {
                throw new Exception("Couldn't establish RabbitMQ connection");
            }
        }

        public async Task CloseConnection()
        {
            if (_channel != null && _channel.IsOpen)
            {
                await _channel.CloseAsync();
            }

            if (_connection != null && _connection.IsOpen)
            {
                await _connection.CloseAsync();
            }
        }

        public async Task ReceiveCreateTaskMessageAndAnswer()
        {
            if (_channel == null || !_channel.IsOpen)
            {
                await InitializeReceiverService();
            }

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (model, eventArgs) =>
            {
                var routingKey = eventArgs.RoutingKey;
                if (routingKey != "create_task")
                {
                    return;
                }

                var _taskService = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<ITaskService>();
                var message = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
                var taskReceived = JsonSerializer.Deserialize<Models.Task>(message);
                try
                {
                    var created = await _taskService.CreateTaskAsync(taskReceived);
                    if (created == null)
                    {
                        var messageError = new MessageError
                        {
                            User_Id = taskReceived.User_Id,
                            Error = "Couldn't create this task"
                        };
                        var error = JsonSerializer.Serialize(messageError);
                        await _taskApiSender.SendErrorMessage("create_task", error);
                    }

                    await _taskApiSender.SendCreateTaskMessage(JsonSerializer.Serialize(created));
                }
                catch (Exception ex)
                {
                    MessageError error = new()
                    {
                        User_Id = taskReceived.User_Id,
                        Error = $"Error while Creating task: {ex.Message}"
                    };
                    await _taskApiSender.SendErrorMessage("create_task", JsonSerializer.Serialize(error));
                }
                finally
                {
                    await _channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
                }
            };

            await _channel.BasicConsumeAsync("create_task", autoAck: false, consumer: consumer);
        }

        public async Task ReceiveDeleteTaskMessageAndAnswer()
        {
            if (_channel == null || !_channel.IsOpen)
            {
                await InitializeReceiverService();
            }

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (model, eventArgs) =>
            {
                var routingKey = eventArgs.RoutingKey;
                if (routingKey != "delete_task")
                {
                    return;
                }

                var _taskService = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<ITaskService>();
                var message = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
                var task = JsonSerializer.Deserialize<Models.Task>(message);
                try
                {
                    bool result = await _taskService.DeleteTaskAsync(task.Id);
                    if (!result)
                    {
                        MessageError error = new()
                        {
                            User_Id = task.User_Id,
                            Error = "Wasn't possible to delete this task"
                        };
                        await _taskApiSender.SendErrorMessage("delete_task", JsonSerializer.Serialize(error));
                    }
                    await _taskApiSender.SendDeleteTaskMessage(JsonSerializer.Serialize(task));
                }
                catch (Exception ex)
                {
                    MessageError error = new()
                    {
                        User_Id = task.User_Id,
                        Error = $"Error while Deleting task: {ex.Message}"
                    };
                    await _taskApiSender.SendErrorMessage("delete_task", JsonSerializer.Serialize(error));
                }
                finally
                {
                    await _channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
                }
            };
            await _channel.BasicConsumeAsync("delete_task", autoAck: false, consumer: consumer);
        }

        public async Task ReceiveGetAllTaskMessageAndAnswer()
        {
            if (_channel == null || !_channel.IsOpen)
            {
                await InitializeReceiverService();
            }

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (model, eventArgs) =>
            {
                var routingKey = eventArgs.RoutingKey;
                if (routingKey != "get_tasks")
                {
                    return;
                }

                var _taskService = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<ITaskService>();
                var message = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
                try
                {
                    TaskList taskList = new TaskList { User_Id = message };
                    IEnumerable<Models.Task> tasks = await _taskService.GetAllTasksAsync(message);
                    taskList.Tasks = tasks.ToList();
                    await _taskApiSender.SendGetAllTaskMessage(JsonSerializer.Serialize(taskList));
                }
                catch (Exception ex)
                {
                    var error = new MessageError
                    {
                        User_Id = message,
                        Error = $"Error while retrieving tasks: {ex.Message}"
                    };
                    await _taskApiSender.SendErrorMessage("get_tasks", JsonSerializer.Serialize(error));
                }
                finally
                {
                    await _channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
                }
            };

            await _channel.BasicConsumeAsync("get_tasks", autoAck: false, consumer: consumer);
        }

        public async Task ReceiveUpdateTaskMessageAndAnswer()
        {
            if (_channel == null || !_channel.IsOpen)
            {
                await InitializeReceiverService();
            }

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (model, eventArgs) =>
            {
                var routingKey = eventArgs.RoutingKey;
                if (routingKey != "update_task")
                {
                    return;
                }

                var _taskService = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<ITaskService>();
                var message = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
                var taskReceived = JsonSerializer.Deserialize<Models.Task>(message);
                try
                {
                    Models.Task updatedTask = await _taskService.UpdateTaskAsync(taskReceived);
                    await _taskApiSender.SendUpdateTaskMessage(JsonSerializer.Serialize(updatedTask));
                }
                catch (Exception ex)
                {
                    var error = new MessageError
                    {
                        User_Id = message,
                        Error = $"Error while updating task: {ex.Message}"
                    };
                    await _taskApiSender.SendErrorMessage("update_task", JsonSerializer.Serialize(error));
                }
                finally
                {
                    await _channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
                }
            };
            await _channel.BasicConsumeAsync("update_task", autoAck: false, consumer: consumer);
        }

        public async ValueTask DisposeAsync()
        {
            await CloseConnection();
        }
    }
}
