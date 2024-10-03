using Commons;
using Npgsql;
using System.Text.Json;

namespace PgNotify_Producer.API;

public class Listen(IConfiguration configuration, ILogger<Listen> logger) : BackgroundService
{
    private readonly IConfiguration _configuration = configuration;
    private readonly ILogger<Listen> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            using var conn = new NpgsqlConnection(_configuration.GetConnectionString("Database"));
            conn.Open();

            // Inscreve-se no canal para receber notificações
            using (var listenCommand = conn.CreateCommand())
            {
                listenCommand.CommandText = $"LISTEN {Config.Channel2}";
                listenCommand.ExecuteNonQuery();
            }

            // Configura o evento de notificação
            conn.Notification += (o, e) =>
            {
                Notification message = JsonSerializer.Deserialize<Notification>(e.Payload);

                // Tempo atual (recebimento da mensagem)
                long receivedTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                long latency = message.CalculateTime(receivedTimestamp);

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
