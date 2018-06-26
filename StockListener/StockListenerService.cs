using System;
using System.Globalization;
using System.IO;
using System.Threading;
using Newtonsoft.Json;
using StockListener.Repository;
using StockMessager;
using SubscriptionClient = Microsoft.ServiceBus.Messaging.SubscriptionClient;

namespace StockListener
{
    public class StockListenerService
    {
        private readonly SubscriptionClient _subscriptionClient;
        private readonly StockItemHandler _handler;
        private readonly StockTransactionLogsDB _stockTransactionLogsDb;

        public StockListenerService(SubscriptionClient subscriptionClient, StockItemHandler handler, StockTransactionLogsDB stockTransactionLogsDb)
        {
            _subscriptionClient = subscriptionClient;
            _handler = handler;
            _stockTransactionLogsDb = stockTransactionLogsDb;
        }

        public void ListenToMessages(CancellationToken token)
        {
            Console.WriteLine("Listening to Stock Events....\n");
            Console.WriteLine("*******************************************\n");

            token.Register(() => _subscriptionClient.CloseAsync());


            _subscriptionClient.OnMessageAsync(async message =>
            {
                if (message.ContentType != "application/json")
                {
                    await message.DeadLetterAsync("Invalid Content Type", $"Unable to process a message with a Content Type of {message.ContentType}");
                    return;
                }

                Console.WriteLine($"New Stock Recieved On {_subscriptionClient.Name} subscription:\n");
                Console.WriteLine($"Message Label: {message.Label}\n");
                Console.WriteLine($"Message Content Type: {message.ContentType}\n");
                Console.WriteLine($"Message Sent Time: {message.EnqueuedTimeUtc.ToString(CultureInfo.InvariantCulture)}\n");


                Stream messageBodyStream = message.GetBody<Stream>();
                string messageBodyContent = await new StreamReader(messageBodyStream).ReadToEndAsync();
                StockItem stockItem = JsonConvert.DeserializeObject<StockItem>(messageBodyContent);
                _handler.HandleStockItem(stockItem);
                _stockTransactionLogsDb.PersistToDB(stockItem);
                Console.WriteLine("*******************************************\n");
                await message.CompleteAsync();
            }
            );
        }
    }
}
