using Commons;
using Npgsql;
using System.Text.Json;

namespace PgNotify_Consumer.API;

public class Response(IConfiguration configuration)
{
    private readonly IConfiguration _configuration = configuration;

    public async Task<bool> Notification(Notification notification)
    {
        try
        {
            using var conn = new NpgsqlConnection(_configuration.GetConnectionString("Database"));
            conn.Open();

            // Comando para enviar a notificação
            using var cmd = new NpgsqlCommand($"NOTIFY {Config.Channel2}, '{JsonSerializer.Serialize(notification)}'", conn);

            await cmd.ExecuteNonQueryAsync();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Erro ao enviar notificação: " + ex.Message);
        }
        return false;
    }
}
