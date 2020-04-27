using RabbitMQ.Client;

namespace RMQ.Infrastructure.Common
{
    public class SingletonConnection
    {
        private static readonly ConnectionFactory instance = new ConnectionFactory 
        { 
            HostName = "localhost" , 
            DispatchConsumersAsync = true 
        };

        private SingletonConnection()
        { }

        public static ConnectionFactory GetInstance()
        {
            return instance;
        }

    }
}
