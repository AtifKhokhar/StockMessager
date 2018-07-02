using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockListener.Repository;
using StockMessager;

namespace StockListener
{
    public class StockItemHandler
    {
        private readonly StockTransactionLogsDB _stockTransactionLogsDb;

        public StockItemHandler(StockTransactionLogsDB stockTransactionLogsDb)
        {
            _stockTransactionLogsDb = stockTransactionLogsDb;
        }

        public void HandleStockItem(StockItem stockItem)
        {
            Console.WriteLine($"Stock Item SKU: {stockItem.Sku}\n");
            Console.WriteLine($"Stock Item Warehouse: {stockItem.Warehouse}\n");
            Console.WriteLine($"Stock Item Quantity: {stockItem.Quantity}\n");
            _stockTransactionLogsDb.PersistToDB(stockItem);
            //_stockTransactionLogsDb.PersistToDbSQL(stockItem);


        }

        public void HandleStockItemBatch(IEnumerable<StockItem> stockItems)
        {
            foreach (var item in stockItems)
            {
                HandleStockItem(item);
                _stockTransactionLogsDb.PersistToDB(item);
            }
            
        }
    }
}
