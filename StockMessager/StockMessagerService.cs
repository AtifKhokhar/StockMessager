using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.ServiceBus.Messaging;

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
            BrokeredMessage message = new BrokeredMessage(stockItem);
            messageSender.SendAsync(message);
        }
    }
}