using System.Configuration;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;

namespace StockMessager
{
    class Program
    {
        string ServiceBusConnectionString = ConfigurationManager.ConnectionStrings["StockEventServiceBus"].ToString();
        const string TopicName = "AtifStockEvent";
        static ITopicClient topicClient;

        static void Main(string[] args)
        {

        }
    }
}
