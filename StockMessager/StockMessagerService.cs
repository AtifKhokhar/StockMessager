using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;

namespace StockMessager
{
    public class StockMessagerService
    {
        private readonly MessageSender messageSender;

        public StockMessagerService(MessageSender messageSender)
        {
            this.messageSender = messageSender;
        }


        public async Task SendMessageAsync(StockItem stockItem)
        {
            var serializedStockItem = JsonConvert.SerializeObject(stockItem);
            BrokeredMessage message = BuildBrokeredMessage(stockItem, serializedStockItem);
            message.Properties.Add("Warehouse",stockItem.Warehouse);
            await messageSender.SendAsync(message);
        }

        private static BrokeredMessage BuildBrokeredMessage(StockItem stockItem, string serializedStockItem)
        {
            return new BrokeredMessage(new MemoryStream(Encoding.UTF8.GetBytes(serializedStockItem)), true)
            {
                ContentType = "application/json",
                Label = stockItem.GetType().ToString()
            };
            
        }

        public async Task SendMessageBatchAsync(List<StockItem> stockItems)
        {
            var brokeredMessages = new List<BrokeredMessage>();
            foreach (var stockItem in stockItems)
            {
                var serializedStockItem = JsonConvert.SerializeObject(stockItem);
                BrokeredMessage message = BuildBrokeredMessage(stockItem, serializedStockItem);
                message.Properties.Add("Warehouse", stockItem.Warehouse);
                brokeredMessages.Add(message);
            }

            await messageSender.SendBatchAsync(brokeredMessages);
        }

        public async Task SendDeadletterMessageAsync(StockItem stockItem)
        {
            var serializedStockItem = JsonConvert.SerializeObject(stockItem);
            BrokeredMessage message =
                new BrokeredMessage(new MemoryStream(Encoding.UTF8.GetBytes(serializedStockItem)), true)
                {
                    ContentType = "application/xml",
                    Label = stockItem.GetType().ToString()
                };
            message.Properties.Add("Warehouse", stockItem.Warehouse);


            await messageSender.SendAsync(message);
        }
    }
}