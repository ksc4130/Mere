using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Dynamic;
using System.Threading.Tasks;

namespace Mere.Sql
{
    public static class MereSqlContextRunQueryExt
    {
        public static IEnumerable<ExpandoObject> RunQueryExpando<T>(this MereSqlContext<T> src, string sql, List<SqlParameter> parameters) where T : new()
        {
            if (src == null) throw new ArgumentNullException(nameof(src));
            using (var cmd = parameters != null ? src.GetCommand(parameters) : src.GetCommand(false))
            {
                try
                {
                    cmd.CommandText = sql ?? src.Sql;
                    cmd.CommandTimeout = src.Timeout;

                    cmd.Connection.Open();
                    using (var reader = new MereSqlDataReader<T>(cmd.ExecuteReader()))
                    {

                        while (reader.Read())
                        {
                            yield return (ExpandoObject)reader;
                        }

                    }
                }
                finally
                {
                    cmd.Connection.Close();
                    cmd.Connection.Dispose();
                    cmd.Parameters.Clear();
                }


            }
        }

        public static List<ExpandoObject> RunQueryExpandoList<T>(this MereSqlContext<T> src, string sql, List<SqlParameter> parameters) where T : new()
        {
            var toReturn = new List<ExpandoObject>();
            using (var cmd = parameters != null ? src.GetCommand(parameters) : src.GetCommand(false))
            {
                try
                {
                    cmd.CommandText = sql ?? src.Sql;
                    cmd.CommandTimeout = src.Timeout;

                    cmd.Connection.Open();
                    using (var reader = new MereSqlDataReader<T>(cmd.ExecuteReader()))
                    {

                        while (reader.Read())
                        {
                            toReturn.Add(reader);
                        }

                    }
                }
                finally
                {
                    cmd.Connection.Close();
                    cmd.Connection.Dispose();
                    cmd.Parameters.Clear();
                }

            }
            return toReturn;
        }

        public static async Task<List<ExpandoObject>> RunQueryExpandoAsync<T>(this MereSqlContext<T> src, string sql, List<SqlParameter> parameters) where T : new()
        {
            var toReturn = new List<ExpandoObject>();
            using (var cmd = parameters != null ? src.GetCommand(parameters) : src.GetCommand(false))
            {
                try
                {
                    cmd.CommandText = sql ?? src.Sql;
                    cmd.CommandTimeout = src.Timeout;

                    await cmd.Connection.OpenAsync();
                    using (var reader = new MereSqlDataReader<T>(await cmd.ExecuteReaderAsync()))
                    {

                        while (await reader.ReadAsync())
                        {
                            toReturn.Add(reader);
                        }

                    }
                }
                finally
                {
                    cmd.Connection.Close();
                    cmd.Connection.Dispose();
                    cmd.Parameters.Clear();
                }

            }
            return toReturn;
        }

        public static IEnumerable<dynamic> RunQueryDynamic<T>(this MereSqlContext<T> src, string sql, List<SqlParameter> parameters) where T : new()
        {
            using (var cmd = parameters != null ? src.GetCommand(parameters) : src.GetCommand(false))
            {
                try
                {
                    cmd.CommandText = sql ?? src.Sql;
                    cmd.CommandTimeout = src.Timeout;

                    cmd.Connection.Open();
                    using (var reader = new MereSqlDataReader<T>(cmd.ExecuteReader()))
                    {

                        while (reader.Read())
                        {
                            yield return (ExpandoObject)reader;
                        }

                    }
                }
                finally
                {
                    cmd.Connection.Close();
                    cmd.Connection.Dispose();
                    cmd.Parameters.Clear();
                }

            }
        }

        public static List<dynamic> RunQueryDynamicList<T>(this MereSqlContext<T> src, string sql, List<SqlParameter> parameters) where T : new()
        {
            var toReturn = new List<dynamic>();
            using (var cmd = parameters != null ? src.GetCommand(parameters) : src.GetCommand(false))
            {
                try
                {
                    cmd.CommandText = sql ?? src.Sql;
                    cmd.CommandTimeout = src.Timeout;

                    cmd.Connection.Open();
                    using (var reader = new MereSqlDataReader<T>(cmd.ExecuteReader()))
                    {

                        while (reader.Read())
                        {
                            toReturn.Add((ExpandoObject)reader);
                        }

                    }
                }
                finally
                {
                    cmd.Connection.Close();
                    cmd.Connection.Dispose();
                    cmd.Parameters.Clear();
                }

            }
            return toReturn;
        }

