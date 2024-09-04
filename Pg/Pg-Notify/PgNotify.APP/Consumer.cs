using Commons;
using Npgsql;
namespace PgNotify.APP;
internal class Consumer
{
    internal static async Task Listen()
    {
        try
        {
            using var conn = new NpgsqlConnection(Constants.Host);
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
                await conn.WaitAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }
}
