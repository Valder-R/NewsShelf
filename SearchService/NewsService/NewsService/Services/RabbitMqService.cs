using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace NewsService.Services;



public interface IRabbitMqService
{
    Task PublishAsync<T>(string queue, T message) where T : class;
}

public class RabbitMqService : IRabbitMqService
{
    private readonly IConnection _connection;
    private readonly ILogger<RabbitMqService> _logger;

    public RabbitMqService(IConfiguration configuration, ILogger<RabbitMqService> logger)
    {
        _logger = logger;
        var hostname = configuration.GetValue<string>("RabbitMq:Hostname") ?? "localhost";
        var port = configuration.GetValue<int>("RabbitMq:Port", 5672);
        var username = configuration.GetValue<string>("RabbitMq:Username") ?? "guest";
        var password = configuration.GetValue<string>("RabbitMq:Password") ?? "guest";
        
        var factory = new ConnectionFactory()
        {
            HostName = hostname,
            Port = port,
            UserName = username,
            Password = password
        };

        _connection = factory.CreateConnection();
    }

    public async Task PublishAsync<T>(string queue, T message) where T : class
    {
        try
        {
            using (var channel = _connection.CreateModel())
            {
                channel.QueueDeclare(
                    queue: queue,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );

                var json = JsonSerializer.Serialize(message);
                var body = Encoding.UTF8.GetBytes(json);

                var basicProperties = channel.CreateBasicProperties();
                basicProperties.Persistent = true;
                basicProperties.ContentType = "application/json";
                basicProperties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

                channel.BasicPublish(
                    exchange: "",
                    routingKey: queue,
                    basicProperties: basicProperties,
                    body: body
                );

                _logger.LogInformation($"Published message to queue '{queue}'");
            }
            
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error publishing to queue '{queue}': {ex.Message}");
            throw;
        }
    }
}
