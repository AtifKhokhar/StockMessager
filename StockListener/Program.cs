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
        static MessagingFactory factory = MessagingFactory.CreateFromConnectionString(ServiceBusConnectionString);
        
        static void Main(string[] args)
        {
            var topicClient = factory.CreateTopicClient(TopicName);
            CreateMultipleSubscriptions(topicClient);
            Console.ReadLine();
        }

        private static void CreateMultipleSubscriptions(TopicClient topicClient)
        {
            StockTransactionLogsDB stockTransactionLogsDb = new StockTransactionLogsDB();
            var stockMessageHandler = new StockItemHandler(stockTransactionLogsDb);
            CreateSubscriptionToListenMessages(topicClient, stockMessageHandler, "GBWarehouseStockLevels");
            CreateSubscriptionToListenMessages(topicClient, stockMessageHandler, "GBWestWarehouseStockLevels");
            CreateSubscriptionToListenMessages(topicClient, stockMessageHandler, "GBEastWarehouseStockLevels");
        }

        private static void CreateSubscriptionToListenMessages(TopicClient topicClient, StockItemHandler stockMessageHandler, string name)
        {
            var subscriptionClient = factory.CreateSubscriptionClient(topicClient.Path, name);
            var stockListener = new StockListenerService(subscriptionClient, stockMessageHandler);
           // stockListener.ListenToBatchMessages(CancellationToken.None);
            stockListener.ListenToMessages(CancellationToken.None);
        }
    }
}
