using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace StockListener
{
    class Program
    {
        static string ServiceBusConnectionString = ConfigurationManager.ConnectionStrings["StockEventServiceBus"].ToString();
        const string TopicName = "stocklevels";
        private const string SubscriptionName = "GBWarehouseStockLevels";
        static MessagingFactory factory = MessagingFactory.CreateFromConnectionString(ServiceBusConnectionString);
        static void Main(string[] args)
        {
            var topicClient = factory.CreateTopicClient("stocklevels");
            var subscriptionClient = factory.CreateSubscriptionClient(topicClient.Path, "GBWarehouseStockLevels");
            var stockListener = new StockListenerService(subscriptionClient,CancellationToken.None);
            stockListener.ListenToMessages();
            Console.ReadLine();
        }
    }
}
