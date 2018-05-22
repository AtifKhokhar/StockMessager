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
            var topicClient = factory.CreateTopicClient(TopicName);
            var subscriptionClient = factory.CreateSubscriptionClient(topicClient.Path, SubscriptionName);
            var stockListener = new StockListenerService(subscriptionClient,CancellationToken.None);
            stockListener.ListenToMessages();
            CreateMultipleSubscriptions(topicClient);
            Console.ReadLine();
        }

        private static void CreateMultipleSubscriptions(TopicClient topicClient)
        {
            var subscriptionClientTwo = factory.CreateSubscriptionClient(topicClient.Path, "GBWestWarehouseStockLevels");
            var stockListenerTwo = new StockListenerService(subscriptionClientTwo, CancellationToken.None);
            stockListenerTwo.ListenToMessages();

            var subscriptionClientThree = factory.CreateSubscriptionClient(topicClient.Path, "GBEastWarehouseStockLevels");
            var stockListenerThree = new StockListenerService(subscriptionClientThree, CancellationToken.None);
            stockListenerThree.ListenToMessages();
        }
    }
}
