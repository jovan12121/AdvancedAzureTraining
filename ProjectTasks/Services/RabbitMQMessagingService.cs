using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
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
        public RabbitMQMessagingService(IConfiguration configuration)
        {
            var keyVaultEndpoint = configuration.GetSection("KeyVault:BaseUrl").Value;
            var clientId = configuration.GetSection("AzureAd:ClientId").Value;
            var clientSecret = configuration.GetSection("AzureAd:ClientSecret").Value;
            var tenantId = configuration.GetSection("AzureAd:TenantId").Value;
            var secretClient = new SecretClient(new Uri(keyVaultEndpoint), new ClientSecretCredential(tenantId, clientId, clientSecret));
            var connectionRabbitmqUrl = secretClient.GetSecret("connectionRabbitmqUrl").Value.Value;
            var rabbitMqUsername = secretClient.GetSecret("RabbitmqUsername").Value.Value;
            var rabbitMqPassword = secretClient.GetSecret("rabbitMqPassword").Value.Value;
            factory = new ConnectionFactory { Uri =  new Uri(connectionRabbitmqUrl), UserName = rabbitMqUsername, Password = rabbitMqPassword };
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
