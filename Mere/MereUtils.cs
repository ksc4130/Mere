using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mail;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Mere.Attributes;

namespace Mere
{
    public partial class MereUtils
    {
        #region sql

        #region async

        public static async Task<int> ExecuteNonQueryAsync<T>(string sql)
            where T : new()
        {
            var mereTable = CacheCheck<T>();
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

        public static Task<List<dynamic>> ExecuteQueryAsync<T>(string sql)
             where T : new()
        {
            return MereQuery.Create<T>().ExecuteCustomQueryDynamicAsync(sql);
        }

        public static Task<List<dynamic>> ExecuteQueryAsync<T>(string sql, object paramsObj)
            where T : new()
        {
            return MereQuery.Create<T>().ExecuteCustomQueryDynamicAsync(sql, paramsObj);
        }

        public static Task<int> StraightUpBulkCopyAsync<T, TDest>()
            where T : new()
            where TDest : new()
        {
            return StraightUpBulkCopyAsync<T, TDest>(false);
        }

        public static Task<int> StraightUpBulkCopyAsync<T, TDest>(bool includeIdentities)
            where T : new()
            where TDest : new()
        {
            var mereTable = CacheCheck<T>();
            var mereTableTo = CacheCheck<TDest>();

            var selProps = includeIdentities
                  ? mereTable.SqlColumnNamesAll
                  : mereTable.SqlColumnNamesNoIdentity;

            var sql =
                "INSERT INTO " + mereTableTo.DatabaseName + ".dbo." + mereTableTo.TableName +
                " SELECT " + selProps +
                " FROM " + mereTable.DatabaseName + ".dbo." + mereTable.TableName + " ";
            return ExecuteNonQueryAsync<T>(sql);
        }

        public static async Task<int> TruncateTableAsync<T>()
            where T : new()
        {
            var mereTable = CacheCheck<T>();
            int result;
            using (var myCn = GetConnection<T>())
            {
                var cmd = myCn.CreateCommand();

                cmd.CommandText = string.Format("TRUNCATE TABLE {0}", mereTable.TableName);
                await myCn.OpenAsync();
                result = await cmd.ExecuteNonQueryAsync();
                myCn.Close();
            }

            return result;
        }

        public static async Task<int> TruncateTableAsync<T>(MereDataSource mereDataSource)
            where T : new()
        {
            var mereTable = CacheCheck<T>();
            int result;
            using (var myCn = GetConnection<T>(mereDataSource))
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

        public static int ExecuteNonQuery<T>(string sql)
            where T : new()
        {
            var mereTable = CacheCheck<T>();
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

        public static IEnumerable<dynamic> ExecuteQuery<T>(string sql)
            where T : new()
        {
            return MereQuery.Create<T>().ExecuteCustomQueryDynamic(sql);
        }

        public static IEnumerable<dynamic> ExecuteQuery<T>(string sql, object paramsObj)
            where T : new()
        {
            return MereQuery.Create<T>().ExecuteCustomQueryDynamic(sql, paramsObj);
        }

        public static int StraightUpBulkCopy<T, TDest>()
            where T : new()
            where TDest : new()
        {
            return StraightUpBulkCopy<T, TDest>(false);
        }

        public static int StraightUpBulkCopy<T, TDest>(bool includeIdentities)
            where T : new()
            where TDest : new()
        {
            var mereTable = CacheCheck<T>();
            var mereTableTo = CacheCheck<TDest>();

            var selProps = includeIdentities
                ? mereTable.SqlColumnNamesAll
                : mereTable.SqlColumnNamesNoIdentity;

            var sql =
                "INSERT INTO " + mereTableTo.DatabaseName + ".dbo." + mereTableTo.TableName +
                " SELECT " + selProps +
                " FROM " + mereTable.DatabaseName + ".dbo." + mereTable.TableName + " ";
            return ExecuteNonQuery<T>(sql);
        }

        public static int TruncateTable<T>()
            where T : new()
        {
            var mereTable = CacheCheck<T>();
            int result;
            using (var myCn = GetConnection<T>())
            {
                var cmd = myCn.CreateCommand();

                cmd.CommandText = string.Format("TRUNCATE TABLE {0}", mereTable.TableName);
                myCn.Open();
                result = cmd.ExecuteNonQuery();
                myCn.Close();
            }

            return result;
        }

        public static int TruncateTable<T>(MereDataSource mereDataSource)
            where T : new()
        {
            var mereTable = CacheCheck<T>();
            int result;
            using (var myCn = GetConnection<T>(mereDataSource))
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

        #endregion

        #region mere helpers

        private static string _connectionStringBase =
            "Data Source={0};Initial Catalog={1};User ID={2};Password={3};" +
            "Asynchronous Processing=True;MultipleActiveResultSets=True;Timeout={4}";

        public static string ConnectionStringBase
        {
            get { return _connectionStringBase; }
            set { _connectionStringBase = value; }
        }

        public static string GetConnectionString(
            string serverName, string databaseName,
            string userId, string password,
            int timeout)
        {
            return string.Format(
                ConnectionStringBase, serverName, databaseName,
                userId, password, timeout);
        }

        public static string GetConnectionString<T>()
            where T : new()
        {
            var ds = GetDataSource<T>();
            return string.Format(
                ConnectionStringBase,
                ds.ServerName,
                ds.DatabaseName,
                ds.UserId,
                ds.Password,
                0);
        }

        public static string GetConnectionString(MereDataSource mds)
        {
            return string.Format(
                ConnectionStringBase,
                mds.ServerName,
                mds.DatabaseName,
                mds.UserId,
                mds.Password,
                0);
        }

        public static string GetConnectionString(MereDataSource mds, int timeout)
        {
            return string.Format(
                ConnectionStringBase,
                mds.ServerName,
                mds.DatabaseName,
                mds.UserId,
                mds.Password,
                timeout);
        }

        public static string GetConnectionString<T>(int timeout)
            where T : new()
        {
            var ds = GetDataSource<T>();
            return string.Format(
                ConnectionStringBase,
                ds[_mereEnvironment].ServerName,
                ds[_mereEnvironment].DatabaseName,
                ds[_mereEnvironment].UserId,
                ds[_mereEnvironment].Password,
                timeout);
        }

        public static SqlConnection GetConnection(
            string serverName, string databaseName,
            string userId, string password,
            int timeout)
        {
            return new SqlConnection(GetConnectionString(
                serverName, databaseName, userId, password, timeout));
        }

        public static SqlConnection GetConnection(MereDataSource mereDataSource)
        {
            var d = mereDataSource[MereEnvironment];
            return new SqlConnection(GetConnectionString(
                d.ServerName, d.DatabaseName, d.UserId, d.Password, 0));
        }

        public static SqlConnection GetConnection<T>(MereDataSource mereDataSource) 
            where T : new()
        {
            mereDataSource = mereDataSource ?? MereDataSource.Create<T>();
            var d = mereDataSource[MereEnvironment];
            return new SqlConnection(GetConnectionString(
                d.ServerName, d.DatabaseName, d.UserId, d.Password, 0));
        }

        public static SqlConnection GetConnection<T>()
            where T : new()
        {
            return new SqlConnection(GetConnectionString<T>());
        }

        public static SqlConnection GetConnection<T>(int timeout)
            where T : new()
        {
            return new SqlConnection(GetConnectionString<T>(timeout));
        }

        public static MereDataSource GetDataSource<T>()
            where T : new()
        {
            return GlobalDataSource != null
                ? GlobalDataSource[MereEnvironment]
                : MereDataSource.Create<T>();
        }

        public static MereDataSource GetDataSource(
            string serverName, string databaseName, string userId, string password)
        {
            return GlobalDataSource != null
                ? GlobalDataSource[MereEnvironment]
                : MereDataSource.Create(serverName, databaseName, userId, password);
        }


        public static MereDataSource GlobalDataSource = null;
        public static Func<string> GetEnvironment = null;

        public static void SetEnvironmentGetter(Func<string> getter)
        {
            GetEnvironment = getter;
        }

        private static readonly Queue<int> MereEnvironmentSubscriptionPreusedIds = new Queue<int>();
        private static int _mereEnvironmentSubscriptionIds;
        private static ConcurrentDictionary<int, Action<string>> _mereEnvironmentSubscriptions;

        public static int OnMereEnvironmentChanged(Action<string> callback)
        {
            if (_mereEnvironmentSubscriptions == null)
                _mereEnvironmentSubscriptions = new ConcurrentDictionary<int, Action<string>>();

            if (!_mereEnvironmentSubscriptions.Values.Contains(callback))
            {
                var id = GetGoodSubKey();

                _mereEnvironmentSubscriptions.TryAdd(id, callback);
                return id;
            }

            return 0;
        }

        private static int GetGoodSubKey()
        {
            int id;
            if (MereEnvironmentSubscriptionPreusedIds != null && MereEnvironmentSubscriptionPreusedIds.Any())
                id = MereEnvironmentSubscriptionPreusedIds.Dequeue();
            else
            {
                _mereEnvironmentSubscriptionIds++;
                id = _mereEnvironmentSubscriptionIds;
            }

            if (_mereEnvironmentSubscriptions.ContainsKey(id))
                return GetGoodSubKey();

            return id;
        }

        public static void OffMereEnvironmentChanged(Action<string> callback)
        {
            if (_mereEnvironmentSubscriptions == null)
                return;

            if (_mereEnvironmentSubscriptions.All(x => x.Value == callback))
                return;

            var sub = _mereEnvironmentSubscriptions.FirstOrDefault(x => x.Value == callback);
            Action<string> check;
            _mereEnvironmentSubscriptions.TryRemove(sub.Key, out check);
            MereEnvironmentSubscriptionPreusedIds.Enqueue(sub.Key);

        }

        public static void OffMereEnvironmentChanged(int id)
        {
            if (_mereEnvironmentSubscriptions == null || id == 0)
                return;

            if (!_mereEnvironmentSubscriptions.ContainsKey(id))
                return;
            Action<string> check;
            _mereEnvironmentSubscriptions.TryRemove(id, out check);
            MereEnvironmentSubscriptionPreusedIds.Enqueue(id);

        }

        private static string _mereEnvironment;

        public static string MereEnvironment
        {
            get
            {
                if (GetEnvironment == null)
                    return null;

                var envir = GetEnvironment();
                if (_mereEnvironment != envir)
                {
                    _mereEnvironment = envir;
                    if (_mereEnvironmentSubscriptions != null)
                    {
                        foreach (var sub in _mereEnvironmentSubscriptions.Values)
                        {
                            sub(_mereEnvironment);
                        }
                    }
                }
                Debug.WriteLine("Mere environment getter " + _mereEnvironment);
                return _mereEnvironment;
            }
        }

        /// <summary>
        /// MereTable object cache
        /// </summary>
        public static ConcurrentDictionary<RuntimeTypeHandle, MereTableMin> Cache =
            new ConcurrentDictionary<RuntimeTypeHandle, MereTableMin>();

        /// <summary>
        /// Flag to try to clean convertions when false just parses and throws any errors that occur
        /// </summary>
        public static bool CleanupConversion = true;

        /// <summary>
        /// Check cache for MereTable object for T if one does not exist then creates, adds to cache, and returns it.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static MereTableMin<T> CacheCheck<T>()
            where T : new()
        {
            var type = typeof(T);
            MereTableMin foundMereTableMin;
            if (Cache.TryGetValue(type.TypeHandle, out foundMereTableMin))
                return (MereTableMin<T>)foundMereTableMin;
            var properties = TypeDescriptor.GetProperties(type).PropertyDescriptorCollectionToList();

            //for columns 
            var hasColumns = properties.Any(
                a => type.GetProperty(a.Name).GetCustomAttributes(typeof(MereColumnAttribute), true).Any());

            var hasKeys = properties.Any(
                a => type.GetProperty(a.Name).GetCustomAttributes(typeof(MereKeyAttribute), true).Any());

            //get fields with column or key attrs or if there are no props with column attr assume 
            //all to be columns and column name is the same as public prop name
            var columnAndKeys = properties.Select(
                s => new
                {
                    KeyAttribute =
                (MereKeyAttribute)
                type.GetProperty(s.Name)
                    .GetCustomAttributes(typeof(MereKeyAttribute), true)
                    .FirstOrDefault(),
                    ColumnAttribute =
                (MereColumnAttribute)
                type.GetProperty(s.Name)
                    .GetCustomAttributes(typeof(MereColumnAttribute), true)
                    .FirstOrDefault(),
                    DisplayNameAttribute =
                (MereDisplayNameAttribute)
                type.GetProperty(s.Name)
                    .GetCustomAttributes(typeof(MereDisplayNameAttribute), true)
                    .FirstOrDefault(),
                    IdentityAttribute =
                (MereIdentityAttribute)
                type.GetProperty(s.Name)
                    .GetCustomAttributes(typeof(MereIdentityAttribute), true)
                    .FirstOrDefault(),
                    NullableAttribute =
                (MereNullableAttribute)
                type.GetProperty(s.Name)
                    .GetCustomAttributes(typeof(MereNullableAttribute), true)
                    .FirstOrDefault(),
                    MaxLengthAttribute =
                (MereMaxLengthAttribute)
                type.GetProperty(s.Name)
                    .GetCustomAttributes(typeof(MereMaxLengthAttribute), true)
                    .FirstOrDefault(),
                    PropertyDescriptor = s,
                    IsColumn = (
                        (type.GetProperty(s.Name).GetCustomAttributes(typeof(MereColumnAttribute), true).Any()
                        || type.GetProperty(s.Name).GetCustomAttributes(typeof(MereKeyAttribute), true).Any())
                        || !hasColumns
                    )
                    && !type.GetProperty(s.Name).GetCustomAttributes(typeof(MereIgnoreAttribute), true).Any()
                }).ToList();

            //var columnNames = new List<string>();
            string keyColumnName = null;
            string keyPropertyName = null;

            var mereColumns = new MereColumnList();
            var allMereColumns = new MereColumnList();

            var intIndex = 0;
            foreach (var column in columnAndKeys)
            {
                var mereColumn =
                    new MereColumn
                    {
                        IsColumn = column.IsColumn,
                        PropertyDescriptor = column.PropertyDescriptor,
                        PropertyName = column.PropertyDescriptor.Name,
                        ColumnName =
                            column.ColumnAttribute != null
                                ? column.ColumnAttribute.CompiledName ?? column.PropertyDescriptor.Name
                                : column.PropertyDescriptor.Name,
                        DisplayName = column.DisplayNameAttribute != null
                                ? column.DisplayNameAttribute.DisplayName
                                : column.ColumnAttribute != null
                                    ? column.ColumnAttribute.CompiledName
                                    : column.PropertyDescriptor.Name,
                        IsKey = column.KeyAttribute != null,
                        IsNullable = column.NullableAttribute != null,
                        IsIdentity = column.IdentityAttribute != null,
                        MaxLength = column.MaxLengthAttribute == null
                                ? null
                                : (int?)column.MaxLengthAttribute.MaxLength

                    };

                //set key names when we hit a key
                if (mereColumn.IsKey)
                {
                    keyColumnName = mereColumn.ColumnName;
                    keyPropertyName = mereColumn.PropertyName;
                } //end set key names when we hit a key


                //add set methods to property helper
                var setAct = BuildSetter(mereColumn);


                //TODO: Add get with implicit operators 
                //add set methods to property helper
                Func<object, object> getFunc = mereColumn.PropertyDescriptor.GetValue;

                mereColumn.SetBase = setAct;
                mereColumn.GetBase = getFunc;
                //end add get and set methods to property helper

                //add to property helpers cache
                if (mereColumn.IsColumn)
                    mereColumns.Add(mereColumn);

                allMereColumns.Add(mereColumn);

                //increment indexer
                intIndex++;
            }

            var viewAttribute =
                (MereViewAttribute)type.GetCustomAttributes(typeof(MereViewAttribute), true).FirstOrDefault();
            var tableAttribute =
                (MereTableAttribute)type.GetCustomAttributes(typeof(MereTableAttribute), true).FirstOrDefault();

            //fill MereTableMin
            var mereTable =
                new MereTableMin<T>()
                {
                    AllMereColumns = allMereColumns,
                    IsView = viewAttribute != null,
                    TableName = tableAttribute != null
                                ? tableAttribute.TableName
                                : viewAttribute != null ? viewAttribute.ViewName : null,
                    DatabaseName = tableAttribute != null
                                    ? tableAttribute.DatabaseName
                                    : viewAttribute != null ? viewAttribute.DatabaseName : null,
                    ServerName = tableAttribute != null
                                    ? tableAttribute.ServerName
                                    : viewAttribute != null ? viewAttribute.ServerName : null,
                    UserId = tableAttribute != null
                                ? tableAttribute.UserId
                                : viewAttribute != null ? viewAttribute.UserId : null,
                    Password = tableAttribute != null
                                ? tableAttribute.Password
                                : viewAttribute != null ? viewAttribute.Password : null,
                    Timeout = tableAttribute != null
                                ? tableAttribute.Timeout
                                : viewAttribute != null ? viewAttribute.Timeout : 0,
                    KeyColumnName = keyColumnName,
                    KeyPropertyName = keyPropertyName,
                    SelectMereColumns = mereColumns
                };
            //end fill MereTableMin

            //add MereTableMin to cache
            Cache.TryAdd(type.TypeHandle, mereTable);

            return mereTable;
        }

        public static MereTableMin CacheCheck(Type type)
        {

            MereTableMin foundMereTableMin;
            if (Cache.TryGetValue(type.TypeHandle, out foundMereTableMin))
                return foundMereTableMin;


            var properties = TypeDescriptor.GetProperties(type).PropertyDescriptorCollectionToList();

            //for columns 
            var hasColumns = properties.Any(
                a => type.GetProperty(a.Name).GetCustomAttributes(typeof(MereColumnAttribute), true).Any());

            var hasKeys = properties.Any(
                a => type.GetProperty(a.Name).GetCustomAttributes(typeof(MereKeyAttribute), true).Any());

            //get fields with column or key attrs or if there are no props with column attr assume 
            //all to be columns and column name is the same as public prop name
            var columnAndKeys =
                properties.Select(
                              s => new
                              {
                                  KeyAttribute =
                              (MereKeyAttribute)
                              type.GetProperty(s.Name)
                                  .GetCustomAttributes(typeof(MereKeyAttribute), true)
                                  .FirstOrDefault(),
                                  ColumnAttribute =
                              (MereColumnAttribute)
                              type.GetProperty(s.Name)
                                  .GetCustomAttributes(typeof(MereColumnAttribute), true)
                                  .FirstOrDefault(),
                                  DisplayNameAttribute =
                              (MereDisplayNameAttribute)
                              type.GetProperty(s.Name)
                                  .GetCustomAttributes(typeof(MereDisplayNameAttribute), true)
                                  .FirstOrDefault(),
                                  IdentityAttribute =
                              (MereIdentityAttribute)
                              type.GetProperty(s.Name)
                                  .GetCustomAttributes(typeof(MereIdentityAttribute), true)
                                  .FirstOrDefault(),
                                  NullableAttribute =
                              (MereNullableAttribute)
                              type.GetProperty(s.Name)
                                  .GetCustomAttributes(typeof(MereNullableAttribute), true)
                                  .FirstOrDefault(),
                                  MaxLengthAttribute =
                              (MereMaxLengthAttribute)
                              type.GetProperty(s.Name)
                                  .GetCustomAttributes(typeof(MereMaxLengthAttribute), true)
                                  .FirstOrDefault(),
                                  PropertyDescriptor = s,
                                  IsColumn = (
                                        (type.GetProperty(s.Name).GetCustomAttributes(typeof(MereColumnAttribute), true).Any()
                                        || type.GetProperty(s.Name).GetCustomAttributes(typeof(MereKeyAttribute), true).Any())
                                        || !hasColumns
                                    )
                                    && !type.GetProperty(s.Name).GetCustomAttributes(typeof(MereIgnoreAttribute), true).Any()
                              }).ToList();

            //var columnNames = new List<string>();
            string keyColumnName = null;
            string keyPropertyName = null;

            var mereColumns = new MereColumnList();
            var allMereColumns = new MereColumnList();

            var intIndex = 0;
            foreach (var column in columnAndKeys)
            {
                var mereColumn =
                    new MereColumn
                    {
                        IsColumn = column.IsColumn,
                        PropertyDescriptor = column.PropertyDescriptor,
                        PropertyName = column.PropertyDescriptor.Name,
                        ColumnName =
                            column.ColumnAttribute != null
                                ? column.ColumnAttribute.CompiledName ?? column.PropertyDescriptor.Name
                                : column.PropertyDescriptor.Name,
                        DisplayName = column.DisplayNameAttribute != null
                                          ? column.DisplayNameAttribute.DisplayName
                                          : column.ColumnAttribute != null
                                                ? column.ColumnAttribute.CompiledName
                                                : column.PropertyDescriptor.Name,
                        IsKey = column.KeyAttribute != null,
                        IsNullable = column.NullableAttribute != null,
                        IsIdentity = column.IdentityAttribute != null,
                        MaxLength = column.MaxLengthAttribute == null
                                        ? null
                                        : (int?)column.MaxLengthAttribute.MaxLength

                    };

                //set key names when we hit a key
                if (mereColumn.IsKey)
                {
                    keyColumnName = mereColumn.ColumnName;
                    keyPropertyName = mereColumn.PropertyName;
                } //end set key names when we hit a key


                //add set methods to property helper
                var setAct = BuildSetter(mereColumn);




                //TODO: Add get with implicit operators 
                //add set methods to property helper
                Func<object, object> getFunc = mereColumn.PropertyDescriptor.GetValue;

                mereColumn.SetBase = setAct;
                mereColumn.GetBase = getFunc;
                //end add get and set methods to property helper

                //add to property helpers cache
                if (mereColumn.IsColumn)
                    mereColumns.Add(mereColumn);

                allMereColumns.Add(mereColumn);

                //increment indexer
                intIndex++;
            }

            var viewAttribute =
                (MereViewAttribute)type.GetCustomAttributes(typeof(MereViewAttribute), true).FirstOrDefault();
            var tableAttribute =
                (MereTableAttribute)type.GetCustomAttributes(typeof(MereTableAttribute), true).FirstOrDefault();

            //fill MereTableMin
            var mereTable =
                new MereTableMin()
                {
                    AllMereColumns = allMereColumns,
                    IsView = viewAttribute != null,
                    TableName = tableAttribute != null
                                    ? tableAttribute.TableName
                                    : viewAttribute != null ? viewAttribute.ViewName : null,
                    DatabaseName = tableAttribute != null
                                       ? tableAttribute.DatabaseName
                                       : viewAttribute != null ? viewAttribute.DatabaseName : null,
                    ServerName = tableAttribute != null
                                     ? tableAttribute.ServerName
                                     : viewAttribute != null ? viewAttribute.ServerName : null,
                    UserId = tableAttribute != null
                                 ? tableAttribute.UserId
                                 : viewAttribute != null ? viewAttribute.UserId : null,
                    Password = tableAttribute != null
                                   ? tableAttribute.Password
                                   : viewAttribute != null ? viewAttribute.Password : null,
                    Timeout = tableAttribute != null
                                  ? tableAttribute.Timeout
                                  : viewAttribute != null ? viewAttribute.Timeout : 0,
                    KeyColumnName = keyColumnName,
                    KeyPropertyName = keyPropertyName,
                    SelectMereColumns = mereColumns
                };
            //end fill MereTableMin

            //add MereTableMin to cache
            Cache.TryAdd(type.TypeHandle, mereTable);

            return mereTable;

        }

        public static Action<object, object> BuildSetter(MereColumn mereColumn)
        {
            Action<object, object> setAct;

            //TODO: Clean up set
            if (mereColumn.PropertyDescriptor.PropertyType == typeof(DateTime))
            {
                //TODO: default to 1/1/1900
                setAct = (parent, val) =>
                {
                    DateTime d;
                    if (!CleanupConversion)
                        d = DateTime.Parse(val.ToString());
                    else
                    {
                        if (val == null || !DateTime.TryParse(val.ToString(), out d))
                            d = new DateTime(1900, 1, 1);

                        if (d.Year < 1900)
                            d = new DateTime(1900, 1, 1, d.Hour, d.Minute, d.Second, d.Millisecond);
                    }

                    mereColumn.PropertyDescriptor.SetValue(parent, d);
                };
            }
            else if (mereColumn.PropertyDescriptor.PropertyType == typeof(DateTime?))
            {
                setAct = (parent, val) =>
                {
                    DateTime? d = null;
                    if (!CleanupConversion)
                        d = (DateTime?)val;
                    else
                    {
                        DateTime test;
                        if (DateTime.TryParse(val == null ? "0" : val.ToString(), out test))
                        {
                            if (test.Year < 1900)
                                test = new DateTime(1900, 1, 1, test.Hour, test.Minute, test.Second, test.Millisecond);

                            d = test;
                        }
                        else
                        {
                            d = null;
                        }
                    }

                    // ReSharper disable AssignNullToNotNullAttribute
                    mereColumn.PropertyDescriptor.SetValue(parent, d);
                    // ReSharper restore AssignNullToNotNullAttribute
                };
            }
            else if (mereColumn.PropertyDescriptor.PropertyType == typeof(int))
            {
                setAct = (parent, val) =>
                {
                    int d;
                    if (!CleanupConversion)
                        d = int.Parse(val.ToString());
                    else
                        int.TryParse(val == null ? "0" : val.ToString(), out d);

                    mereColumn.PropertyDescriptor.SetValue(parent, d);
                };
            }
            else if (mereColumn.PropertyDescriptor.PropertyType == typeof(int?))
            {
                setAct = (parent, val) =>
                {
                    int? d;
                    if (!CleanupConversion)
                        d = (int?)val;
                    else
                    {
                        int test;
                        if (int.TryParse(val == null ? "0" : val.ToString(), out test))
                        {
                            d = test;
                        }
                        else
                        {
                            d = null;
                        }
                    }

                    // ReSharper disable AssignNullToNotNullAttribute
                    mereColumn.PropertyDescriptor.SetValue(parent, d);
                    // ReSharper restore AssignNullToNotNullAttribute
                };
            }
            else if (mereColumn.PropertyDescriptor.PropertyType == typeof(long))
            {
                setAct = (parent, val) =>
                {
                    long d;
                    if (!CleanupConversion)
                        d = long.Parse(val.ToString());
                    else
                        long.TryParse(val == null ? "0" : val.ToString(), out d);

                    mereColumn.PropertyDescriptor.SetValue(parent, d);
                };
            }
            else if (mereColumn.PropertyDescriptor.PropertyType == typeof(long?))
            {
                setAct = (parent, val) =>
                {
                    long? d;
                    if (!CleanupConversion)
                        d = (long?)val;
                    else
                    {
                        long test;
                        if (long.TryParse(val == null ? "0" : val.ToString(), out test))
                        {
                            d = test;
                        }
                        else
                        {
                            d = null;
                        }
                    }

                    mereColumn.PropertyDescriptor.SetValue(parent,
                                                                      d == null ? "" : d.ToString());
                };
            }
            else if (mereColumn.PropertyDescriptor.PropertyType == typeof(decimal))
            {
                setAct = (parent, val) =>
                {
                    decimal d;
                    if (!CleanupConversion)
                        d = decimal.Parse(val.ToString());
                    else
                        decimal.TryParse(val == null ? "0" : val.ToString(), out d);


                    mereColumn.PropertyDescriptor.SetValue(parent, d);
                };
            }
            else if (mereColumn.PropertyDescriptor.PropertyType == typeof(decimal?))
            {
                setAct = (parent, val) =>
                {
                    decimal? d;
                    if (!CleanupConversion)
                        d = (decimal?)val;
                    else
                    {
                        decimal test;
                        if (decimal.TryParse(val == null ? "0" : val.ToString(), out test))
                        {
                            d = test;
                        }
                        else
                        {
                            d = null;
                        }
                    }

                    // ReSharper disable AssignNullToNotNullAttribute
                    mereColumn.PropertyDescriptor.SetValue(parent, d);
                    // ReSharper restore AssignNullToNotNullAttribute
                };
            }
            else
            {
                setAct = mereColumn.PropertyDescriptor.SetValue;
            }

            return setAct;
        }

        /// <summary>
        /// Generates, adds to list, and returns a unique name based on list 
        /// </summary>
        /// <param name="pName"></param>
        /// <param name="usedNames"></param>
        /// <param name="cnt"></param>
        /// <returns></returns>
        public static string NoDupNameCheck(
            string pName, ref IList<string> usedNames, int cnt = 0)
        {
            if (usedNames.Contains(pName.ToLower() + cnt))
                return NoDupNameCheck(pName.ToLower(), ref usedNames, cnt + 1);

            usedNames.Add(pName.ToLower() + cnt);
            return pName.ToLower() + cnt;
        }

        /// <summary>
        /// Generates and returns a unique name based on list
        /// </summary>
        /// <param name="pName"></param>
        /// <param name="usedNames"></param>
        /// <param name="cnt"></param>
        /// <returns></returns>
        public static string NoDupNameCheck(
            string pName, IList<string> usedNames, int cnt = 0)
        {
            if (usedNames.Contains(pName.ToLower() + cnt))
                return NoDupNameCheck(pName.ToLower(), usedNames, cnt + 1);

            return pName.ToLower() + cnt;
        }

        public static string NoDupNameCheck(
            string pName, IList<SqlParameter> usedNames, int cnt = 0)
        {
            if (usedNames.Any(x => x.ParameterName.ToLower() == pName.ToLower() + cnt))
                return NoDupNameCheck(pName.ToLower(), usedNames, cnt + 1);

            return pName.ToLower() + cnt;
        }

        #endregion

        #region parsing methods

        public static string ParseParmObject<T>(
            ref MereContext<T> mereContext,
            ref List<SqlParameter> parameters,
            object obj,
            string whereSql = null,
            bool or = false,
            bool addParamOnly = false,
            bool innerGroup = false,
            int backup = 0
            ) where T : new()
        {

            if (mereContext.CloseFilterGroup)
            {
                if (mereContext.CurMereFilterGroup != null && mereContext.CurMereFilterGroup.HasFilters && !addParamOnly)
                {

                    mereContext.WhereChunks.Add(mereContext.CurMereFilterGroup);
                    mereContext.CurMereFilterGroup = new MereFilterGroup(or);
                }

                mereContext.CloseFilterGroup = false;
            }

            var whereChunk = new StringBuilder();

            if (whereSql != null && !addParamOnly)
            {
                if (whereSql.Trim().ToLower().StartsWith("where"))
                {
                    var sp = whereSql.Trim().Skip(5).Select(s => s).ToArray();
                    whereSql = string.Join("", sp);
                }

            }

            if (obj != null)
            {
                var props = obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();

                for (var i = 0; i < props.Count; i++)
                {
                    var toAdd = ParseParamValue(ref mereContext, ref parameters, props[i].Name, props[i].GetValue(obj), whereSql);

                    if (toAdd == null)
                        continue;

                    if (whereChunk.Length > 0 && whereSql == null)
                    {
                        whereChunk.Append(" AND ");
                    }

                    if (whereSql != null)
                        whereSql = toAdd;
                    else
                        whereChunk.Append(toAdd);
                }

            }
            if (mereContext.WhereChunks == null && !addParamOnly)
                mereContext.WhereChunks = new List<MereFilterGroup>();


            if (!addParamOnly)
            {
                if (mereContext.CurMereFilterGroup == null)
                    mereContext.CurMereFilterGroup = new MereFilterGroup(or);

                mereContext.CurMereFilterGroup.AddFilter(or ? " OR " : " AND ", whereSql ?? whereChunk.ToString(), innerGroup, backup);

            }

            return whereSql ?? whereChunk.ToString();
        }

        public static string ParseParamValue<T>(
            ref MereContext<T> mereContext,
            ref List<SqlParameter> parameters,
            string propOrColName,
            object value,
            string whereSql = null,
            string sqlOperator = null
            ) where T : new()
        {
            string toReturn;

            var mereColumn = mereContext.CurMereTableMin.GetMereColumn(propOrColName);

            if (whereSql == null && mereColumn == null)
                return null;

            if (value == null)
            {

                if (whereSql != null)
                {
                    toReturn = whereSql.Replace("@" + propOrColName, " NULL ");
                }
                else
                {
                    var trimmedSql = sqlOperator != null ? sqlOperator.Trim() : "=";
                    if (trimmedSql == "=")
                        toReturn = string.Format(" [{0}] IS NULL ", mereColumn.ColumnName);
                    else if (trimmedSql == "!=" || trimmedSql == "<>")
                        toReturn = string.Format(" [{0}] IS NOT NULL ", mereColumn.ColumnName);
                    else
                        throw new ArgumentNullException("value", "value can only be with =, !=, or <>");
                }
            }

            var isListOrArray = false;
            var valPropertTypeFullName = "";
            if (value != null)
            {
                var baseType = value.GetType().BaseType;
                valPropertTypeFullName = value.GetType().FullName;
                if (baseType != null)
                {
                    isListOrArray = baseType.FullName.ToLower().Contains("enumerable") ||
                                    baseType.FullName.ToLower().Contains("array") ||
                                    baseType.FullName.ToLower().Contains("list");
                }

                isListOrArray = isListOrArray || value.GetType().FullName.ToLower().Contains("enumerable") ||
                                value.GetType().FullName.ToLower().Contains("array") ||
                                value.GetType().FullName.ToLower().Contains("list");
            }


            if (isListOrArray)
            {
                var forIn = new StringBuilder();
                var cnt = 0;
                foreach (var p in (IEnumerable)value)
                {
                    if (cnt > 0)
                        forIn.Append(", ");

                    if ((mereColumn != null && (mereColumn.PropertyDescriptor.PropertyType.FullName.ToLower().Contains("string")
                        || mereColumn.PropertyDescriptor.PropertyType.FullName.ToLower().Contains("date")))
                        || (valPropertTypeFullName.ToLower().Contains("string")
                        || valPropertTypeFullName.ToLower().Contains("date")))
                    {
                        forIn.AppendFormat("'{0}'", p);
                    }
                    else
                    {
                        forIn.AppendFormat("{0}", p);
                    }
                    cnt++;
                }

                if (whereSql != null)
                {
                    toReturn = whereSql.Replace("@" + propOrColName, forIn.ToString());
                }
                else
                {
                    toReturn = string.Format(" {0} IN ({1}) ", mereColumn.ColumnName, forIn);
                }
            }
            else
            {
                var pName = MereUtils.NoDupNameCheck("@" + propOrColName, parameters);

                //ParamNames.Add(pName);

                var parm = new SqlParameter();
                parm.ParameterName = pName;
                parm.Value = value ?? DBNull.Value;
                parameters.Add(parm);

                if (whereSql != null)
                {
                    toReturn = whereSql.Replace("@" + propOrColName, pName);
                }
                else
                {
                    sqlOperator = sqlOperator ?? " = ";
                    if (value == null)
                        toReturn = string.Format(" [{0}] {1} ", mereColumn.ColumnName, (sqlOperator.Trim() == "!=" || sqlOperator.Trim() == "<>") ? " IS NOT NULL " : " IS NULL ");
                    else
                        toReturn = string.Format(" [{0}] {1} {2} ", mereColumn.ColumnName, sqlOperator ?? " = ", pName);
                }
            }

            return toReturn;
        }

        #endregion

        #region property helpers

        private static PropertyInfo GetPropertyInternal(
            LambdaExpression p)
        {
            MemberExpression memberExpression;

            var body = p.Body as UnaryExpression;
            if (body != null)
            {
                var ue = body;
                memberExpression = (MemberExpression)ue.Operand;
            }
            else
            {
                memberExpression = (MemberExpression)p.Body;
            }
            return (PropertyInfo)(memberExpression).Member;
        }

        public static PropertyInfo GetProperty<TObject>(
            Expression<Func<TObject, object>> p)
        {
            return GetPropertyInternal(p);
        }

        public static PropertyInfo GetProperty<TObject, TProp>(
            Expression<Func<TObject, TProp>> p)
        {
            return GetPropertyInternal(p);
        }

        #endregion

        #region base helpers

        #region emailer

        public static void SendEmail(
            string host, int port,
            string from, IList<string> to,
            string subject, string body,
            bool bodyIsHtml = false,
            IList<string> attachments = null)
        {
            var msg = new MailMessage();

            for (var i = 0; i < to.Count; i++)
            {
                var t = to[i];
                msg.To.Add(t);
            }

            msg.From = new MailAddress(from);
            msg.Body = body;
            msg.IsBodyHtml = bodyIsHtml;
            msg.Priority = MailPriority.Normal;

            if (attachments != null)
            {
                for (var i = 0; i < attachments.Count; i++)
                {
                    msg.Attachments.Add(new Attachment(attachments[i]));
                }
            }

            using (var smtp = new SmtpClient(host, port))
            {
                smtp.Send(msg);
            }
        }

        public class Emailer
        {
            public Emailer(string host, int port)
            {
                Host = host;
                Port = port;
            }

            public string Host { get; set; }

            public int Port { get; set; }

            public string From { get; set; }

            public IList<string> To { get; set; }

            public IList<string> Cc { get; set; }

            public IList<string> Bcc { get; set; }

            public IList<Attachment> Attachments { get; set; }

            public void AddAttachment(string path)
            {
                AddAttachment(new Attachment(path));
            }

            public void AddAttachment(Attachment attachment)
            {
                if (Attachments == null)
                    Attachments = new List<Attachment>();
                Attachments.Add(attachment);
            }

            public void SendHtmlEmail(
                string subject, string htmlBody, params string[] to)
            {
                SendEmail(to, subject, htmlBody, true);
            }

            public void SendHtmlEmailWithAttachments(
                string subject, string htmlBody, string to,
                params string[] attachments)
            {
                SendEmail(new[] { to }, subject, htmlBody, true, attachments.ToList());
            }

            public void SendEmail(
                string subject, string body, params string[] to)
            {
                SendEmail(to, subject, body);
            }

            public void SendEmailWithAttachments(
                string subject, string body, string to,
                params string[] attachments)
            {
                SendEmail(new[] { to }, subject, body, false, attachments.ToList());
            }

            private void SendEmail(
                IList<string> to, string subject, string body,
                bool bodyIsHtml = false,
                IList<string> attachments = null)
            {
                var msg = new MailMessage();

                for (var i = 0; i < to.Count; i++)
                {
                    msg.To.Add(to[i]);
                }

                msg.Subject = subject;
                msg.From = new MailAddress(From);
                msg.Body = body;
                msg.IsBodyHtml = bodyIsHtml;
                msg.Priority = MailPriority.Normal;

                if (attachments != null)
                {
                    for (var i = 0; i < attachments.Count; i++)
                    {
                        msg.Attachments.Add(new Attachment(attachments[i]));
                    }
                }

                if (Attachments != null)
                {
                    for (var i = 0; i < Attachments.Count; i++)
                    {
                        msg.Attachments.Add(Attachments[i]);
                    }
                }

                using (var smtp = new SmtpClient(Host, Port))
                {
                    smtp.Send(msg);
                }
            }
        }

        #endregion

        #region ints
        //public static class Ints
        //{
        /// <summary>
        /// Checks if int is in range created using provided start and end values
        /// </summary>
        /// <param name="toCheck"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static bool InRange(int toCheck, int start, int end)
        {
            if (start < end)
                return CreateRange(start, end).Contains(toCheck);

            if (start == end)
                return toCheck == start;

            throw new ArgumentException("Start number must be less than end number");
        }

        /// <summary>
        /// Create range of numbers using provided start and end values
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static IEnumerable<int> CreateRange(int start, int end)
        {
            return Enumerable.Range(start, (end - start) + 1);
        }
        //}
        #endregion

        #region strings

        //public static class Strings
        //{
        /// <summary>
        /// Removes multiple spaces for strings
        /// </summary>
        /// <param name="strIn"></param>
        /// <returns></returns>
        public static string RemoveMultipleSpaces(string strIn)
        {
            var regex = new Regex(@"[ ]{2,}", RegexOptions.None);
            return regex.Replace(strIn, @" ").Trim();
        }

        /// <summary>
        /// Encrypts string with salt
        /// </summary>
        /// <param name="plain"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        public static string Encrypt(string plain, string salt = "KX1c#Vb5Gt6l@")
        {
            const string s = "zAqWsX1cD2e3Rf4Vb5Gt6YhN7mJu8IkZ9aQwS0xC*dErF&vBgT^yHn%MjUi$K#l@Op!PoL";

            plain = "4Vb5Gt6YhN" + plain + "yHn%MjUi$K#l@Op!PoL";
            var sb = new StringBuilder();
            for (var i = 0; i < plain.Length; i++)
            {
                sb.AppendFormat("{0}{1}", s[i % s.Length], plain[i]);

                if (salt.Length > 0)
                    sb.Append(salt[i % salt.Length]);
            }

            var h = HashAlgorithm.Create("SHA1");

            if (h == null)
                return null;

            var binHash = h.ComputeHash(Encoding.Unicode.GetBytes(sb.ToString()));

            return Convert.ToBase64String(binHash);
        }

        /// <summary>
        /// Splits string on " ", ".", and "?" to find the word count
        /// </summary>
        /// <param name="str"></param>
        /// <returns>Returns number of words in str</returns>
        public static int WordCount(string str)
        {
            return GetWords(str).Count();
        }

        /// <summary>
        /// Splits string on ' ', '.', '?', '!', ';', '(', ')' to find the words
        /// </summary>
        /// <param name="str"></param>
        /// <returns>Returns IEnumerable of words in str</returns>
        public static IEnumerable<string> GetWords(string str)
        {
            return str.Split(new[] { ' ', '.', '?', '!', ';', '(', ')' }, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// Splits string on spaces
        /// </summary>
        /// <param name="strIn"></param>
        /// <param name="removeEmpties"></param>
        /// <returns></returns>
        public static IEnumerable<string> SplitAndStripOnSpaces(string strIn, bool removeEmpties = false)
        {
            if (removeEmpties)
                return strIn.Split(" ".ToArray(), StringSplitOptions.RemoveEmptyEntries);

            return strIn.Split(' ');
        }

        /// <summary>
        /// Splits string on spaces
        /// </summary>
        /// <param name="strIn"></param>
        /// <param name="removeEmpties"></param>
        /// <returns>IEnumerable after spliting and making lowercase</returns>
        public static IEnumerable<string> SplitAndStripOnSpacesToLower(string strIn, bool removeEmpties = false)
        {
            if (removeEmpties)
                return strIn.Split(" ".ToArray(), StringSplitOptions.RemoveEmptyEntries).Select(s => s.ToLower());

            return strIn.Split(' ').Select(s => s.ToLower());
        }

        public static IEnumerable<string> SplitOnNewLine(string strIn, bool removeEmpties = false)
        {
            if (removeEmpties)
                return strIn.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            return strIn.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
        }
        //}//end strings
        #endregion

        #region datetimes

        #region julian helpers
        public static bool IsJulianDate(int year, int month, int day)
        {
            // All dates prior to 1582 are in the Julian calendar
            if (year < 1582)
                return true;
            // All dates after 1582 are in the Gregorian calendar
            else if (year > 1582)
                return false;
            else
            {
                // If 1582, check before October 4 (Julian) or after October 15 (Gregorian)
                if (month < 10)
                    return true;
                else if (month > 10)
                    return false;
                else
                {
                    if (day < 5)
                        return true;
                    else if (day > 14)
                        return false;
                    else
                        // Any date in the range 10/5/1582 to 10/14/1582 is invalid 
                        throw new ArgumentOutOfRangeException("This date is not valid as it does not exist in either the Julian or the Gregorian calendars.");
                }
            }
        }

        public static bool IsJulianDate(DateTime date)
        {
            return IsJulianDate(date.Year, date.Month, date.Day);
        }

        private static double DateToJulianDate(int year, int month, int day, int hour, int minute, int second, int millisecond)
        {
            // Determine correct calendar based on date
            var julianCalendar = IsJulianDate(year, month, day);

            var m = month > 2 ? month : month + 12;
            var y = month > 2 ? year : year - 1;
            var d = day + hour / 24.0 + minute / 1440.0 + (second + millisecond * 1000) / 86400.0;
            var b = julianCalendar ? 0 : 2 - y / 100 + y / 100 / 4;

            return (int)(365.25 * (y + 4716)) + (int)(30.6001 * (m + 1)) + d + b - 1524.5;
        }

        public static double JulianDate(int year, int month, int day, int hour, int minute, int second, int millisecond)
        {
            return DateToJulianDate(year, month, day, hour, minute, second, millisecond);
        }


        public static double JulianDate(DateTime date)
        {
            return DateToJulianDate(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second, date.Millisecond);
        }
        #endregion

        //public static class DateTimes
        //{

        public static DateTime GetDayByOccurrenceInMonth(
            DateTime startDate, DayOfWeek day, int occurrence)
        {
            if (occurrence < 0)
                throw new ArgumentException("Can't find 0 occurrences of a day. Occurrence must greater than 0.");

            if (occurrence > 5)
                throw new ArgumentException("Can't have more than 5 occurrences of a day in a month. Occurrence must less than or equal to 5.");

            var curDate = startDate.GetFirstDayOfMonth();
            var cnt = 0;

            while (curDate.DayOfWeek != day)
            {
                if (cnt > 5)
                    throw new ArgumentException("Choosen day doesn't occurr that many time in current month");

                curDate = curDate.AddDays(1);
            }

            return curDate;
        }

        //        public static DateTime GetDayByOccurrenceInMonth(DateTime startDate, DayOfWeek day, int occurrence)
        //        {
        //            if (occurrence == 0)
        //                throw new ArgumentException("Can't find 0 occerrences of a day. Occerrence must greater or less than 0.");
        //
        //            var cnt = occurrence > 1 ? 1 : -1;
        //            var cntIncre = occurrence > 1 ? 1 : -1;
        //            var incre = cntIncre;
        //            var movedToWeekIncre = false;
        //
        //            while (cnt != occurrence)
        //            {
        //                if (startDate.DayOfWeek == day)
        //                {
        //                    cnt += cntIncre;
        //
        //                    if (!movedToWeekIncre)
        //                    {
        //                        incre *= 7;
        //                        movedToWeekIncre = true;
        //                    }
        //                }
        //
        //                startDate = startDate.AddDays(incre);
        //            }
        //
        //            return startDate;
        //        }

        //        public static DateTime GetDayByOccurrenceInMonthFromFirst(DateTime now, DayOfWeek day, int occurrence)
        //        {
        //            if (occurrence == 0)
        //                throw new ArgumentException("Can't find 0 occerrences of a day. Occerrence must greater or less than 0.");
        //
        //            now = GetFirstDayOfMonth(now);
        //            var cnt = occurrence > 1 ? 1 : -1;
        //            var cntIncre = occurrence > 1 ? 1 : -1;
        //            var incre = cntIncre;
        //            var movedToWeekIncre = false;
        //
        //            while (cnt != occurrence)
        //            {
        //                if (now.DayOfWeek == day)
        //                {
        //                    cnt += cntIncre;
        //
        //                    if (!movedToWeekIncre)
        //                    {
        //                        incre *= 7;
        //                        movedToWeekIncre = true;
        //                    }
        //                }
        //
        //                now = now.AddDays(incre);
        //            }
        //
        //            return now;
        //        }

        public static DateTime GetFirstDayOfLastMonth()
        {
            return GetFirstDayOfLastMonth(DateTime.Now);
        }

        public static DateTime GetFirstDayOfLastMonth(DateTime now)
        {
            return GetFirstDayOfMonth(now.AddMonths(-1));
        }

        public static DateTime GetFirstDayOfMonth()
        {
            return GetFirstDayOfMonth(DateTime.Now);
        }

        public static DateTime GetFirstDayOfMonth(DateTime now)
        {
            return now.AddDays((now.Day - 1) * (-1));
        }

        public static DateTime GetLastDayOfLastMonth()
        {
            return GetLastDayOfLastMonth(DateTime.Now);
        }

        public static DateTime GetLastDayOfLastMonth(DateTime now)
        {
            return now.AddDays(now.Day * (-1));
        }

        public static DateTime GetLastDayOfMonth()
        {
            return GetLastDayOfMonth(DateTime.Now);
        }

        public static DateTime GetLastDayOfMonth(DateTime now)
        {
            return now.AddMonths(1).AddDays(now.Day * (-1));
        }

        public static DateTime GoToPrevMonth(int month)
        {
            return GoToPrevMonth(DateTime.Now, month);
        }

        public static DateTime GoToPrevMonth(DateTime now, int month)
        {
            return now.AddMonths((now.Month - month) * (-1));
        }

        public static DateTime GoToNextMonth(int month)
        {
            return GoToNextMonth(DateTime.Now, month);
        }

        public static DateTime GoToNextMonth(DateTime now, int month)
        {
            return now.AddMonths(now.Month < month ? month - now.Month : 12 - (now.Month - month));
        }

        public static DateTime GetFirstDayOfLastYear()
        {
            return GetFirstDayOfLastYear(DateTime.Now);
        }

        public static DateTime GetFirstDayOfLastYear(DateTime now)
        {
            return GetFirstDayOfYear(now).AddYears(-1);
        }

        public static DateTime GetFirstDayOfYear()
        {
            return GetFirstDayOfYear(DateTime.Now);
        }

        public static DateTime GetFirstDayOfYear(DateTime now)
        {
            return GetFirstDayOfMonth(now.Month > 1 ? GoToPrevMonth(now, 1) : now);
        }

        public static DateTime GetLastDayOfLastYear()
        {
            return GetLastDayOfLastYear(DateTime.Now);
        }

        public static DateTime GetLastDayOfLastYear(DateTime now)
        {
            return GetLastDayOfYear(now).AddYears(-1);
        }

        public static DateTime GetLastDayOfYear()
        {
            return GetLastDayOfYear(DateTime.Now);
        }

        public static DateTime GetLastDayOfYear(DateTime now)
        {
            return GetFirstDayOfYear(now).AddYears(1).AddDays(-1);
        }

        public static DateTime GetFirstDayOfQuarter()
        {
            return GetFirstDayOfQuarter(DateTime.Now);
        }

        public static DateTime GetFirstDayOfQuarter(DateTime now)
        {
            var quarter = GetQuarter(now);
            //var yearsToAdd = quarter == 4 ? -1 : 0;
            var monthsToAdd = now.Month > 1 ? (now.Month - (quarter * 3 - 2)) * (-1) : -12;

            return GetFirstDayOfMonth(now.AddMonths(monthsToAdd));//.AddYears(yearsToAdd));
        }

        public static DateTime GetFirstDayLastQuarter()
        {
            return GetFirstDayLastQuarter(DateTime.Now);
        }

        public static DateTime GetFirstDayLastQuarter(DateTime now)
        {
            var lastQuarter = GetLastQuarter(now);
            var yearsToAdd = lastQuarter == 4 ? -1 : 0;
            var monthsToAdd = now.Month > 1 ? (now.Month - (lastQuarter * 3 - 2)) * (-1) : -12;

            return GetFirstDayOfMonth(now.AddMonths(monthsToAdd).AddYears(yearsToAdd));
        }

        public static DateTime GetLastDayOfQuarter()
        {
            return GetLastDayOfQuarter(DateTime.Now);
        }

        public static DateTime GetLastDayOfQuarter(DateTime now)
        {
            var quarter = GetQuarter(now);
            //var yearsToAdd = quarter == 4 ? -1 : 0;
            var monthsToAdd = now.Month > 1 ? (now.Month - (quarter * 3)) * (-1) : -12;

            return GetLastDayOfMonth(now.AddMonths(monthsToAdd));//.AddYears(yearsToAdd));
        }

        public static DateTime GetLastDayLastQuarter()
        {
            return GetLastDayLastQuarter(DateTime.Now);
        }

        public static DateTime GetLastDayLastQuarter(DateTime now)
        {
            var lastQuarter = GetLastQuarter(now);
            var yearsToAdd = lastQuarter == 4 ? -1 : 0;
            var monthsToAdd = now.Month > 1 ? (now.Month - (lastQuarter * 3)) * (-1) : -12;

            return GetLastDayOfMonth(now.AddMonths(monthsToAdd).AddYears(yearsToAdd));
        }

        public static DateTime GetFirstDayLastFirstQuarter()
        {
            return GetFirstDayLastFirstQuarter(DateTime.Now);
        }

        public static DateTime GetFirstDayLastFirstQuarter(DateTime now)
        {
            return GetFirstDayOfMonth(GoToPrevMonth(1));
        }

        public static DateTime GetFirstDayLastSecondQuarter()
        {
            return GetFirstDayLastSecondQuarter(DateTime.Now);
        }

        public static DateTime GetFirstDayLastSecondQuarter(DateTime now)
        {
            return GetFirstDayOfMonth(GoToPrevMonth(4));
        }

        public static DateTime GetFirstDayLastThirdQuarter()
        {
            return GetFirstDayLastThirdQuarter(DateTime.Now);
        }

        public static DateTime GetFirstDayLastThirdQuarter(DateTime now)
        {
            return GetFirstDayOfMonth(GoToPrevMonth(7));
        }

        public static DateTime GetFirstDayLastFourthQuarter()
        {
            return GetFirstDayLastFourthQuarter(DateTime.Now);
        }

        public static DateTime GetFirstDayLastFourthQuarter(DateTime now)
        {
            return GetFirstDayOfMonth(GoToPrevMonth(10));
        }

        public static DateTime GetFirstDayNextFirstQuarter()
        {
            return GetFirstDayNextFirstQuarter(DateTime.Now);
        }

        public static DateTime GetFirstDayNextFirstQuarter(DateTime now)
        {
            return GetFirstDayOfMonth(GoToNextMonth(1));
        }

        public static DateTime GetFirstDayNextSecondQuarter()
        {
            return GetFirstDayNextSecondQuarter(DateTime.Now);
        }

        public static DateTime GetFirstDayNextSecondQuarter(DateTime now)
        {
            return GetFirstDayOfMonth(GoToNextMonth(4));
        }

        public static DateTime GetFirstDayNextThirdQuarter()
        {
            return GetFirstDayNextThirdQuarter(DateTime.Now);
        }

        public static DateTime GetFirstDayNextThirdQuarter(DateTime now)
        {
            return GetFirstDayOfMonth(GoToNextMonth(7));
        }

        public static DateTime GetFirstDayNextFourthQuarter()
        {
            return GetFirstDayNextFourthQuarter(DateTime.Now);
        }

        public static DateTime GetFirstDayNextFourthQuarter(DateTime now)
        {
            return GetFirstDayOfMonth(GoToNextMonth(10));
        }

        public static DateTime GetLastDayLastFirstQuarter()
        {
            return GetLastDayLastFirstQuarter(DateTime.Now);
        }

        public static DateTime GetLastDayLastFirstQuarter(DateTime now)
        {
            return GetLastDayOfMonth(GoToPrevMonth(1));
        }

        public static DateTime GetLastDayLastSecondQuarter()
        {
            return GetLastDayLastSecondQuarter(DateTime.Now);
        }

        public static DateTime GetLastDayLastSecondQuarter(DateTime now)
        {
            return GetLastDayOfMonth(GoToPrevMonth(4));
        }

        public static DateTime GetLastDayLastThirdQuarter()
        {
            return GetLastDayLastThirdQuarter(DateTime.Now);
        }

        public static DateTime GetLastDayLastThirdQuarter(DateTime now)
        {
            return GetLastDayOfMonth(GoToPrevMonth(7));
        }

        public static DateTime GetLastDayLastFourthQuarter()
        {
            return GetLastDayLastFourthQuarter(DateTime.Now);
        }

        public static DateTime GetLastDayLastFourthQuarter(DateTime now)
        {
            return GetLastDayOfMonth(GoToPrevMonth(10));
        }

        public static DateTime GetLastDayNextFirstQuarter()
        {
            return GetLastDayNextFirstQuarter(DateTime.Now);
        }

        public static DateTime GetLastDayNextFirstQuarter(DateTime now)
        {
            return GetLastDayOfMonth(GoToNextMonth(1));
        }

        public static DateTime GetLastDayNextSecondQuarter()
        {
            return GetLastDayNextSecondQuarter(DateTime.Now);
        }

        public static DateTime GetLastDayNextSecondQuarter(DateTime now)
        {
            return GetLastDayOfMonth(GoToNextMonth(4));
        }

        public static DateTime GetLastDayNextThirdQuarter()
        {
            return GetLastDayNextThirdQuarter(DateTime.Now);
        }

        public static DateTime GetLastDayNextThirdQuarter(DateTime now)
        {
            return GetLastDayOfMonth(GoToNextMonth(7));
        }

        public static DateTime GetLastDayNextFourthQuarter()
        {
            return GetLastDayNextFourthQuarter(DateTime.Now);
        }

        public static DateTime GetLastDayNextFourthQuarter(DateTime now)
        {
            return GetLastDayOfMonth(GoToNextMonth(10));
        }

        public static int GetQuarter()
        {
            return GetQuarter(DateTime.Now);
        }

        public static int GetQuarter(DateTime now)
        {
            return (now.Month - 1) / 3 + 1;
        }

        public static int GetLastQuarter()
        {
            return GetQuarter(DateTime.Now) - 1;
        }

        public static int GetLastQuarter(DateTime now)
        {
            var lastQuarter = GetQuarter(now) - 1;
            return lastQuarter < 1 ? 4 : lastQuarter;
        }

        public static int GetNextQuarter()
        {
            return GetNextQuarter(DateTime.Now);
        }

        public static int GetNextQuarter(DateTime now)
        {
            var lastQuarter = GetQuarter(now) + 1;
            return lastQuarter > 4 ? 1 : lastQuarter;
        }

        public static bool IsQuarter(DateTime now, int quarter)
        {
            return GetQuarter(now) == quarter;
        }

        public static bool IsFirstQuarter(DateTime now)
        {
            return IsQuarter(now, 1);
        }

        public static bool IsSecondQuarter(DateTime now)
        {
            return IsQuarter(now, 2);
        }

        public static bool IsThirdQuarter(DateTime now)
        {
            return IsQuarter(now, 3);
        }

        public static bool IsFourthQuarter(DateTime now)
        {
            return IsQuarter(now, 4);
        }

        public static bool IsFirstQuarter()
        {
            return IsFirstQuarter(DateTime.Now);
        }

        public static bool IsSecondQuarter()
        {
            return IsSecondQuarter(DateTime.Now);
        }

        public static bool IsThirdQuarter()
        {
            return IsThirdQuarter(DateTime.Now);
        }

        public static bool IsFourthQuarter()
        {
            return IsFirstQuarter(DateTime.Now);
        }

        /// <summary>
        /// Gets date for instance of day next week.
        /// </summary>
        /// <param name="startDate">Date to start at.</param>
        /// <param name="moveTo">String version of day of the week to move to.</param>
        /// <param name="weeksAway">How many weeks away to find date 0 = next instance</param>
        /// <returns></returns>
        public static DateTime GetPrevWeeksDayOfWeekDate(DateTime startDate, DayOfWeek moveTo, int weeksAway)
        {
            weeksAway = (startDate.DayOfWeek < moveTo && weeksAway > 0) ? weeksAway - 1 : weeksAway;

            var daysToAdd = weeksAway * 7;
            var toMove = (7 - (moveTo - startDate.DayOfWeek)) % 7;
            toMove = toMove == 0 ? 7 : (toMove + daysToAdd);
            return startDate.AddDays(toMove * -1);
        }

        /// <summary>
        /// Gets date for instance of day next week.
        /// </summary>
        /// <param name="startDate">Date to start at.</param>
        /// <param name="moveTo">String version of day of the week to move to.</param>
        /// <returns></returns>
        public static DateTime GetPrevWeeksDayOfWeekDate(DateTime startDate, DayOfWeek moveTo)
        {
            return GetPrevWeeksDayOfWeekDate(startDate, moveTo, 1);
        }

        /// <summary>
        /// Gets date for instance of day next week.
        /// </summary>
        /// <param name="startDate">Date to start at.</param>
        /// <param name="moveTo">String version of day of the week to move to.</param>
        /// <param name="weeksAway">How many weeks away to find date 0 = next instance</param>
        /// <returns></returns>
        public static DateTime GetPrevWeeksDayOfWeekDate(DateTime startDate, string moveTo, int weeksAway)
        {
            DayOfWeek dow;
            if (!Enum.TryParse(moveTo, true, out dow))
                throw new ArgumentException("Provided moveTo value was not a valid day of the week string");

            return GetPrevWeeksDayOfWeekDate(startDate, dow, weeksAway);
        }

        /// <summary>
        /// Gets date for instance of day next week.
        /// </summary>
        /// <param name="startDate">Date to start at.</param>
        /// <param name="moveTo">String version of day of the week to move to.</param>
        /// <param name="weeksAway">How many weeks away to find date 0 = next instance</param>
        /// <returns></returns>
        public static DateTime GetNextWeeksDayOfWeek(DateTime startDate, DayOfWeek moveTo, int weeksAway)
        {
            var daysToAdd = weeksAway * 7;
            var toMove = ((moveTo - startDate.DayOfWeek) + 7) % 7;
            toMove = toMove == 0 ? 7 : (toMove + daysToAdd);
            return startDate.AddDays(toMove);
        }

        /// <summary>
        /// Gets date for instance of day next week.
        /// </summary>
        /// <param name="startDate">Date to start at.</param>
        /// <param name="moveTo">String version of day of the week to move to.</param>
        /// <returns></returns>
        public static DateTime GetNextWeeksDayOfWeek(DateTime startDate, DayOfWeek moveTo)
        {
            return GetNextWeeksDayOfWeek(startDate, moveTo, 1);
        }

        /// <summary>
        /// Gets date for instance of day next week.
        /// </summary>
        /// <param name="startDate">Date to start at.</param>
        /// <param name="moveTo">String version of day of the week to move to.</param>
        /// <param name="weeksAway">How many weeks away to find date 0 = next instance</param>
        /// <returns></returns>
        public static DateTime GetNextWeeksDayOfWeek(DateTime startDate, string moveTo, int weeksAway)
        {
            DayOfWeek dow;
            if (!Enum.TryParse(moveTo, true, out dow))
                throw new ArgumentException("Provided moveTo value was not a valid day of the week string");

            return GetNextWeeksDayOfWeek(startDate, dow, weeksAway);
        }

        /// <summary>
        /// Gets date for previous instance of day.
        /// </summary>
        /// <param name="startDate">Date to start at.</param>
        /// <param name="moveTo">String version of day of the week to move to.</param>
        /// <returns></returns>
        public static DateTime GetPrevDayOfWeek(DateTime startDate, DayOfWeek moveTo)
        {
            return GetPrevWeeksDayOfWeekDate(startDate, moveTo, 0);
        }

        /// <summary>
        /// Gets date for previous instance of day.
        /// </summary>
        /// <param name="startDate">Date to start at.</param>
        /// <param name="moveTo">Day of the week to move to.</param>
        /// <returns></returns>
        public static DateTime GetPrevDayOfWeek(DateTime startDate, string moveTo)
        {
            return GetPrevWeeksDayOfWeekDate(startDate, moveTo, 0);
        }

        /// <summary>
        /// Gets date for next instance of day.
        /// </summary>
        /// <param name="startDate">Date to start at.</param>
        /// <param name="moveTo">String version of day of the week to move to.</param>
        /// <returns></returns>
        public static DateTime GetNextDayOfWeek(DateTime startDate, DayOfWeek moveTo)
        {
            return GetNextWeeksDayOfWeek(startDate, moveTo, 0);
        }

        /// <summary>
        /// Gets date for next instance of day.
        /// </summary>
        /// <param name="startDate">Date to start at.</param>
        /// <param name="moveTo">Day of the week to move to.</param>
        /// <returns></returns>
        public static DateTime GetNextDayOfWeek(DateTime startDate, string moveTo)
        {
            return GetNextWeeksDayOfWeek(startDate, moveTo, 0);
        }
        //}//end datetimes
        #endregion

        #endregion
    }
}
