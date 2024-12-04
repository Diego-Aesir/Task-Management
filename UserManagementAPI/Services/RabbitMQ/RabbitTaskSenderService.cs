using UserManagementAPI.Interface;
using RabbitMQ.Client;
using System.Text;

namespace UserManagementAPI.Services.RabbitMQ
{
    public class RabbitTaskSenderService : IRabbitTaskSender
    {
        private IConnection _connection;
        private IChannel _channel;

        public async Task InitializeSenderServiceAsync()
        {
            if (_connection != null && _connection.IsOpen && _channel != null && _channel.IsOpen)
            {
                return;
            }

            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "user_api",
                Password = "api123"
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
            catch (Exception ex)
            {
                throw new Exception("Couldn't establish Rabbit Connection", ex);
            }
        }

        public async Task RequestGetTasks(string message)
        {
            if (_channel == null || !_channel.IsOpen)
            {
                await InitializeSenderServiceAsync();
            }
            var body = Encoding.UTF8.GetBytes(message);
            await _channel.BasicPublishAsync(exchange: "", routingKey: "get_tasks", body: body);
        }

        public async Task RequestCreateTask(string message)
        {
            if (_channel == null || !_channel.IsOpen)
            {
                await InitializeSenderServiceAsync();
            }

            var body = Encoding.UTF8.GetBytes(message);
            await _channel.BasicPublishAsync(exchange: "", routingKey: "create_task", body);
        }

        public async Task RequestUpdateTask(string message)
        {
            if (_channel == null || !_channel.IsOpen)
            {
                await InitializeSenderServiceAsync();
            }
            var body = Encoding.UTF8.GetBytes(message);
            await _channel.BasicPublishAsync(exchange: "", routingKey: "update_task", body);
        }

        public async Task RequestDeleteTask(string message)
        {
            if (_channel == null || !_channel.IsOpen)
            {
                await InitializeSenderServiceAsync();
            }
            var body = Encoding.UTF8.GetBytes(message);
            await _channel.BasicPublishAsync(exchange: "", routingKey: "delete_task", body);
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
