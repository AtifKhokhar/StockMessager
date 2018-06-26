using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using StockListener.Repository;

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
            var stockMessageHandler = new StockItemHandler();
            var topicClient = factory.CreateTopicClient(TopicName);
            var subscriptionClient = factory.CreateSubscriptionClient(topicClient.Path, SubscriptionName);
            StockTransactionLogsDB stockTransactionLogsDb = new StockTransactionLogsDB();
            var stockListener = new StockListenerService(subscriptionClient, stockMessageHandler, stockTransactionLogsDb);
            stockListener.ListenToMessages(CancellationToken.None);
            CreateMultipleSubscriptions(topicClient);
            Console.ReadLine();
        }

        private static void CreateMultipleSubscriptions(TopicClient topicClient)
        {
            //todo refactor this code to avoid repetitiion
            var stockMessageHandler = new StockItemHandler();
            CreateSubscriptionToListenMessages(topicClient, stockMessageHandler, "GBWestWarehouseStockLevels");
            CreateSubscriptionToListenMessages(topicClient, stockMessageHandler, "GBEastWarehouseStockLevels");
        }

        private static void CreateSubscriptionToListenMessages(TopicClient topicClient, StockItemHandler stockMessageHandler, string name)
        {
            StockTransactionLogsDB stockTransactionLogsDb = new StockTransactionLogsDB();
            var subscriptionClient = factory.CreateSubscriptionClient(topicClient.Path, name);
            var stockListener = new StockListenerService(subscriptionClient, stockMessageHandler, stockTransactionLogsDb);
            stockListener.ListenToMessages(CancellationToken.None);
        }
    }
}
