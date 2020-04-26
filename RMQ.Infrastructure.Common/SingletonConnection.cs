using RabbitMQ.Client;
using System;

namespace RMQ.Infrastructure.Common
{
    public class SingletonConnection
    {
        private static ConnectionFactory instance = new ConnectionFactory { HostName = "localhost" };

        private SingletonConnection()
        {}

        public static ConnectionFactory GetInstance() {
            return instance;
        }

    }
}
