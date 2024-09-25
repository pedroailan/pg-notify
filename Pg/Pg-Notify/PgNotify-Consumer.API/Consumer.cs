﻿using Commons;
using Npgsql;
using System.Text.Json;

namespace PgNotify_Consumer.API;

public class Consumer(IConfiguration configuration, Response response) : BackgroundService
{
    private readonly IConfiguration _configuration = configuration;
    private readonly Response _response = response;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            using var conn = new NpgsqlConnection(_configuration.GetConnectionString("Database"));
            conn.Open();

            // Inscreve-se no canal para receber notificações
            using (var listenCommand = conn.CreateCommand())
            {
                listenCommand.CommandText = $"LISTEN {Notify.Channel1}";
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
                Console.WriteLine("\n");
                Console.WriteLine($"Tempo total entre produção e consumo: {latency} ms");
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