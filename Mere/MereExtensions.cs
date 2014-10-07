using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mere.Interfaces;

namespace Mere
{
    public static class MereExtensions
    {
        #region async
        public static async Task<int> ExecuteNonQueryAsync<T>(this T instance, string sql) where T : new()
        {
            var mereTable = MereUtils.CacheCheck<T>();
            int result;
            using (var myCn = mereTable.GetConnection())
            {
                var cmd = myCn.CreateCommand();

                cmd.CommandText = sql;
                await myCn.OpenAsync();
                result = await cmd.ExecuteNonQueryAsync();
                myCn.Close();
            }

            return result;
        }

        public static async Task<List<dynamic>> ExecuteQueryAsync<T>(this T instance, string sql) where T : new()
        {
            var mereTable = MereUtils.CacheCheck<T>();

            var toReturn = new List<dynamic>();

            using (var myCn = mereTable.GetConnection())
            {
                var cmd = myCn.CreateCommand();

                cmd.CommandText = sql;
                await myCn.OpenAsync();
                using (var reader = new MereSqlDataReader<T>(await cmd.ExecuteReaderAsync()))
                {
                    while (reader.Read())
                    {
                        toReturn.Add((dynamic)reader);
                    }
                    myCn.Close();
                }
            }
            
            return toReturn;
        }

        public static async Task<int> TruncateTableAsync<T>(this T instance) where T : new()
        {
            var mereTable = MereUtils.CacheCheck<T>();
            int result;
            using (var myCn = mereTable.GetConnection())
            {
                var cmd = myCn.CreateCommand();

                cmd.CommandText = string.Format("TRUNCATE TABLE {0}", mereTable.TableName);
                await myCn.OpenAsync();
                result = await cmd.ExecuteNonQueryAsync();
                myCn.Close();
            }

            return result;
        }
        #endregion

        #region sync
        public static int ExecuteNonQuery<T>(this T instance, string sql) where T : new()
        {
            var mereTable = MereUtils.CacheCheck<T>();
            int result;
            using (var myCn = mereTable.GetConnection())
            {
                var cmd = myCn.CreateCommand();

                cmd.CommandText = sql;
                myCn.Open();
                result = cmd.ExecuteNonQuery();
                myCn.Close();
            }

            return result;
        }

        public static IEnumerable<dynamic> ExecuteCustomDynamicQueryToDynamic<T>(this T instance, string sql) where T : new()
        {
            var mereTable = MereUtils.CacheCheck<T>();

            var toReturn = new List<dynamic>();

            using (var myCn = mereTable.GetConnection())
            {
                var cmd = myCn.CreateCommand();

                cmd.CommandText = sql;
                myCn.Open();
                using (var reader = new MereSqlDataReader<T>(cmd.ExecuteReader()))
                {
                    while (reader.Read())
                    {
                        toReturn.Add((dynamic)reader);
                    }
                    myCn.Close();
                }
            }

            return toReturn;
        }

        public static IEnumerable<T> ExecuteCustomQuery<T>(this T instance, string sql) where T : new()
        {
            var mereTable = MereUtils.CacheCheck<T>();

            var toReturn = new List<T>();

            using (var myCn = mereTable.GetConnection())
            {
                var cmd = myCn.CreateCommand();

                cmd.CommandText = sql;
                myCn.Open();
                using (var reader = new MereSqlDataReader<T>(cmd.ExecuteReader()))
                {
                    while (reader.Read())
                    {
                        toReturn.Add((T)reader);
                    }
                    myCn.Close();
                }
            }

            return toReturn;
        }

        public static int TruncateTable<T>(this T instance) where T : new()
        {
            var mereTable = MereUtils.CacheCheck<T>();
            int result;
            using (var myCn = mereTable.GetConnection())
            {
                var cmd = myCn.CreateCommand();

                cmd.CommandText = string.Format("TRUNCATE TABLE {0}", mereTable.TableName);
                myCn.Open();
                result = cmd.ExecuteNonQuery();
                myCn.Close();
            }

            return result;
        }
        #endregion
    }
}
