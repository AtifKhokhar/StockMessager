using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockMessager;

namespace StockListener
{
    public class StockItemHandler
    {
        public void HandleStockItem(StockItem stockItem)
        {
            Console.WriteLine($"Stock Item SKU: {stockItem.Sku}\n");
            Console.WriteLine($"Stock Item Warehouse: {stockItem.Warehouse}\n");
            Console.WriteLine($"Stock Item Quantity: {stockItem.Quantity}\n");
        }
    }
}
