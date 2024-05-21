using ProjectTasks.Interfaces;
using RabbitMQ.Client;
using System.Text;
namespace ProjectTasks.Services
{
    public class RabbitMQMessagingService : IRabbitMQMessagingService
    {
        private ConnectionFactory factory;
        private IConnection connection;
        private IModel channel;
        public RabbitMQMessagingService()
        {
            factory = new ConnectionFactory();
            connection = factory.CreateConnection();
            channel = connection.CreateModel();

            channel.QueueDeclare(queue: "messagingqueue",
                     durable: false,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);
        }

        public void PublishMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(DateTime.Now.ToString("G") + ": " + message);
            channel.BasicPublish(exchange: string.Empty,
                                 routingKey: "messagingqueue",
                                 basicProperties: null,
                                 body: body);
        }
    }
}
