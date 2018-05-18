using System;
using System.Globalization;
using System.IO;
using System.Threading;
using Newtonsoft.Json;
using StockMessager;
using SubscriptionClient = Microsoft.ServiceBus.Messaging.SubscriptionClient;

namespace StockListener
{
    public class StockListenerService
    {
        private readonly SubscriptionClient _subscriptionClient;
        private readonly CancellationToken _token;

        public StockListenerService(SubscriptionClient subscriptionClient, CancellationToken token)
        {
            _subscriptionClient = subscriptionClient;
            _token = token;
        }

        public void ListenToMessages()
        {
            Console.WriteLine("Listening to Stock Events....\n");
            Console.WriteLine("*******************************************\n");

            _subscriptionClient.OnMessageAsync(async message =>
                {
                    Console.WriteLine("New Stock Recieved:\n");
                    Console.WriteLine($"Message Label: {message.Label}\n");
                    Console.WriteLine($"Message Content Type: {message.ContentType}\n");
                    Console.WriteLine($"Message Sent Time: {message.EnqueuedTimeUtc.ToString(CultureInfo.InvariantCulture)}\n");
                    

                    Stream messageBodyStream = message.GetBody<Stream>();
                    string messageBodyContent = await new StreamReader(messageBodyStream).ReadToEndAsync();
                    StockItem stockItem = JsonConvert.DeserializeObject<StockItem>(messageBodyContent);
                    //todo refactor out to to private methods
                    //Todo handler class that handles stock item object messages
                    Console.WriteLine($"Stock Item SKU: {stockItem.Sku}\n");
                    Console.WriteLine($"Stock Item Warehouse: {stockItem.Warehouse}\n");
                    Console.WriteLine($"Stock Item Quantity: {stockItem.Quantity}\n");

                    Console.WriteLine("*******************************************\n");
                    await message.CompleteAsync();
                    _token.Register(() => _subscriptionClient.CloseAsync());
                }
            );
        }


    }
}
