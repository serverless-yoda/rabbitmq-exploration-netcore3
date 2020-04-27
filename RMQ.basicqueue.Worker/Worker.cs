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
                //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                /*
             * This tells RabbitMQ not to give more than one message to a worker at a time. 
             * Or, in other words, don't dispatch a new message to a worker until it has processed and acknowledged the previous one. 
             * Instead, it will dispatch it to the next worker that is not still busy.
             */
                _builder.Model.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);


                var consumer = new AsyncEventingBasicConsumer(_builder.Model);
                consumer.Received += Consumer_Received;

                /*
                 setting autoAck: false will be sure that even if you kill a worker using CTRL+C while it was processing a message, 
                 nothing will be lost. Soon after the worker dies all unacknowledged messages will be redelivered.
                 */

                _builder.Model.BasicConsume(queue: Utility.QUEUE_NAME,
                                            autoAck: false,
                                            consumer: consumer);

                await Task.Delay(1000, stoppingToken);
            }
        }

        private async Task Consumer_Received(object sender, BasicDeliverEventArgs e)
        {

            System.ReadOnlyMemory<byte> body = e.Body;
            var message = Encoding.UTF8.GetString(body.ToArray());
            var eventName = e.RoutingKey;

            await ProcessEvent(null, eventName).ConfigureAwait(false);

            ((AsyncEventingBasicConsumer)sender).Model.BasicAck(deliveryTag: e.DeliveryTag, multiple: false);
        }

        private async Task ProcessEvent(string body, string eventName)
        {
            Write(body + "=" + eventName);
            _logger.LogInformation("Event : {eventName} , Message : {message}", eventName,body);
            await Task.CompletedTask;
        }
    }
}
