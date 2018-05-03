using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace StockMessager
{
    [DataContract]
    public class StockItem
    {
        public StockItem(string sku, int quantity, string warehouse, string deliveryProvider)
        {
            Sku = sku;
            Quantity = quantity;
            Warehouse = warehouse;
            DeliveryProvider = deliveryProvider;
        }

        [DataMember]
        public string Sku { get; set; }
        [DataMember]
        public int Quantity { get; set; }
        [DataMember]
        public string Warehouse { get; set; }
        [DataMember]
        public string DeliveryProvider { get; set; }
    }
}
