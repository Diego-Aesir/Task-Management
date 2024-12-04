using System.Text;
using RabbitMQ.Client;
using TaskManagementAPI.Interfaces.RabbitMQ;

namespace TaskManagementAPI.Services.RabbitMQ
{
    public class SendMessageService : IRabbitTaskApiSender, IAsyncDisposable
    {
        IConnection _connection;
        IChannel _channel;

        public async Task InitializeSenderService()
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

                await _channel.QueueDeclareAsync("receive_all_task", durable: true, exclusive: false, autoDelete: false);
                await _channel.QueueDeclareAsync("receive_task_creation", durable: true, exclusive: false, autoDelete: false);
                await _channel.QueueDeclareAsync("receive_task_update", durable: true, exclusive: false, autoDelete: false);
                await _channel.QueueDeclareAsync("receive_task_delete", durable: true, exclusive: false, autoDelete: false);
            }
            catch (Exception)
            {
                throw new Exception("Couldn't stablish Rabbit Connection");
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

        public async Task SendCreateTaskMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);
            await _channel.BasicPublishAsync(exchange: "", routingKey: "receive_task_creation", body: body);
        }

        public async Task SendDeleteTaskMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);
            await _channel.BasicPublishAsync(exchange: "", routingKey: "receive_task_delete", body: body);
        }

        public async Task SendErrorMessage(string routingKey, string queueError)
        {
            string translatedRoutingKey;
            if (_channel == null || !_channel.IsOpen)
            {
                await InitializeSenderService();
            }

            switch (routingKey)
            {
                case "get_tasks":
                    translatedRoutingKey = "receive_all_task";
                    break;

                case "create_task":
                    translatedRoutingKey = "receive_task_creation";
                    break;

                case "update_task":
                    translatedRoutingKey = "receive_task_update";
                    break;

                case "delete_task":
                    translatedRoutingKey = "receive_task_delete";
                    break;

                default:
                    translatedRoutingKey = "ErrorRoute";
                    break;
            };

            var body = Encoding.UTF8.GetBytes(queueError);
            await _channel.BasicPublishAsync(exchange: "", routingKey: translatedRoutingKey, body: body);
        }

        public async Task SendGetAllTaskMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);
            await _channel.BasicPublishAsync(exchange: "", routingKey: "receive_all_task", body: body);
        }

        public async Task SendUpdateTaskMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);
            await _channel.BasicPublishAsync(exchange: "", routingKey: "receive_task_update", body: body);
        }

        public async ValueTask DisposeAsync()
        {
            await CloseConnection();
        }
    }
}
