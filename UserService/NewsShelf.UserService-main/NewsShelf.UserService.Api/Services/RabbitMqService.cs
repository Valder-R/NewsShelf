using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace NewsShelf.UserService.Api.Services;

/// <summary>
/// RabbitMQ message publisher service
/// </summary>
public interface IRabbitMqService
{
    Task PublishAsync<T>(string queue, T message) where T : class;
}

public class RabbitMqService : IRabbitMqService
{
    private readonly IConnection _connection;
    private readonly ILogger<RabbitMqService> _logger;
    private readonly string _hostname;
    private readonly int _port;

    public RabbitMqService(IConfiguration configuration, ILogger<RabbitMqService> logger)
    {
        _logger = logger;
        _hostname = configuration.GetValue<string>("RabbitMq:Hostname") ?? "localhost";
        _port = configuration.GetValue<int>("RabbitMq:Port", 5672);
        
        var factory = new ConnectionFactory()
        {
            HostName = _hostname,
            Port = _port,
            UserName = configuration.GetValue<string>("RabbitMq:Username") ?? "guest",
            Password = configuration.GetValue<string>("RabbitMq:Password") ?? "guest"
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

                _logger.LogInformation($"Published message to queue '{queue}': {json}");
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
