using Npgsql;
namespace PgNotify.APP;
internal class Connection
{
    public static string ConnectionString = "Host=host.docker.internal;Username=postgres;Password=postgrespw;Database=pgnotify;Port=32770";

    public static async Task Listen()
    {
        using var conn = new NpgsqlConnection(ConnectionString);
        conn.Open();

        // Inscreve-se no canal para receber notificações
        using (var listenCommand = conn.CreateCommand())
        {
            string directory = RelativePath();
            string sqlCommandText = File.ReadAllText(directory + "\notify.sql");
            listenCommand.CommandText = "LISTEN my_channel";
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

    private static string RelativePath()
    {
        var assembly = AppDomain.CurrentDomain.BaseDirectory;
        string relativePath = assembly;
        return relativePath;
    }
}
