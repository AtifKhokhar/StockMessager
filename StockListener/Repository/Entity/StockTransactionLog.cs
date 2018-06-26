using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;

namespace StockListener.Repository.Entity
{
    [Table("StockTransactionLogs")]
    public class StockTransactionLog
    {
        public string TransactionId { get; set; }
        public string Sku { get; set; }
        public int Quantity { get; set; }
        public string Warehouse { get; set; }
        public string DeliveryProvider { get; set; }
        public DateTime DateTransactionReceived { get; set; }


    }
}
