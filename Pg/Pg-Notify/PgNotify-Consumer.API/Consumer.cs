using Commons;
using Npgsql;

namespace PgNotify_Consumer.API;

public class Consumer(IConfiguration configuration) : BackgroundService
{
    private readonly IConfiguration _configuration = configuration;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            using var conn = new NpgsqlConnection(_configuration.GetConnectionString("Database"));
            conn.Open();

            // Inscreve-se no canal para receber notificações
            using (var listenCommand = conn.CreateCommand())
            {
                listenCommand.CommandText = $"LISTEN {Constants.Channel}";
                listenCommand.ExecuteNonQuery();
            }

            // Configura o evento de notificação
            conn.Notification += (o, e) =>
            {
                Console.WriteLine($"Recebida notificação: {e.Channel}, Payload: {e.Payload}");
            };

            Console.WriteLine("Aguardando notificações...");

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