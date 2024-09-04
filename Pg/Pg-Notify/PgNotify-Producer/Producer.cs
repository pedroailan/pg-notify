using Commons;
using Npgsql;

namespace PgNotify_Producer
{
    internal class Producer
    {
        public static async Task Notification(string message)
        {
            try
            {
                using var conn = new NpgsqlConnection(Constants.Host);
                conn.Open();

                // Comando para enviar a notificação
                using var cmd = new NpgsqlCommand($"NOTIFY {Constants.Channel}, '{message}'", conn);

                await cmd.ExecuteNonQueryAsync();
                Console.WriteLine($"Notificação enviada para o canal '{Constants.Channel}' com a mensagem: {message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao enviar notificação: " + ex.Message);
            }
        }
    }
}
