using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Amqp.Serialization;
using Microsoft.ServiceBus.Messaging;
using TopicClient = Microsoft.Azure.ServiceBus.TopicClient;

namespace StockMessager
{
    public class Program
    {
        static string ServiceBusConnectionString = ConfigurationManager.ConnectionStrings["StockEventServiceBus"].ToString();
        const string TopicName = "stocklevels";
        static MessagingFactory factory = MessagingFactory.CreateFromConnectionString(ServiceBusConnectionString);
        static MessageSender sender = factory.CreateMessageSender(TopicName);

        public static void Main(string[] args)
        {
           
            var stockMessagerService = new StockMessagerService(sender);
            var stockItem = new StockItem("ABC", 1, "FCLondon", "DHL");

            Console.WriteLine("Enter batch size to send messages");
            var stockItemBatch = new List<StockItem>();

            string cmd;
            while ((cmd = Console.ReadLine()) != "exit")
            {
                var numberOfMessages = Int32.Parse(cmd);
                for (int i = 0; i < numberOfMessages; i++)
                {
                    //stockMessagerService.SendMessageAsync(stockItem).Wait();
                    stockItemBatch.Add(stockItem);
                }
                stockMessagerService.SendMessageBatchAsync(stockItemBatch).Wait();
                Console.WriteLine($"Message Batch sent");

                stockMessagerService.SendMessageAsync(new StockItem("DEF", 1, "FCWestLondon", "UPS")).Wait();
                Console.WriteLine($"Message with SKU 'DEF' sent");


                stockMessagerService.SendDeadletterMessageAsync(stockItem).Wait();
                Console.WriteLine($"Poisoned Message sent");
            }
            

        }

    }
}
