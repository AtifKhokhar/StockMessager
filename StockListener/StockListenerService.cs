using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
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

        public StockListenerService(SubscriptionClient subscriptionClient, StockItemHandler handler)
        {
            _subscriptionClient = subscriptionClient;
            _handler = handler;
        }

        public void ListenToMessages(CancellationToken token)
        {
            Console.WriteLine("Listening to Stock Events....\n");
            Console.WriteLine("*******************************************\n");

            token.Register(() => _subscriptionClient.CloseAsync());

            _subscriptionClient.OnMessageAsync(async message =>
                {
                    if (await ProcessMessage(message)) return;
                    await message.CompleteAsync();
                }
            );
        }

        private async Task<bool> ProcessMessage(BrokeredMessage message)
         {
            if (message.ContentType != "application/json")
            {
                await message.DeadLetterAsync("Invalid Content Type",
                    $"Unable to process a message with a Content Type of {message.ContentType}");
                return true;
            }

            Console.WriteLine($"New Stock Recieved On {_subscriptionClient.Name} subscription:\n");
            Console.WriteLine($"Message Label: {message.Label}\n");
            Console.WriteLine($"Message Content Type: {message.ContentType}\n");
            Console.WriteLine($"Message Sent Time: {message.EnqueuedTimeUtc.ToString(CultureInfo.InvariantCulture)}\n");


            Stream messageBodyStream = message.GetBody<Stream>();
            string messageBodyContent = await new StreamReader(messageBodyStream).ReadToEndAsync();
            StockItem stockItem = JsonConvert.DeserializeObject<StockItem>(messageBodyContent);

            _handler.HandleStockItem(stockItem);
            Console.WriteLine("*******************************************\n");
            return false;
        }

        public async Task ListenToBatchMessages(CancellationToken token)
        {
            Console.WriteLine("Listening to Stock Batch Events....\n");
            Console.WriteLine("*******************************************\n");

            token.Register(() => _subscriptionClient.CloseAsync());

            while (!token.IsCancellationRequested)
            {
                var messages = await _subscriptionClient.ReceiveBatchAsync(10);

                // deserialize
                // pass stockitems to handler
                // complete 

                await _subscriptionClient.CompleteBatchAsync(messages.Select(m => m.LockToken));
            }
        }
    }
}
