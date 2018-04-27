using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockMessager
{
    public class StockItem
    {
        public string Sku { get; set; }
        public int Quantity { get; set; }
        public string Warehouse { get; set; }
        public string DeliveryProvider { get; set; }
    }
}
