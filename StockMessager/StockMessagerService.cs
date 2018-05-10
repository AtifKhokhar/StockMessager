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
            BrokeredMessage message =
                new BrokeredMessage(new MemoryStream(Encoding.UTF8.GetBytes(serializedStockItem)), true)
                {
                    ContentType = "application/json",
                    Label = stockItem.GetType().ToString()
                };


          await messageSender.SendAsync(message);
        }
    }
}