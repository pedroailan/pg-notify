using Commons;
using Npgsql;
using System.Text.Json;

namespace PgNotify_Consumer.API;

public class Consumer(IConfiguration configuration, Response response, ILogger<Consumer> logger) : BackgroundService
{
    private readonly IConfiguration _configuration = configuration;
    private readonly Response _response = response;
    private readonly ILogger<Consumer> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            using var conn = new NpgsqlConnection(_configuration.GetConnectionString("Database"));
            conn.Open();

            // Inscreve-se no canal para receber notificações
            using (var listenCommand = conn.CreateCommand())
            {
                listenCommand.CommandText = $"LISTEN {Config.Channel1}";
                listenCommand.ExecuteNonQuery();
            }

            // Configura o evento de notificação
            conn.Notification += async (o, e) =>
            {
                Notification message = JsonSerializer.Deserialize<Notification>(e.Payload);

                // Tempo atual (recebimento da mensagem)
                long receivedTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                long latency = message.CalculateTime(receivedTimestamp);

                await _response.Notification(message);
                _logger.LogInformation("{0};{1}", DateTime.Now.ToString("HH:mm:ss:fff"), latency);
            };

            // Mantém a conexão aberta para receber notificações
            while (true)
            {
                await conn.WaitAsync(stoppingToken);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }
}