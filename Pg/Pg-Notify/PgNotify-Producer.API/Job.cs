using Commons;

namespace PgNotify_Producer.API
{
    public class Job(Producer producer) : BackgroundService
    {
        private readonly Producer _producer = producer;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                Console.WriteLine("\n");
                await _producer.Notification(new Notification());
                await Task.Delay(Notify.Interval, stoppingToken);
            }
        }
    }
}