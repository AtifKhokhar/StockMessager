using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;
using StockListener.Repository.Entity;
using StockMessager;

namespace StockListener.Repository
{
    public class StockTransactionLogsDB
    {

        public void PersistToDB(StockItem stockItem)
        {
            using (var sqlConnection
                = new SqlConnection(ConfigurationManager.ConnectionStrings["DBConnString"].ConnectionString))
            {
                sqlConnection.Open();

                var stockTransactionLog = new StockTransactionLog()
                {
                    TransactionId = Guid.NewGuid().ToString(),
                    Sku = stockItem.Sku,
                    Quantity = stockItem.Quantity,
                    Warehouse = stockItem.Warehouse,
                    DeliveryProvider = stockItem.DeliveryProvider,
                    DateTransactionReceived = DateTime.UtcNow
                };

                try
                {

                    sqlConnection.Insert(stockTransactionLog);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }

                sqlConnection.Close();

                Console.WriteLine("Done. ");
            }
        }
    }
}
