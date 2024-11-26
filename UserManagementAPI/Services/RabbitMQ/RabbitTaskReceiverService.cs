using UserManagementAPI.Interface;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using UserManagementAPI.DTO.RabbitMQ;

namespace UserManagementAPI.Services.RabbitMQ
{
    public class RabbitTaskReceiverService : IRabbitTaskReceiver
    {
        IConnection _connection;
        IChannel _channel;

        public async Task InitializeReceiverServiceAsync()
        {
            if (_connection != null && _connection.IsOpen && _channel != null && _channel.IsOpen)
            {
                return;
            }

            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "user_api",
                Password = "api123",
            };

            try
            {
                _connection = await factory.CreateConnectionAsync();
                _channel = await _connection.CreateChannelAsync();

                await _channel.QueueDeclareAsync("receive_all_task", durable: true, exclusive: false, autoDelete: false);
                await _channel.QueueDeclareAsync("receive_task_creation", durable: true, exclusive: false, autoDelete: false);
                await _channel.QueueDeclareAsync("receive_task_update", durable: true, exclusive: false, autoDelete: false);
                await _channel.QueueDeclareAsync("receive_task_delete", durable: true, exclusive: false, autoDelete: false);
                await _channel.QueueDeclareAsync("receive_next_task_due", durable: true, exclusive: false, autoDelete: false);
            }
            catch (Exception)
            {
                throw new Exception("Couldn't stablish Rabbit Connection");
            }
        }

        public async Task<AllTasksReceive> RetrieveAllTasks(string userId)
        {
            var tcs = new TaskCompletionSource<AllTasksReceive>();

            if (_channel == null || !_channel.IsOpen)
            {
                await InitializeReceiverServiceAsync();
            }
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (model, eventArgs) =>
            {
                var routingKey = eventArgs.RoutingKey;
                if(routingKey != "receive_all_task")
                {
                    return;
                }

                var message = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
                var tasks = JsonSerializer.Deserialize<AllTasksReceive>(message);
                
                if (tasks.userId != userId)
                {
                    return;
                }

                await _channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
                tcs.SetResult(tasks);
            };

            await _channel.BasicConsumeAsync("  ", autoAck: false, consumer: consumer);
            return await tcs.Task;
        }

        public async Task<TaskReceive> RetrieveCreatedTask(string userId)
        {
            var tcs = new TaskCompletionSource<TaskReceive>();

            if (_channel == null || !_channel.IsOpen)
            {
                await InitializeReceiverServiceAsync();
            }
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (model, eventArgs) =>
            {
                var routing_key = eventArgs.RoutingKey;
                if(routing_key != "receive_task_creation")
                {
                    return;
                }

                var message = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
                var task = JsonSerializer.Deserialize<TaskReceive>(message);
                if (task.User_Id != userId)
                {
                    return;
                }

                await _channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
                tcs.SetResult(task);
            };

            await _channel.BasicConsumeAsync("receive_task_creation", autoAck: false, consumer);
            return await tcs.Task;
        }

        public async Task<TaskReceive> RetrieveUpdatedTask(string userId, int taskId)
        {
            var tcs = new TaskCompletionSource<TaskReceive>();

            if (_channel == null || !_channel.IsOpen)
            {
                await InitializeReceiverServiceAsync();
            }
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (model, eventArgs) =>
            {
                var routing_key = eventArgs.RoutingKey;
                if( routing_key != "receive_task_update")
                {
                    return;
                };

                var message = eventArgs.Body.ToArray();
                var task = JsonSerializer.Deserialize<TaskReceive>(message);
                if(task.User_Id != userId || task.Id != taskId)
                {
                    return;
                };

                await _channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
                tcs.SetResult(task);
            };
            await _channel.BasicConsumeAsync("receive_task_update", autoAck: false, consumer);
            return await tcs.Task;
        }

        public async Task<TaskReceive> RetrieveDeleteTaskResult(string userId)
        {
            var tcs = new TaskCompletionSource<TaskReceive>();

            if (_channel == null || !_channel.IsOpen)
            {
                await InitializeReceiverServiceAsync();
            }
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (model, eventArgs) =>
            {
                var routing_key = eventArgs.RoutingKey;
                if (routing_key != "receive_task_delete")
                {
                    return;
                };

                var message = eventArgs.Body.ToArray();
                var task = JsonSerializer.Deserialize<TaskReceive>(message);
                if (task.User_Id != userId)
                {
                    return;
                };

                await _channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
                tcs.SetResult(task);
            };
            await _channel.BasicConsumeAsync("receive_task_delete", autoAck: false, consumer);
            return await tcs.Task;
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
    }
}