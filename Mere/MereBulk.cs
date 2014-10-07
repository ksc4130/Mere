using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mere
{
    public class MereBulk
    {
        public static bool MereBulkCopy<T, TCopyTo>(IEnumerable<T> toCopyFrom, int batchSize = 1000, bool truncateLength = false)
            where T : new()
            where TCopyTo : new()
        {
            var toInsertList = toCopyFrom.MereCopyTo<T, TCopyTo>().ToList();
            if (toInsertList.Count <= 0)
                return true;

            var mereTableMin = MereUtils.CacheCheck<TCopyTo>();

            var mereDataReader = new MereDataReader<TCopyTo>(toInsertList,  truncateLength);

            using (var conn = mereTableMin.GetConnection())
            {
                conn.Open();
                using (var cpy = new SqlBulkCopy(conn))
                {
                    cpy.EnableStreaming = true;
                    cpy.BulkCopyTimeout = 0;
                    cpy.BatchSize = batchSize;
                    foreach (var mereColumn in mereTableMin.SelectMereColumns)
                    {
                        var col = mereColumn.ColumnName;
                        cpy.ColumnMappings.Add(col, col);
                    }
                    cpy.DestinationTableName = mereTableMin.TableName;
                    //cpy.NotifyAfter = 1;
                    //cpy.SqlRowsCopied += (o, e) => Console.WriteLine(e.RowsCopied);
                    cpy.WriteToServer(mereDataReader);
                }
            }

            return true;
        }

        public async static Task<bool> MereBulkCopyAsync<T, TCopyTo>(IEnumerable<T> toCopyFrom, int batchSize = 1000, bool truncateLength = false)
            where T : new()
            where TCopyTo : new()
        {
            var toInsertList = toCopyFrom.MereCopyTo<T, TCopyTo>().ToList();
            if (toInsertList.Count <= 0)
                return true;

            var mereTableMin = MereUtils.CacheCheck<TCopyTo>();

            var mereDataReader = new MereDataReader<TCopyTo>(toInsertList, truncateLength);

            using (var conn = mereTableMin.GetConnection())
            {
                await conn.OpenAsync();
                using (var cpy = new SqlBulkCopy(conn))
                {
                    cpy.EnableStreaming = true;
                    cpy.BulkCopyTimeout = 0;
                    cpy.BatchSize = batchSize;
                    foreach (var mereColumn in mereTableMin.SelectMereColumns)
                    {
                        var col = mereColumn.ColumnName;
                        cpy.ColumnMappings.Add(col, col);
                    }
                    cpy.DestinationTableName = mereTableMin.TableName;
                    //cpy.NotifyAfter = 1;
                    //cpy.SqlRowsCopied += (o, e) => Console.WriteLine(e.RowsCopied);
                    await cpy.WriteToServerAsync(mereDataReader);
                }
            }

            return true;
        }
    }
}
