using System;
using System.Text;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var builder = new ConfigurationBuilder();
builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

IConfiguration config = builder.Build();
var keyVaultEndpoint = config.GetSection("KeyVault:BaseUrl").Value;
var clientId = config.GetSection("AzureAd:ClientId").Value;
var clientSecret = config.GetSection("AzureAd:ClientSecret").Value;
var tenantId = config.GetSection("AzureAd:TenantId").Value;
var secretClient = new SecretClient(new Uri(keyVaultEndpoint), new ClientSecretCredential(tenantId, clientId, clientSecret));
var connectionRabbitmqUrl = secretClient.GetSecret("connectionRabbitmqUrl").Value.Value;
var rabbitMqUsername = secretClient.GetSecret("RabbitmqUsername").Value.Value;
var rabbitMqPassword = secretClient.GetSecret("rabbitMqPassword").Value.Value;
var factory = new ConnectionFactory { Uri = new Uri(connectionRabbitmqUrl), UserName = rabbitMqUsername, Password = rabbitMqPassword};
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.QueueDeclare(queue: "messagingqueue",
                     durable: false,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);

Console.WriteLine(" [*] Waiting for messages.");

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($" [x] Received {message}");
};
channel.BasicConsume(queue: "messagingqueue",
                     autoAck: true,
                     consumer: consumer);

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();