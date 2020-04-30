using RabbitMQ.Client;
using RMQ.Application;
using RMQ.Infrastructure.Common;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Console;

namespace RMQ.directexchange.Console
{
    class Program
    {

        static void Main(string[] args)
        {
            CreateDirectExchangeMessage();
            WriteLine(" Press [enter] to exit.");
            ReadLine();
        }

        static void CreateDirectExchangeMessage()
        {
            var builder = QueueDirectorBuilder
                    .NewQueueBuilderInfo
                    .CreateConnection()
                    .CreateDirectExchange()
                    .Build();

            List<string> severity = new List<string>
            {
                "error",
                "info",
                "debug",
                "warning"
            };

            for(int i = 1; i < 10; i++)
            {
                int index = new Random().Next(1, 4);
                string currentSeverity = severity[index];

                string message = String.Concat(currentSeverity, " message # :", i.ToString());
                var body = Encoding.UTF8.GetBytes(message);

                builder.Model.BasicPublish(exchange: Utility.DIRECT_EXCHANGE,
                                           routingKey: currentSeverity,
                                           basicProperties: null,
                                           body: body);


                WriteLine(" [x] Sent {0}", message);
            }
          
        }
    }
}
