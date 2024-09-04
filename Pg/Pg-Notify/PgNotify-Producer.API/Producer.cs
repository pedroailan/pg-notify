using Commons;
using Npgsql;

namespace PgNotify_Producer.API;

public class Producer(IConfiguration configuration)
{
    private readonly IConfiguration _configuration = configuration;

    public async Task Notification(Notification notification)
    {
        try
        {
            using var conn = new NpgsqlConnection(_configuration.GetConnectionString("Database"));
            conn.Open();

            // Comando para enviar a notificação
            using var cmd = new NpgsqlCommand($"NOTIFY {Constants.Channel}, '{notification.Message}'", conn);

            await cmd.ExecuteNonQueryAsync();
            Console.WriteLine($"Notificação enviada para o canal '{Constants.Channel}' com a mensagem: {notification.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Erro ao enviar notificação: " + ex.Message);
        }
    }
}