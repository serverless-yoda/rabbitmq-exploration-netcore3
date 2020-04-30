using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RMQ.Application;
using RMQ.Infrastructure.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace RMQ.directexchange.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly Queue model;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;

            model = QueueDirectorBuilder.NewQueueBuilderInfo
                        .CreateConnection()
                        .CreateDirectExchange()
                        .Build();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            List<string> severity = new List<string>
            {
                "error",
                "info",
                "debug",
                "warning"
            };

            var queueName = model.Model.QueueDeclare().QueueName;

            int index = new Random().Next(1, 4);
            string currentSeverity = severity[index];

            foreach(var item in severity)
            {
                model.Model.QueueBind(queue:queueName, 
                                      Utility.DIRECT_EXCHANGE, 
                                      routingKey: item);
            }

            model.Model.BasicQos(0, 1, false);

            while (!stoppingToken.IsCancellationRequested)
            {

                var consumer = new AsyncEventingBasicConsumer(model.Model);
                consumer.Received += Consumer_Received;

                model.Model.BasicConsume(queue: queueName, 
                                         autoAck: true, 
                                         consumer);
                await Task.Delay(1000, stoppingToken);
            }

           
        }

        private async Task Consumer_Received(object sender, BasicDeliverEventArgs e)
        {

            System.ReadOnlyMemory<byte> body = e.Body;
            var message = Encoding.UTF8.GetString(body.ToArray());
            var eventName = e.RoutingKey;

            await ProcessEvent(message, eventName).ConfigureAwait(false);

            ((AsyncEventingBasicConsumer)sender).Model.BasicAck(deliveryTag: e.DeliveryTag, multiple: false);
        }

        private async Task ProcessEvent(string body, string eventName)
        {
            
            _logger.LogInformation("Event : {eventName} , Message : {message}", eventName, body);
            await Task.CompletedTask;
        }
    }
}

