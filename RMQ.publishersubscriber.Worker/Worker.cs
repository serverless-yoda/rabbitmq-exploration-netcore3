using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RMQ.Application;
using RMQ.Infrastructure.Common;

namespace RMQ.publishersubscriber.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly Queue _model;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
            var model = QueueDirectorBuilder
                            .NewQueueBuilderInfo
                            .CreateConnection()
                            .CreateExchange()
                            .Build();

            _model = model;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                var randomQueuename = _model.Model.QueueDeclare().QueueName;
                _model.Model.QueueBind(queue: randomQueuename, 
                                       exchange: Utility.EXCHANGE_NAME, 
                                       routingKey: "");

                _logger.LogInformation("waiting for logs");

                var consumer = new AsyncEventingBasicConsumer(_model.Model);
                consumer.Received += async (model, ea) =>
                {
                    System.ReadOnlyMemory<byte> body = ea.Body;
                    var message = Encoding.UTF8.GetString(body.ToArray());
                    _logger.LogInformation("logs recevied: {message}", message);

                    await Task.CompletedTask;
                };

                _model.Model.BasicConsume(randomQueuename, true, consumer);

                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
