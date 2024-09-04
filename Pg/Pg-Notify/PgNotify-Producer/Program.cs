using PgNotify_Producer;
await Task.Delay(500);
await Producer.Notification("Hello word!");

while (true)
{
    Console.Write("Digite uma notificação: ");
    string? notification = Console.ReadLine();

    if (notification?.ToLower() == "sair")
    {
        Console.WriteLine("Encerrando...");
        break;
    }
    await Producer.Notification(notification ?? string.Empty);
}

//for (int i = 0; i < 100000; i++)
//{
//    await Task.WhenAll(
//        Producer.Notification("Notificacao - A - " + i),
//        Producer.Notification("Notificacao - B - " + i),
//        Producer.Notification("Notificacao - C - " + i),
//        Producer.Notification("Notificacao - D - " + i)
//    );
//}