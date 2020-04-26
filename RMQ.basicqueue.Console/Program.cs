using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RMQ.Application;
using RMQ.Domain;
using RMQ.Infrastructure.Common;
using System;
using System.Text;
using static System.Console;

namespace RMQ.basicqueue.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateQueues();
            //System.Threading.Thread.Sleep(1000);
            ReceiveQueue();
        }

        private static void CreateQueues()
        {
            int counter = 1000;
            Random rnd = new Random();

            var builder = QueueDirectorBuilder
                            .NewQueueBuilderInfo
                            .CreateConnection("localhost", "guest", "guest")
                            .CreateQueue()
                            .Build();

            for (int i = 0; i < counter; i++)
            {
                var payment = new PaymentTransaction
                {
                    AmountToPay = rnd.Next(100),
                    CardNumber = String.Concat("xxxxx-", i.ToString()),
                    CardTypeId = rnd.Next(3),
                    Id = rnd.Next(1, 13),
                    Name = "Manny" + rnd.Next(1, 10)
                };

                var message = JsonConvert.SerializeObject("Hello World!");
                var body = Encoding.UTF8.GetBytes(message);

                builder.Model.BasicPublish("", Utility.QUEUE_NAME, null, body);
                WriteLine("[x] Payment Message Sent: {0} : {1}", payment.CardNumber, payment.AmountToPay);
            }
        }

        public static void ReceiveQueue()
        {
            var builder = QueueDirectorBuilder
                            .NewQueueBuilderInfo
                            .CreateConnection("localhost", "guest", "guest")
                            .CreateQueue()
                            .Build();


            var consumer = new EventingBasicConsumer(builder.Model);
            try
            {
                consumer.Received += (model, ea) =>
                            {
                                var message = Encoding.UTF8.GetString(ea.Body.Serialize());

                                WriteLine("[x] Payment Message Receive: {0}", message);

                            };
                builder.Model.BasicConsume(queue: Utility.QUEUE_NAME,
                                            autoAck: true,
                                            consumer);
            }
            catch (Exception ex)
            {
                WriteLine(ex.Message);
            }


            WriteLine(" Press enter to exit");

        }
    }
}
