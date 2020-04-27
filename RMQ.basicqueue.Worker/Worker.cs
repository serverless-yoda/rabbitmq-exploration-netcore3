using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RMQ.Application;
using RMQ.Infrastructure.Common;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Console;

namespace RMQ.basicqueue.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly Queue _builder;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
            var builder = QueueDirectorBuilder.NewQueueBuilderInfo
                            .CreateConnection()
                            .CreateQueue()
                            .Build();
            _builder = builder;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                var consumer = new AsyncEventingBasicConsumer(_builder.Model);
                consumer.Received += Consumer_Received;
                _builder.Model.BasicConsume(Utility.QUEUE_NAME, true, consumer);

                await Task.Delay(1000, stoppingToken);
            }
        }

        private async Task Consumer_Received(object sender, BasicDeliverEventArgs e)
        {

            System.ReadOnlyMemory<byte> body = e.Body;
            var message = Encoding.UTF8.GetString(body.ToArray());
            var eventName = e.RoutingKey;
           
            await ProcessEvent(null, eventName).ConfigureAwait(false);

        }

        private async Task ProcessEvent(string body, string eventName)
        {
            Write(body + "=" + eventName);
            _logger.LogInformation("Event : {message}", body);
            await Task.CompletedTask;
        }
    }
}