        public static async Task<List<dynamic>> RunQueryDynamicAsync<T>(this MereSqlContext<T> src, string sql, List<SqlParameter> parameters) where T : new()
        {
            var toReturn = new List<dynamic>();
            using (var cmd = parameters != null ? src.GetCommand(parameters) : src.GetCommand(false))
            {
                try
                {
                    cmd.CommandText = sql ?? src.Sql;
                    cmd.CommandTimeout = src.Timeout;

                    await cmd.Connection.OpenAsync();
                    using (var reader = new MereSqlDataReader<T>(await cmd.ExecuteReaderAsync()))
                    {

                        while (await reader.ReadAsync())
                        {
                            toReturn.Add((ExpandoObject)reader);
                        }

                    }
                }
                finally
                {
                    cmd.Connection.Close();
                    cmd.Connection.Dispose();
                    cmd.Parameters.Clear();
                }

            }
            return toReturn;
        }

        public static IEnumerable<T> RunQuery<T>(this MereSqlContext<T> src, string sql, List<SqlParameter> parameters) where T : new()
        {
            using (var cmd = parameters != null ? src.GetCommand(parameters) : src.GetCommand(false))
            {
                try
                {
                    cmd.CommandText = sql ?? src.Sql;
                    cmd.CommandTimeout = src.Timeout;

                    cmd.Connection.Open();
                    using (var reader = new MereSqlDataReader<T>(cmd.ExecuteReader(), src.SelectMereColumnsList, true))
                    {

                        while (reader.Read())
                        {
                            yield return reader;
                        }

                    }
                }
                finally
                {
                    cmd.Connection.Close();
                    cmd.Connection.Dispose();
                    cmd.Parameters.Clear();
                }

            }
        }
        public static List<T> RunQueryList<T>(this MereSqlContext<T> src, string sql, List<SqlParameter> parameters) where T : new()
        {
            var toReturn = new List<T>();
            using (var cmd = parameters != null ? src.GetCommand(parameters) : src.GetCommand(false))
            {
                try
                {
                    cmd.CommandText = sql ?? src.Sql;
                    cmd.CommandTimeout = src.Timeout;

                    cmd.Connection.Open();
                    using (var reader = new MereSqlDataReader<T>(cmd.ExecuteReader(), src.SelectMereColumnsList, true))
                    {

                        while (reader.Read())
                        {
                            toReturn.Add(reader);
                        }

                    }
                }
                finally
                {
                    cmd.Connection.Close();
                    cmd.Connection.Dispose();
                    cmd.Parameters.Clear();
                }

            }
            return toReturn;
        }

        public static async Task<List<T>> RunQueryAsync<T>(this MereSqlContext<T> src, string sql, List<SqlParameter> parameters) where T : new()
        {
            var toReturn = new List<T>();
            using (var cmd = parameters != null ? src.GetCommand(parameters) : src.GetCommand(false))
            {
                try
                {
                    cmd.CommandText = sql ?? src.Sql;
                    cmd.CommandTimeout = src.Timeout;

                    await cmd.Connection.OpenAsync();
                    using (var reader = new MereSqlDataReader<T>(await cmd.ExecuteReaderAsync(), src.SelectMereColumnsList, true))
                    {

                        while (await reader.ReadAsync())
                        {
                            toReturn.Add(reader);
                        }

                    }
                }
                finally
                {
                    cmd.Connection.Close();
                    cmd.Connection.Dispose();
                    cmd.Parameters.Clear();
                }

            }
            return toReturn;
        }

        public static IEnumerable<T> RunQuery<T>(this MereSqlContext<T> src) where T : new()
        {
            return src.RunQuery(src.Sql, src.Parameters);
        }
        public static List<T> RunQueryList<T>(this MereSqlContext<T> src) where T : new()
        {
            return src.RunQueryList(src.Sql, src.Parameters);
        }

        public static Task<List<T>> RunQueryAsync<T>(this MereSqlContext<T> src) where T : new()
        {
            return src.RunQueryAsync(src.Sql, src.Parameters);
        }
    }
}