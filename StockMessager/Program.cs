using System;
using System.Configuration;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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

            Console.WriteLine("Press any key to continue and send messages");
            var numberOfMessages = 5;
            //todo send deadletter message
            string cmd;
            while ((cmd = Console.ReadLine()) != "exit")
            {
                for (int i = 0; i < numberOfMessages; i++)
                {
                    stockMessagerService.SendMessageAsync(stockItem).Wait();
                    Console.WriteLine($"Message {i} sent");
                }
            }
            

        }

    }
}
