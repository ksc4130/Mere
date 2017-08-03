using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mere
{
    public static class MereBulkExtensions
    {
        public static bool MereBulkInsert<T>(this IList<T> toInsertEn) where T : new()
        {
            return MereBulkInsert((IEnumerable<T>) toInsertEn, 1000, false);
        }

        public static bool MereBulkInsert<T>(this IList<T> toInsertEn, int batchSize) where T : new()
        {
            return MereBulkInsert((IEnumerable<T>)toInsertEn, batchSize, false);
        }

        public static bool MereBulkInsert<T>(this IEnumerable<T> toInsertEn) where T : new()
        {
            return MereBulkInsert(toInsertEn, 1000, false);
        }


        public static bool MereBulkInsert<T>(this IEnumerable<T> toInsertEn, int batchSize, bool truncateLength) where T : new()
        {
            var toInsertList = toInsertEn.ToList();
            if (toInsertList.Count <= 0)
                return true;

            var mereTableMin = MereUtils.CacheCheck<T>();
            
            var mereDataReader = new MereDataReader<T>(toInsertList, truncateLength);

            using (var conn = MereUtils.GetConnection<T>())
            {
                conn.Open();
                using (var cpy = new SqlBulkCopy(conn))
                {
                    cpy.EnableStreaming = true;
                    cpy.BulkCopyTimeout = 0;
                    cpy.BatchSize = batchSize;

                    mereTableMin.SelectMereColumns.ForEach(
                        x => cpy.ColumnMappings.Add(x.ColumnName, x.ColumnName));

                    cpy.DestinationTableName = mereTableMin.TableName;
                    //cpy.NotifyAfter = 1;
                    //cpy.SqlRowsCopied += (o, e) => Console.WriteLine(e.RowsCopied);

                    cpy.WriteToServer(mereDataReader);
                }
            }

            return true;
        }

        public static Task<bool> MereBulkInsertAsync<T>(this IList<T> toInsertEn) where T : new()
        {
            return MereBulkInsertAsync((IEnumerable<T>)toInsertEn, 1000, false);
        }

        public static Task<bool> MereBulkInsertAsync<T>(this IList<T> toInsertEn, int batchSize) where T : new()
        {
            return MereBulkInsertAsync((IEnumerable<T>)toInsertEn, batchSize, false);
        }

        public static Task<bool> MereBulkInsertAsync<T>(this IEnumerable<T> toInsertEn) where T : new()
        {
            return MereBulkInsertAsync(toInsertEn, 1000, false);
        }

        public static Task<bool> MereBulkInsertAsync<T>(this IList<T> toInsertEn, int batchSize, MereDataSource mereDataSource) where T : new()
        {
            return toInsertEn.MereBulkInsertAsync<T>(batchSize, false, mereDataSource);
        }

        public async static Task<bool> MereBulkInsertAsync<T>(this IEnumerable<T> toInsertEn, int batchSize, bool truncateLength) where T : new()
        {
            var toInsertList = toInsertEn.ToList();
            if (toInsertList.Count <= 0)
                return true;

            var mereTableMin = MereUtils.CacheCheck<T>();

            var mereDataReader = new MereDataReader<T>(toInsertList, truncateLength);

            using (var conn = MereUtils.GetConnection<T>())
            {
                await conn.OpenAsync();
                using (var cpy = new SqlBulkCopy(conn))
                {
                    cpy.EnableStreaming = true;
                    cpy.BulkCopyTimeout = 0;
                    cpy.BatchSize = batchSize;

                    mereTableMin.SelectMereColumns.ForEach(
                        x => cpy.ColumnMappings.Add(x.ColumnName, x.ColumnName));

                    cpy.DestinationTableName = mereTableMin.TableName;
                    //cpy.NotifyAfter = 1;
                    //cpy.SqlRowsCopied += (o, e) => Console.WriteLine(e.RowsCopied);

                    await cpy.WriteToServerAsync(mereDataReader);
                }
            }

            return true;
        }

        public async static Task<bool> MereBulkInsertAsync<T>(this IEnumerable<T> toInsertEn, int batchSize, bool truncateLength, MereDataSource mereDataSource) where T : new()
        {
            var toInsertList = toInsertEn.ToList();
            if (toInsertList.Count <= 0)
                return true;

            var mereTableMin = MereUtils.CacheCheck<T>();
            var mereDataReader = new MereDataReader<T>(toInsertList, truncateLength);
            SqlConnection conn = mereDataSource == null ? MereUtils.GetConnection<T>() : MereUtils.GetConnection(mereDataSource);

            using (conn)
            {
                await conn.OpenAsync();
                using (var cpy = new SqlBulkCopy(conn))
                {
                    cpy.EnableStreaming = true;
                    cpy.BulkCopyTimeout = 0;
                    cpy.BatchSize = batchSize;

                    mereTableMin.SelectMereColumns.ForEach(
                        x => cpy.ColumnMappings.Add(x.ColumnName, x.ColumnName));

                    cpy.DestinationTableName = mereTableMin.TableName;

                    await cpy.WriteToServerAsync(mereDataReader);
                }
            }

            return true;
        }

        public static bool MereBulkCopy<T, TCopyTo>(this IEnumerable<T> toCopyFrom, int batchSize = 1000, bool truncateLength = false) where T : new() where TCopyTo : new()
        {
            var toInsertList = toCopyFrom.MereCopyTo<T, TCopyTo>().ToList();
            if (toInsertList.Count <= 0)
                return true;

            var mereTableMin = MereUtils.CacheCheck<TCopyTo>();

            var mereDataReader = new MereDataReader<TCopyTo>(toInsertList, truncateLength);

            using (var conn = MereUtils.GetConnection<T>())
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

        public async static Task<bool> MereBulkCopyAsync<T, TCopyTo>(this IEnumerable<T> toCopyFrom, int batchSize = 1000, bool truncateLength = false) where T : new() where TCopyTo : new()
        {
            var toInsertList = toCopyFrom.MereCopyTo<T, TCopyTo>().ToList();
            if (toInsertList.Count <= 0)
                return true;

            var mereTableMin = MereUtils.CacheCheck<TCopyTo>();

            var mereDataReader = new MereDataReader<TCopyTo>(toInsertList, truncateLength);

            using (var conn = MereUtils.GetConnection<T>())
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

        public static bool MereBulkCopy<T>(this IEnumerable<T> toCopyFrom ,MereDataSource copyToMereDataSource, int batchSize = 1000, bool truncateLength = false)
            where T : new()
        {
            var toInsertList = toCopyFrom.ToList();
            if (toInsertList.Count <= 0)
                return true;

            var mereTableMin = MereUtils.CacheCheck<T>();

            var mereDataReader = new MereDataReader<T>(toInsertList, truncateLength);

            using (var conn = MereUtils.GetConnection(copyToMereDataSource))
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
                    cpy.DestinationTableName = copyToMereDataSource.TableName ?? mereTableMin.TableName;
                    //cpy.NotifyAfter = 1;
                    //cpy.SqlRowsCopied += (o, e) => Console.WriteLine(e.RowsCopied);
                    cpy.WriteToServer(mereDataReader);
                }
            }

            return true;
        }

        public async static Task<bool> MereBulkCopyAsync<T>(this IEnumerable<T> toCopyFrom, MereDataSource copyToMereDataSource, int batchSize = 1000, bool truncateLength = false)
            where T : new()
        {
            var toInsertList = toCopyFrom.ToList();
            if (toInsertList.Count <= 0)
                return true;

            var mereTableMin = MereUtils.CacheCheck<T>();

            var mereDataReader = new MereDataReader<T>(toInsertList, truncateLength);

            using (var conn = MereUtils.GetConnection(copyToMereDataSource))
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
                    cpy.DestinationTableName = copyToMereDataSource.TableName ?? mereTableMin.TableName;
                    //cpy.NotifyAfter = 1;
                    //cpy.SqlRowsCopied += (o, e) => Console.WriteLine(e.RowsCopied);
                    await cpy.WriteToServerAsync(mereDataReader);
                }
            }

            return true;
        }

        public static int MereStraightUpBulkCopy<T, TDest>(bool includeIdentities = false) where T : new() where TDest : new()
        {
            var mereTableMin = MereUtils.CacheCheck<T>();
            var mereTableMinTo = MereUtils.CacheCheck<TDest>();
            var copied = 0;
            using (var conn = mereTableMin.GetConnection())
            {
                var cmd = new SqlCommand { CommandTimeout = 0, Connection = conn };

                var selProps = includeIdentities
                                   ? mereTableMin.SqlColumnNamesAll
                                   : mereTableMin.SqlColumnNamesNoIdentity;
                var sql = "INSERT INTO " + mereTableMinTo.DatabaseName + ".dbo." + mereTableMinTo.TableName + " SELECT " + selProps + " FROM " + mereTableMin.DatabaseName + ".dbo." + mereTableMin.TableName + " ";
                cmd.CommandText = sql;
                conn.Open();
                copied = cmd.ExecuteNonQuery();
            }

            return copied;
        }

        public async static Task<int> MereStraightUpBulkCopyAsync<T, TDest>(bool includeIdentities = false) where T : new() where TDest : new()
        {
            var mereTableMin = MereUtils.CacheCheck<T>();
            var mereTableMinTo = MereUtils.CacheCheck<TDest>();
            var copied = 0;
            using (var conn = mereTableMin.GetConnection())
            {
                var cmd = new SqlCommand { CommandTimeout = 0, Connection = conn };

                var selProps = includeIdentities
                                   ? mereTableMin.SqlColumnNamesAll
                                   : mereTableMin.SqlColumnNamesNoIdentity;
                var sql = "INSERT INTO " + mereTableMinTo.DatabaseName + ".dbo." + mereTableMinTo.TableName + " SELECT " + selProps + " FROM " + mereTableMin.DatabaseName + ".dbo." + mereTableMin.TableName + " ";
                cmd.CommandText = sql;
                await conn.OpenAsync();
                copied = await cmd.ExecuteNonQueryAsync();
            }

            return copied;
        }

        //=========================================================================================
        public static bool MereBulkUpdate<T>(
            this IList<T> toUpdateEn, IEnumerable<String> keyFields,
            IEnumerable<String> setFields
            ) where T : new()
        {
            return MereBulkUpdate((IEnumerable<T>)toUpdateEn, keyFields, setFields, 1000, false);
        }

        //=========================================================================================
        public static bool MereBulkUpdate<T>(
            this IList<T> toUpdateEn, IEnumerable<String> keyFields,
            IEnumerable<String> setFields, int batchSize
            ) where T : new()
        {
            return MereBulkUpdate((IEnumerable<T>)toUpdateEn, keyFields, setFields, batchSize, false);
        }


        //=========================================================================================
        public static bool MereBulkUpdate<T>(
            this IEnumerable<T> toUpdateEn, IEnumerable<String> keyFields,
            IEnumerable<String> setFields
            ) where T : new()
        {
            return MereBulkUpdate(toUpdateEn, keyFields, setFields, 1000, false);
        }

        //=========================================================================================
        public static bool MereBulkUpdate<T>(
            this IEnumerable<T> toUpdateEn, IEnumerable<String> keyFields,
            IEnumerable<String> setFields, int batchSize, bool truncateLength
            ) where T : new()
        {
            var toUpdateEnList = toUpdateEn.ToList();
            if (toUpdateEnList.Count <= 0)
            { return true; }

            var setFieldList = setFields.ToList();
            if (setFieldList.Count <= 0)
            { return true; }

            var keyFieldsList = keyFields.ToList();
            if (keyFieldsList.Count <= 0)
            { return true; }

            var mereTableMin = MereUtils.CacheCheck<T>();

            var mereDataReader = new MereDataReader<T>(toUpdateEnList, truncateLength);

            using (var conn = MereUtils.GetConnection<T>())
            {
                conn.Open();
                using (var cpy = new SqlBulkCopy(conn))
                {
                    cpy.EnableStreaming = true;
                    cpy.BulkCopyTimeout = 0;
                    cpy.BatchSize = batchSize;

                    //-----------------------------------------------------------
                    var tempSetSb = new StringBuilder();
                    var identityColumns = new List<String>();

                    foreach (var column in mereTableMin.SelectMereColumns)
                    {
                        if (setFieldList.Contains(column.ColumnName) ||
                            keyFieldsList.Contains(column.ColumnName))
                        {
                            if (column.IsIdentity)
                            { identityColumns.Add("[" + column.ColumnName + "]"); }

                            cpy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                            tempSetSb.Append("[" + column.ColumnName + "],");
                        }
                    }

                    if (tempSetSb.Length > 0)
                    { tempSetSb.Length--; }

                    //-----------------------------------------------------------
                    var tempTable = "##" + mereTableMin.TableName + "_" +
                        Guid.NewGuid().ToString().Replace("-", "_");

                    var command = new SqlCommand("", conn);

                    command.CommandText = String.Format(
                        "Select top 0 {1} Into {2} from {3}{0}",
                        "\r\n",
                        tempSetSb,
                        tempTable,
                        mereTableMin.TableName);
                    command.ExecuteNonQuery();

                    foreach (var identity in identityColumns)
                    {
                        command.CommandText = String.Format(
                            "Alter Table {1}{0}Drop Column {2}{0}",
                            "\r\n",
                            tempTable,
                            identity);
                        command.ExecuteNonQuery();

                        command.CommandText = String.Format(
                            "Alter Table {1}{0}Add {2} int{0}",
                            "\r\n",
                            tempTable,
                            identity);
                        command.ExecuteNonQuery();
                    }

                    cpy.DestinationTableName = tempTable;
                    cpy.WriteToServer(mereDataReader);

                    //-----------------------------------------------------------
                    var setSb = new StringBuilder();
                    setFieldList.ForEach(x =>
                        setSb.Append(String.Format(
                            "t1.{1} = t2.{1},{0}      ",
                            "\r\n", x)));

                    if (setSb.Length > 9)
                    { setSb.Length = setSb.Length - 9; }

                    var keySb = new StringBuilder();
                    keyFieldsList.ForEach(x =>
                        keySb.Append(String.Format(
                            "t1.{1} = t2.{1}{0}  AND ",
                            "\r\n", x)));

                    if (keySb.Length > 8)
                    { keySb.Length = keySb.Length - 8; }

                    //----------------------------------------------------------
                    command.CommandText = String.Format(
                        "Update t1{0}" +
                        "  Set {3}{0}" +
                        " From {1} t1{0}" +
                        "Inner Join {2} t2{0}" +
                        "   On {4}",
                        "\r\n",
                        mereTableMin.TableName,
                        tempTable, setSb, keySb);
                    command.ExecuteNonQuery();
                }
            }

            return true;
        }
    }
}
