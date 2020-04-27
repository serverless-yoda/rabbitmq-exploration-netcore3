using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RMQ.Application;
using RMQ.Domain;
using RMQ.Infrastructure.Common;
using System;
//using RMQ.Application;
//using RMQ.Domain;
//using RMQ.Infrastructure.Common;
using System.Text;
using static System.Console;

namespace RMQ.basicqueue.Console
{
    class Program
    {
        private static void Main(string[] args)
        {
            if (args is null)
            {
                throw new ArgumentNullException(nameof(args));
            }
            CreateQueues();
        }

        private static void CreateQueues()
        {
            int counter = 100;
            Random rnd = new Random();

            var builder = QueueDirectorBuilder
                            .NewQueueBuilderInfo
                            .CreateConnection()
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

                var message = JsonConvert.SerializeObject(payment);
                var body = Encoding.UTF8.GetBytes(message);

                builder.Model.BasicPublish("", Utility.QUEUE_NAME, null, body);
                WriteLine("[x] Payment Message Sent: {0} : {1}", payment.CardNumber, payment.AmountToPay);

                System.Threading.Thread.Sleep(1000);
            }
        }

        #region Test
        public static void SendQueue()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "hello",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                string message = "Hello World!";
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                     routingKey: "hello",
                                     basicProperties: null,
                                     body: body);
                WriteLine(" [x] Sent {0}", message);
            }

            WriteLine(" Press [enter] to exit.");
            ReadLine();
        }


        public static void Received()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.QueueDeclare(queue: Utility.QUEUE_NAME,
                                durable: false,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                System.ReadOnlyMemory<byte> body = ea.Body;
                var message = Encoding.UTF8.GetString(body.ToArray());
                WriteLine(" [x] Received {0}", message);
            };
            channel.BasicConsume(queue: Utility.QUEUE_NAME,
                                 autoAck: true,
                                 consumer: consumer);

            WriteLine(" Press [enter] to exit.");
            ReadLine();
        }

        #endregion

    }
}
