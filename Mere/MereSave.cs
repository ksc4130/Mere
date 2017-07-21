using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mere
{
    public class MereSave
    {
        #region sync

        public static bool Execute<T>(T src) where T : new()
        {
            return Execute(src, MereUtils.GetDataSource<T>());
        }

        public static bool Execute<T>(T src, MereDataSource mereDataSource) where T : new()
        {

            var mereTable = MereUtils.CacheCheck<T>();

            using (var myCn = MereUtils.GetConnection<T>(mereDataSource))
            {
                myCn.Open();
                using (var tx = myCn.BeginTransaction())
                {
                    using (var cmd = myCn.CreateCommand())
                    {
                        cmd.Transaction = tx;

                        foreach (var sel in mereTable.SelectMereColumnsNoIdentity().Select((prop, index) => new { index, prop }))
                        {
                            var v = sel.prop.GetBase(src);
                            if (v is DateTime && DateTime.Parse(v.ToString()).Year < 1753)
                                v = new DateTime(1753, 1, 1);

                            cmd.Parameters.AddWithValue("@" + sel.prop.ColumnName, v ?? DBNull.Value);
                        }

                        var key = mereTable.SelectMereColumns.FirstOrDefault(w => w.IsKey);

                        var identity = mereTable.SelectMereColumns.FirstOrDefault(w => w.IsIdentity);
                        if (key != null)
                        {
                            var val = key.GetBase(src);
                            cmd.Parameters.AddWithValue("@key", val);
                            cmd.CommandText = mereTable.GetUpsertSqlWithKey();

                            if (identity != null)
                            {
                                var inserted = cmd.ExecuteNonQuery();
                                cmd.CommandText = "SELECT @@IDENTITY";
                                var reader = cmd.ExecuteReader();
                                while (reader.Read())
                                {
                                    var id = reader[0];
                                    if (id != DBNull.Value)
                                        identity.SetBase(src, id);
                                }
                                reader.Close();
                                tx.Commit();

                                return inserted > 0;
                            }
                        }

                        //defaults to insert if no key available
                        cmd.CommandText = mereTable.SqlInsert;
                        var inserted1 = cmd.ExecuteNonQuery();
                        tx.Commit();
                        if (inserted1 <= 0)
                        {

                        }
                        return inserted1 > 0;

                    }
                }
            }
        }
        #endregion

        public static Task<bool> ExecuteAsync<T>(T src) where T : new()
        {
            return ExecuteAsync(src, MereUtils.GetDataSource<T>());
        }

        public static async Task<bool> ExecuteAsync<T>(T src, MereDataSource mereDataSource) where T : new()
        {
            var mereTable = MereUtils.CacheCheck<T>();

            using (var myCn = MereUtils.GetConnection<T>(mereDataSource))
            {
                await myCn.OpenAsync();
                using (var tx = myCn.BeginTransaction())
                {
                    using (var cmd = myCn.CreateCommand())
                    {
                        cmd.Transaction = tx;

                        foreach (var sel in mereTable.SelectMereColumnsNoIdentity().Select((prop, index) => new { index, prop }))
                        {
                            var v = sel.prop.GetBase(src);
                            if (v is DateTime && DateTime.Parse(v.ToString()).Year < 1753)
                                v = new DateTime(1753, 1, 1);

                            cmd.Parameters.AddWithValue("@" + sel.prop.ColumnName, v ?? DBNull.Value); // Changed .PropertyName to .ColumnName
                        }

                        var key = mereTable.SelectMereColumns.FirstOrDefault(w => w.IsKey);

                        var identity = mereTable.SelectMereColumns.FirstOrDefault(w => w.IsIdentity);
                        if (key != null)
                        {
                            var val = key.GetBase(src);
                            cmd.Parameters.AddWithValue("@key", val);
                            cmd.CommandText = mereTable.GetUpsertSqlWithKey();

                            if (identity != null)
                            {
                                var inserted = await cmd.ExecuteNonQueryAsync();
                                cmd.CommandText = "SELECT @@IDENTITY";
                                var reader = await cmd.ExecuteReaderAsync();
                                while (reader.Read())
                                {
                                    var id = reader[0];
                                    if (id != DBNull.Value)
                                        identity.SetBase(src, id);
                                }
                                reader.Close();
                                tx.Commit();

                                return inserted > 0;
                            }
                        }

                        //defaults to insert if no key available
                        cmd.CommandText = mereTable.SqlInsert;
                        var inserted1 = await cmd.ExecuteNonQueryAsync();
                        tx.Commit();
                        if (inserted1 <= 0)
                        {

                        }
                        return inserted1 > 0;

                    }
                }
            }
        }
    }
}
