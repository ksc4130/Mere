using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Mere.Interfaces;

namespace Mere
{
    public class MereQuery
    {
        protected MereQuery() { }

        public static IMereQueryPre<T> Create<T>() where T : new()
        {
            return MereQuery<T>.Create();
        }
        public static IMereQueryPre<T> Create<T>(MereDataSource dataSource) where T : new()
        {
            return MereQuery<T>.Create(dataSource);
        }
        public static IMereQueryPre<T> Create<T>(int top) where T : new()
        {
            return MereQuery<T>.Create(top);
        }
        public static IMereQueryPre<T> Create<T>(MereDataSource dataSource, int top) where T : new()
        {
            return MereQuery<T>.Create(dataSource, top);
        }
    }
    public class MereQuery<T> where T : new()
    {
        private MereQuery()
        {
            _mereContext = new MereContext<T>();
            _pre = new MereQueryPre(this);
            _post = new MereQueryPost(this);
        }

        private MereQuery(MereDataSource dataSource)
        {
            _mereContext = new MereContext<T>(dataSource);
            _pre = new MereQueryPre(this);
            _post = new MereQueryPost(this);
        }

        private MereQuery(MereDataSource dataSource, int top)
        {
            _mereContext = new MereContext<T>(dataSource, top);
            _pre = new MereQueryPre(this);
            _post = new MereQueryPost(this);
        }

        private MereQuery(int top)
        {
            _mereContext = new MereContext<T>(top);
            _pre = new MereQueryPre(this);
            _post = new MereQueryPost(this);
        }

        public static IMereQueryPre<T> Create()
        {
            var parent = new MereQuery<T>();
            return parent._pre;
        }
        public static IMereQueryPre<T> Create(MereDataSource dataSource)
        {
            var parent = new MereQuery<T>(dataSource);
            
            return parent._pre;
        }
        public static IMereQueryPre<T> Create(int top)
        {
            var parent = new MereQuery<T>(top);
            return parent._pre;
        }
        public static IMereQueryPre<T> Create(MereDataSource dataSource, int top)
        {
            var parent = new MereQuery<T>(dataSource, top);
            return parent._pre;
            
        }

        private MereContext<T> _mereContext;
        private readonly IMereQueryPre<T> _pre;
        private readonly IMereQueryPost<T> _post;

        private class MereQueryBase<T, TProp> : IMereQueryFilter<T, TProp> where T : new()
        {

            public MereQueryBase(MereQuery<T> parent)
            {
                _parent = parent;
            }

            private readonly MereQuery<T> _parent;
            private MereContext<T> QueryContext { get { return _parent._mereContext; } }
            #region Creates
            public static IMereQueryFilter<T, TProp> Create(MereQuery<T> parent)
            {
                return new MereQueryBase<T, TProp>(parent);
            }
            #endregion

            #region filter methods
            public IMereQueryPost<T> EqualTo(TProp value)
            {
                _parent._mereContext.AddFilter(value, SqlOperator.EqualTo);
                return _parent._post;
            }

            public IMereQueryPost<T> EqualToCaseSensitive(TProp value)
            {
                QueryContext.AddFilter(value, SqlOperator.EqualToCaseSensitive);
                return _parent._post;
            }

            public IMereQueryPost<T> NotEqualTo(TProp value)
            {
                QueryContext.AddFilter(value, SqlOperator.NotEqualTo);
                return _parent._post;
            }

            public IMereQueryPost<T> NotEqualToCaseSensitive(TProp value)
            {
                QueryContext.AddFilter(value, SqlOperator.NotEqualToCaseSensitive);
                return _parent._post;
            }

            public IMereQueryPost<T> GreaterThan(TProp value)
            {

                QueryContext.AddFilter(value, SqlOperator.GreaterThan);
                return _parent._post;
            }

            public IMereQueryPost<T> GreaterThanOrEqualTo(TProp value)
            {
                QueryContext.AddFilter(value, SqlOperator.GreaterThanOrEqualTo);
                return _parent._post;
            }

            public IMereQueryPost<T> LessThan(TProp value)
            {
                QueryContext.AddFilter(value, SqlOperator.LessThan);
                return _parent._post;
            }

            public IMereQueryPost<T> LessThanOrEqualTo(TProp value)
            {
                QueryContext.AddFilter(value, SqlOperator.LessThanOrEqualTo);
                return _parent._post;
            }

            public IMereQueryPost<T> In<TVal>(TVal values) where TVal : IEnumerable<TProp>
            {
                QueryContext.AddFilterIn(values);
                return _parent._post;
            }

            public IMereQueryPost<T> In(params TProp[] values)
            {
                QueryContext.AddFilterIn(values);
                return _parent._post;
            }

            public IMereQueryPost<T> InCaseSensitive<TVal>(TVal values) where TVal : IEnumerable<TProp>
            {
                QueryContext.AddFilterInCaseSensitive(values);
                return _parent._post;
            }

            public IMereQueryPost<T> InCaseSensitive(params TProp[] values)
            {
                QueryContext.AddFilterInCaseSensitive(values);
                return _parent._post;
            }

            public IMereQueryPost<T> NotIn<TVal>(TVal values) where TVal : IEnumerable<TProp>
            {
                QueryContext.AddFilterNotIn(values);
                return _parent._post;
            }

            public IMereQueryPost<T> NotIn(params TProp[] values)
            {
                QueryContext.AddFilterNotIn(values);
                return _parent._post;
            }

            public IMereQueryPost<T> NotInCaseSensitive<TVal>(TVal values) where TVal : IEnumerable<TProp>
            {
                QueryContext.AddFilterNotInCaseSensitive(values);
                return _parent._post;
            }

            public IMereQueryPost<T> NotInCaseSensitive(params TProp[] values)
            {
                QueryContext.AddFilterNotInCaseSensitive(values);
                return _parent._post;
            }

            public IMereQueryPost<T> Between(TProp value1, TProp value2)
            {
                QueryContext.AddFilterBetween(value1, value2);
                return _parent._post;
            }

            public IMereQueryPost<T> NotBetween(TProp value1, TProp value2)
            {
                QueryContext.AddFilterNotBetween(value1, value2);
                return _parent._post;
            }

            public IMereQueryPost<T> Contains(TProp value)
            {
                var v = "%" + value + "%";
                QueryContext.AddFilter(v, SqlOperator.Like);
                return _parent._post;
            }

            public IMereQueryPost<T> NotContains(TProp value)
            {
                var v = "%" + value + "%";
                QueryContext.AddFilter(v, SqlOperator.NotLike);
                return _parent._post;
            }

            public IMereQueryPost<T> StartsWith(TProp value)
            {
                var v = value + "%";

                QueryContext.AddFilter(v, SqlOperator.Like);
                return _parent._post;
            }

            public IMereQueryPost<T> NotStartsWith(TProp value)
            {
                var v = value + "%";

                QueryContext.AddFilter(v, SqlOperator.NotLike);
                return _parent._post;
            }

            public IMereQueryPost<T> EndsWith(TProp value)
            {
                var v = "%" + value;

                QueryContext.AddFilter(v, SqlOperator.Like);
                return _parent._post;
            }

            public IMereQueryPost<T> NotEndsWith(TProp value)
            {
                var v = "%" + value;

                QueryContext.AddFilter(v, SqlOperator.NotLike);
                return _parent._post;
            }

            public IMereQueryPost<T> ContainsCaseSensitive(TProp value)
            {
                var v = "%" + value + "%";
                QueryContext.AddFilter(v, SqlOperator.LikeCaseSensitive);
                return _parent._post;
            }

            public IMereQueryPost<T> NotContainsCaseSensitive(TProp value)
            {
                var v = "%" + value + "%";
                QueryContext.AddFilter(v, SqlOperator.NotLikeCaseSensitive);
                return _parent._post;
            }

            public IMereQueryPost<T> StartsWithCaseSensitive(TProp value)
            {
                var v = value + "%";

                QueryContext.AddFilter(v, SqlOperator.LikeCaseSensitive);
                return _parent._post;
            }

            public IMereQueryPost<T> NotStartsWithCaseSensitive(TProp value)
            {
                var v = value + "%";

                QueryContext.AddFilter(v, SqlOperator.NotLikeCaseSensitive);
                return _parent._post;
            }

            public IMereQueryPost<T> EndsWithCaseSensitive(TProp value)
            {
                var v = "%" + value;

                QueryContext.AddFilter(v, SqlOperator.LikeCaseSensitive);
                return _parent._post;
            }

            public IMereQueryPost<T> NotEndsWithCaseSensitive(TProp value)
            {
                var v = "%" + value;

                QueryContext.AddFilter(v, SqlOperator.NotLikeCaseSensitive);
                return _parent._post;
            }

            #endregion
        }

        private class MereQueryPre : MereQueryBase, IMereQueryPre<T>
        {

            public MereQueryPre(MereQuery<T> parent) : base(parent) { }

            #region methods
            public IMereQueryPre<T> SetTop(int top)
            {
                QueryContext.Top = top;
                return this;
            }

            public IMereQueryPre<T> SetTimeout(int timeout)
            {
                QueryContext.Timeout = timeout;
                return this;
            }

            public IMereQueryPre<T> SetFields<TFields>()
            {
                QueryContext.SetFields<TFields>();
                return this;
            }

            public IMereQueryPre<T> SetFields<TFields>(Func<T, TFields> newFields)
            {
                QueryContext.SetFields(newFields);
                return this;
            }

            public IMereQueryPre<T> SetFields(dynamic fields)
            {
                QueryContext.SetFields(fields);
                return this;
            }

            public IMereQueryPre<T> OrderBy<TProp>(Expression<Func<T, TProp>> prop, bool descending = false)
            {
                var pName = prop.GetPropName();

                //QueryContext.OrderByList = QueryContext.OrderByList ?? new List<string>();

                var desc = descending ? " DESC " : "";
                QueryContext.OrderByList.Add("[" + pName + "]" + desc);
                //QueryContext.QueryHasChangeSinceLastExecute = true;
                return this;
            }

            public IMereQueryPre<T> OrderBy<TProp>(IEnumerable<string> fields, bool allDescending = false)
            {
                //QueryContext.OrderByList = QueryContext.OrderByList ?? new List<string>();

                var desc = allDescending ? " DESC " : "";

                foreach (var pName in fields)
                {
                    QueryContext.OrderByList.Add("[" + pName + "]" + desc);
                }


                //QueryContext.QueryHasChangeSinceLastExecute = true;
                return this;
            }

            public IMereQueryPre<T> SetDatabase(string databaseName)
            {
                QueryContext.DatabaseName = databaseName;
                return this;
            }

            public IMereQueryPre<T> SetServer(string serverName)
            {
                QueryContext.ServerName = serverName;
                return this;
            }

            public IMereQueryPre<T> SetTable(string tableName)
            {
                QueryContext.TableName = tableName;
                return this;
            }

            public IMereQueryPre<T> SetUserId(string userId)
            {
                QueryContext.UserId = userId;
                return this;
            }

            public IMereQueryPre<T> SetPassword(string password)
            {
                QueryContext.Password = password;
                return this;
            }
            #endregion

            #region filter methods
            public IMereQueryFilter<T, TProp> Where<TProp>(Expression<Func<T, TProp>> fieldSelector)
            {
                QueryContext.InitFilterAdd(fieldSelector);
                return new MereQueryBase<T, TProp>(Parent);
            }

            public IMereQueryPost<T> Where(string whereSql, object whereValues = null)
            {
                QueryContext.AddFilter(whereSql, whereValues);
                return Parent._post;
            }

            #endregion
        }

        private class MereQueryPost : MereQueryBase, IMereQueryPost<T>
        {
            public MereQueryPost(MereQuery<T> parent) : base(parent) { }

            #region methods
            public IMereQueryPost<T> Top(int top)
            {
                QueryContext.Top = top;
                return this;
            }

            public IMereQueryPost<T> SetTimeout(int timeout)
            {
                QueryContext.Timeout = timeout;
                return this;
            }

            public IMereQueryPost<T> SetFields<TFields>()
            {
                QueryContext.SetFields<TFields>();
                return this;
            }

            public IMereQueryPost<T> SetFields<TFields>(Func<T, TFields> newFields)
            {
                QueryContext.SetFields(newFields);
                return this;
            }

            public IMereQueryPost<T> SetFields(dynamic fields)
            {
                QueryContext.SetFields(fields);
                return this;
            }

            public IMereQueryPost<T> OrderBy<TProp>(Expression<Func<T, TProp>> prop, bool descending = false)
            {
                var pName = prop.GetPropName();

                //QueryContext.OrderByList = QueryContext.OrderByList ?? new List<string>();

                var desc = descending ? " DESC " : "";
                QueryContext.OrderByList.Add("[" + pName + "]" + desc);
                //QueryContext.QueryHasChangeSinceLastExecute = true;
                return this;
            }

            public IMereQueryPost<T> OrderBy<TProp>(IEnumerable<string> fields, bool allDescending = false)
            {
                //QueryContext.OrderByList = QueryContext.OrderByList ?? new List<string>();

                var desc = allDescending ? " DESC " : "";

                foreach (var pName in fields)
                {
                    QueryContext.OrderByList.Add("[" + pName + "]" + desc);
                }


                //QueryContext.QueryHasChangeSinceLastExecute = true;
                return this;
            }

            public IMereQueryPost<T> SetDatabase(string databaseName)
            {
                QueryContext.DatabaseName = databaseName;
                return this;
            }

            public IMereQueryPost<T> SetServer(string serverName)
            {
                QueryContext.ServerName = serverName;
                return this;
            }

            public IMereQueryPost<T> SetTable(string tableName)
            {
                QueryContext.TableName = tableName;
                return this;
            }

            public IMereQueryPost<T> SetUserId(string userId)
            {
                QueryContext.UserId = userId;
                return this;
            }

            public IMereQueryPost<T> SetPassword(string password)
            {
                QueryContext.Password = password;
                return this;
            }
            #endregion

            #region filter methods

            #region AndsAndAndGroups
            //ands
            /// <summary>
            /// Initiate adding an and filter by selecting the field to filter on
            /// </summary>
            /// <typeparam name="TProp">Selected property type</typeparam>
            /// <param name="fieldSelector">Lambda exepression to select field to filter on</param>
            /// <returns>QueryObject to add operator and value</returns>
            public IMereQueryFilter<T, TProp> And<TProp>(Expression<Func<T, TProp>> fieldSelector)
            {
                QueryContext.InitFilterAdd(fieldSelector);
                return new MereQueryBase<T, TProp>(Parent);
            }

            /// <summary>
            /// Adding custom and filter/statement with hardcoded values or 
            /// pass in an object with properties named the same as you parameters minus the "@" sign.
            /// </summary>
            /// <param name="whereSql">Custom where filter/statement</param>
            /// <param name="whereValues">Parameter values for custom filter/statement</param>
            /// <returns></returns>
            public IMereQueryPost<T> And(string whereSql, object whereValues = null)
            {
                QueryContext.AddFilter(whereSql, whereValues);
                return this;
            }

            //and filter groups
            public IMereQueryFilter<T, TProp> AndGroup<TProp>(Expression<Func<T, TProp>> prop)
            {
                QueryContext.InitFilterGroupAdd(prop);
                return new MereQueryBase<T, TProp>(Parent);
            }

            public IMereQueryPost<T> AndGroup(string whereSql, object whereValues = null)
            {
                QueryContext.AddFilterGroup(whereSql, whereValues);
                return this;
            }

            //end and filter groups
            public IMereQueryFilter<T, TProp> AndGroupInner<TProp>(Expression<Func<T, TProp>> prop)
            {
                QueryContext.InitFilterGroupAdd(prop, false, true);
                return new MereQueryBase<T, TProp>(Parent);
            }

            public IMereQueryPost<T> AndGroupInner(string whereSql, object whereValues = null)
            {
                QueryContext.AddFilterGroup(whereSql, whereValues, false, true);
                return this;
            }

            public IMereQueryFilter<T, TProp> AndGroupBackup<TProp>(Expression<Func<T, TProp>> prop, int backup = 1)
            {
                QueryContext.InitFilterGroupAdd(prop, false, true, backup);
                return new MereQueryBase<T, TProp>(Parent);
            }

            public IMereQueryPost<T> AndGroupBackup(string whereSql, object whereValues = null, int backup = 1)
            {
                QueryContext.AddFilterGroup(whereSql, whereValues, false, true, backup);
                return this;
            }

            //end ands 
            #endregion

            #region OrsAndOrGroups
            //ands
            /// <summary>
            /// Initiate adding an and filter by selecting the field to filter on
            /// </summary>
            /// <typeparam name="TProp">Selected property type</typeparam>
            /// <param name="fieldSelector">Lambda exepression to select field to filter on</param>
            /// <returns>QueryObject to add operator and value</returns>
            public IMereQueryFilter<T, TProp> Or<TProp>(Expression<Func<T, TProp>> fieldSelector)
            {
                QueryContext.InitFilterAdd(fieldSelector, true);
                return new MereQueryBase<T, TProp>(Parent);
            }

            /// <summary>
            /// Adding custom and filter/statement with hardcoded values or 
            /// pass in an object with properties named the same as you parameters minus the "@" sign.
            /// </summary>
            /// <param name="whereSql">Custom where filter/statement</param>
            /// <param name="whereValues">Parameter values for custom filter/statement</param>
            /// <returns></returns>
            public IMereQueryPost<T> Or(string whereSql, object whereValues = null)
            {
                QueryContext.AddFilter(whereSql, whereValues, true);
                return this;
            }

            //and filter groups
            public IMereQueryFilter<T, TProp> OrGroup<TProp>(Expression<Func<T, TProp>> prop)
            {
                QueryContext.InitFilterGroupAdd(prop, true);
                return new MereQueryBase<T, TProp>(Parent);
            }

            public IMereQueryPost<T> OrGroup(string whereSql, object whereValues = null)
            {
                QueryContext.AddFilterGroup(whereSql, whereValues, true);
                return this;
            }

            //end and filter groups
            public IMereQueryFilter<T, TProp> OrGroupInner<TProp>(Expression<Func<T, TProp>> prop)
            {
                QueryContext.InitFilterGroupAdd(prop, true, true);
                return new MereQueryBase<T, TProp>(Parent);
            }

            public IMereQueryPost<T> OrGroupInner(string whereSql, object whereValues = null)
            {
                QueryContext.AddFilterGroup(whereSql, whereValues, true, true);
                return this;
            }

            public IMereQueryFilter<T, TProp> OrGroupBackup<TProp>(Expression<Func<T, TProp>> prop, int backup = 1)
            {
                QueryContext.InitFilterGroupAdd(prop, true, true, backup);
                return new MereQueryBase<T, TProp>(Parent);
            }

            public IMereQueryPost<T> OrGroupBackup(string whereSql, object whereValues = null, int backup = 1)
            {
                QueryContext.AddFilterGroup(whereSql, whereValues, true, true, backup);
                return this;
            }

            //end ors 
            #endregion

            #endregion
        }

        private abstract class MereQueryBase : IMereQueryBase<T>
        {
            //protected MereContext<T> QueryContext { get; set; }
            public MereQueryBase(MereQuery<T> parent)
            {
                Parent = parent;
            }

            protected readonly MereQuery<T> Parent;
            public MereContext<T> QueryContext { get { return Parent._mereContext; } set { Parent._mereContext = value; } }

            public IMereQueryPost<T> AddFilter<TProp>(Expression<Func<T, TProp>> fieldSelector, SqlOperator sqlOperator,
                                                      TProp value)
            {
                return AddFilter(fieldSelector, sqlOperator, value, false);
            }
            
            public IMereQueryPost<T> AddFilter<TProp>(Expression<Func<T, TProp>> fieldSelector, SqlOperator sqlOperator,
                                                      TProp value, bool or)
            {
                QueryContext.InitFilterAdd(fieldSelector, or);
                QueryContext.AddFilter(value, sqlOperator);
                return Parent._post;
            }

            public IMereQueryPost<T> AddFilter(string fieldName, SqlOperator sqlOperator, object value)
            {
                return AddFilter(fieldName, sqlOperator, value, false);
            }

            public IMereQueryPost<T> AddFilter(string fieldName, SqlOperator sqlOperator, object value, bool or)
            {
                QueryContext.InitFilterAdd(fieldName, or);
                QueryContext.AddFilter(value, sqlOperator);
                return Parent._post;
            }

            //#region execute to generic

            //#region sync
            //public IEnumerable<TResult> Execute<TResult>()
            //{
            //    Parent.PreExecuteChecks();

            //    QueryContext.Connection.Open();
            //    using (var reader = new MereSqlDataReader<TResult>(QueryContext.Command.ExecuteReader(), QueryContext.SelectMereColumnsList))
            //    {
            //        while (reader.Read())
            //        {
            //            yield return reader;
            //        }
            //        QueryContext.Connection.Close();
            //    }
            //}

            //public List<TResult> ExecuteToList<TResult>(Func<T, TResult> selectFields)
            //{
            //    return ExecuteToList(selectFields(new T()));
            //}

            //public List<TResult> ExecuteToList<TResult>() where TResult : class
            //{
            //    var sw = Stopwatch.StartNew();
            //    long conOpen;
            //    long readDone;
            //    var selectMereColumnsBefore = QueryContext.SelectMereColumnsList;
            //    QueryContext.SelectMereColumnsList = MereUtils.CacheCheckMin<TResult>().SelectMereColumns;
            //    Parent.PreExecuteChecks();

            //    var toReturn = new List<TResult>();

            //    QueryContext.Connection.Open();
            //    conOpen = sw.ElapsedMilliseconds;
            //    using (var reader = new MereSqlDataReader<TResult>(QueryContext.Command.ExecuteReader(), QueryContext.SelectMereColumnsList))
            //    {
            //        while (reader.Read())
            //        {
            //            toReturn.Add(reader);
            //        }
            //        readDone = sw.ElapsedMilliseconds;
            //        Console.WriteLine("done reading {0} read time: {1}", readDone, readDone - conOpen);
            //        QueryContext.Connection.Close();
            //    }
            //    QueryContext.SelectMereColumnsList = selectMereColumnsBefore;
            //    return toReturn;
            //}

            //private List<TResult> ExecuteToList<TResult>(TResult instance)
            //{
            //    var sw = Stopwatch.StartNew();
            //    long conOpen;
            //    long readDone;
            //    var selectMereColumnsBefore = QueryContext.SelectMereColumnsList;
            //    QueryContext.SelectMereColumnsList = MereUtils.CacheCheckMin<TResult>().SelectMereColumns;
            //    Parent.PreExecuteChecks();

            //    var toReturn = new List<TResult>();
            //    var type = instance.GetType();
            //    QueryContext.Connection.Open();
            //    conOpen = sw.ElapsedMilliseconds;
            //    using (var reader = QueryContext.Command.ExecuteReader())
            //    {
            //        var properties = TypeDescriptor.GetProperties(instance);
            //        while (reader.Read())
            //        {
            //            var objIdx = 0;
            //            var objArray = new object[properties.Count];
            //            foreach (PropertyDescriptor info in properties)
            //                objArray[objIdx++] = reader[info.Name];
            //            toReturn.Add((TResult)Activator.CreateInstance(type, objArray));
            //        }
            //        readDone = sw.ElapsedMilliseconds;
            //        Console.WriteLine("done reading {0} read time: {1}", readDone, readDone - conOpen);
            //        QueryContext.Connection.Close();
            //    }
            //    QueryContext.SelectMereColumnsList = selectMereColumnsBefore;
            //    return toReturn;
            //}
            //#endregion

            //#region async
            //public async Task<List<TResult>> ExecuteAsync<TResult>()
            //{
            //    Parent.PreExecuteChecks();

            //    var toReturn = new List<TResult>();

            //    await QueryContext.Connection.OpenAsync();
            //    using (var reader = new MereSqlDataReader<TResult>(await QueryContext.Command.ExecuteReaderAsync(), QueryContext.SelectMereColumnsList))
            //    {
            //        while (await reader.ReadAsync())
            //        {
            //            toReturn.Add(reader);
            //        }
            //        QueryContext.Connection.Close();
            //    }
            //    return toReturn;
            //}

            //public List<TResult> ExecuteToList<TResult>(Func<T, TResult> selectFields)
            //{
            //    return ExecuteToList(selectFields(new T()));
            //}

            //public Task<List<TResult>> ExecuteToList<TResult>() where TResult : class
            //{
            //    var sw = Stopwatch.StartNew();
            //    long conOpen;
            //    long readDone;
            //    var selectMereColumnsBefore = QueryContext.SelectMereColumnsList;
            //    QueryContext.SelectMereColumnsList = MereUtils.CacheCheckMin<TResult>().SelectMereColumns;
            //    Parent.PreExecuteChecks();

            //    var toReturn = new List<TResult>();

            //    QueryContext.Connection.Open();
            //    conOpen = sw.ElapsedMilliseconds;
            //    using (var reader = new MereSqlDataReader<TResult>(QueryContext.Command.ExecuteReader(), QueryContext.SelectMereColumnsList))
            //    {
            //        while (reader.Read())
            //        {
            //            toReturn.Add(reader);
            //        }
            //        readDone = sw.ElapsedMilliseconds;
            //        Console.WriteLine("done reading {0} read time: {1}", readDone, readDone - conOpen);
            //        QueryContext.Connection.Close();
            //    }
            //    QueryContext.SelectMereColumnsList = selectMereColumnsBefore;
            //    return toReturn;
            //}

            //private List<TResult> ExecuteToList<TResult>(TResult instance)
            //{
            //    var sw = Stopwatch.StartNew();
            //    long conOpen;
            //    long readDone;
            //    var selectMereColumnsBefore = QueryContext.SelectMereColumnsList;
            //    QueryContext.SelectMereColumnsList = MereUtils.CacheCheckMin<TResult>().SelectMereColumns;
            //    Parent.PreExecuteChecks();

            //    var toReturn = new List<TResult>();
            //    var type = instance.GetType();
            //    QueryContext.Connection.Open();
            //    conOpen = sw.ElapsedMilliseconds;
            //    using (var reader = QueryContext.Command.ExecuteReader())
            //    {
            //        var properties = TypeDescriptor.GetProperties(instance);
            //        while (reader.Read())
            //        {
            //            var objIdx = 0;
            //            var objArray = new object[properties.Count];
            //            foreach (PropertyDescriptor info in properties)
            //                objArray[objIdx++] = reader[info.Name];
            //            toReturn.Add((TResult)Activator.CreateInstance(type, objArray));
            //        }
            //        readDone = sw.ElapsedMilliseconds;
            //        Console.WriteLine("done reading {0} read time: {1}", readDone, readDone - conOpen);
            //        QueryContext.Connection.Close();
            //    }
            //    QueryContext.SelectMereColumnsList = selectMereColumnsBefore;
            //    return toReturn;
            //}
            //#endregion
            //#endregion//end execute to generic

            #region sync executes
            public IEnumerable<T> Execute()
            {
                return QueryContext.ExecuteQuery();
            }

            public List<T> ExecuteToList()
            {
                return QueryContext.ExecuteQueryToList();
            }

            public List<IDataRecord> ExecuteToIDataRecordList()
            {
                return QueryContext.ExecuteQueryToIDataRecordList();
            }

            public T ExecuteFirstOrDefault()
            {
                return QueryContext.ExecuteQueryFirstOrDefault();
            }

            #region ExecuteExpando
            public IEnumerable<dynamic> ExecuteDynamic()
            {
                return QueryContext.ExecuteQueryDynamic();
            }

            public IEnumerable<dynamic> ExecuteDynamic<TFields>(Func<T, TFields> selectFields)
            {
                return QueryContext.ExecuteQueryDynamic(selectFields);
            }

            public IEnumerable<dynamic> ExecuteDynamic(object selectFields)
            {
                return QueryContext.ExecuteQueryDynamic(selectFields);
            }

            public List<dynamic> ExecuteDynamicToList()
            {
                return QueryContext.ExecuteQueryDynamicToList();
            }

            public List<dynamic> ExecuteDynamicToList<TFields>(Func<T, TFields> selectFields)
            {
                return QueryContext.ExecuteQueryDynamicToList(selectFields);
            }

            public List<dynamic> ExecuteDynamicToList(object selectFields)
            {
                return QueryContext.ExecuteQueryDynamicToList(selectFields);
            }

            public IEnumerable<ExpandoObject> ExecuteExpando()
            {
                return QueryContext.ExecuteQueryExpando();
            }

            public IEnumerable<ExpandoObject> ExecuteExpando<TFields>(Func<T, TFields> selectFields)
            {
                return QueryContext.ExecuteQueryExpando(selectFields);
            }

            public IEnumerable<ExpandoObject> ExecuteExpando(object selectFields)
            {
                return QueryContext.ExecuteQueryExpando(selectFields);
            }

            public List<ExpandoObject> ExecuteExpandoToList()
            {
                return QueryContext.ExecuteQueryExpandoToList();
            }

            public List<ExpandoObject> ExecuteExpandoToList<TFields>(Func<T, TFields> selectFields)
            {
                return QueryContext.ExecuteQueryExpandoToList(selectFields);
            }

            public List<ExpandoObject> ExecuteExpandoToList(object selectFields)
            {
                return QueryContext.ExecuteQueryExpandoToList(selectFields);
            }
            #endregion

            #region ExecuteCustomQuery
            public List<dynamic> ExecuteCustomQueryDynamicToList(string customQuery)
            {
                return QueryContext.ExecuteQueryCustomQueryDynamicToList(customQuery);
            }

            public List<dynamic> ExecuteCustomQueryDynamicToList(string customQuery, object parameters)
            {
                return QueryContext.ExecuteQueryCustomQueryDynamicToList(customQuery, parameters);
            }

            public IEnumerable<dynamic> ExecuteCustomQueryDynamic(string customQuery)
            {
                return QueryContext.ExecuteQueryCustomQueryDynamic(customQuery);
            }

            public IEnumerable<dynamic> ExecuteCustomQueryDynamic(string customQuery, object parameters)
            {
                return QueryContext.ExecuteQueryCustomQueryDynamic(customQuery, parameters);
            }

            public List<T> ExecuteCustomQueryToList(string customQuery)
            {
                return QueryContext.ExecuteQueryCustomQueryToList(customQuery);
            }

            public List<T> ExecuteCustomQueryToList(string customQuery, object parameters)
            {
                return QueryContext.ExecuteQueryCustomQueryToList(customQuery, parameters);
            }

            public IEnumerable<T> ExecuteCustomQuery(string customQuery)
            {
                return QueryContext.ExecuteQueryCustomQuery(customQuery);
            }

            public IEnumerable<T> ExecuteCustomQuery(string customQuery, object parameters)
            {
                return QueryContext.ExecuteQueryCustomQuery(customQuery, parameters);
            }
            #endregion

            #region ExecuteDistinct
            public IEnumerable<T> ExecuteDistinct()
            {
                return QueryContext.ExecuteQueryDistinct();
            }

            public IEnumerable<object> ExecuteDistinct<TFields>(Func<T, TFields> fields)
            {
                return QueryContext.ExecuteQueryDistinct(fields);
            }

            public int ExecuteDistinctCount()
            {
                return QueryContext.ExecuteDistinctCount();
            }

            public int ExecuteDistinctCount<TFields>(Func<T, TFields> fields)
            {
                return QueryContext.ExecuteDistinctCount(fields);
            }

            public int ExecuteCount()
            {
                return QueryContext.ExecuteCount();
            }
            #endregion
            #endregion

            #region async executes

            public Task<List<T>> ExecuteAsync()
            {
                return QueryContext.ExecuteQueryAsync();
            }

            public Task<T> ExecuteFirstOrDefaultAsync()
            {
                return QueryContext.ExecuteQueryFirstOrDefaultAsync();
            }

            #region ExecuteExpando
            public Task<List<dynamic>> ExecuteDynamicAsync()
            {
                return QueryContext.ExecuteQueryDynamicAsync();
            }

            public Task<List<dynamic>> ExecuteDynamicAsync<TFields>(Func<T, TFields> selectFields)
            {
                return QueryContext.ExecuteQueryDynamicAsync(selectFields);
            }

            public Task<List<dynamic>> ExecuteDynamicAsync(object selectFields)
            {
                return QueryContext.ExecuteQueryDynamicAsync(selectFields);
            }

            public Task<List<ExpandoObject>> ExecuteExpandoAsync()
            {
                return QueryContext.ExecuteQueryExpandoAsync();
            }

            public Task<List<ExpandoObject>> ExecuteExpandoAsync<TFields>(Func<T, TFields> selectFields)
            {
                return QueryContext.ExecuteQueryExpandoAsync(selectFields);
            }

            public Task<List<ExpandoObject>> ExecuteExpandoAsync(object selectFields)
            {
                return QueryContext.ExecuteQueryExpandoAsync(selectFields);
            }
            #endregion

            #region ExecuteCustomQuery
            public Task<List<ExpandoObject>> ExecuteCustomQueryExpandoAsync(string customQuery)
            {
                return QueryContext.ExecuteQueryCustomQueryExpandoAsync(customQuery);
            }

            public Task<List<ExpandoObject>> ExecuteCustomQueryExpandoAsync(string customQuery, object parameters)
            {
                return QueryContext.ExecuteQueryCustomQueryExpandoAsync(customQuery, parameters);
            }

            public Task<List<dynamic>> ExecuteCustomQueryDynamicAsync(string customQuery)
            {
                return QueryContext.ExecuteQueryCustomQueryDynamicAsync(customQuery);
            }

            public Task<List<dynamic>> ExecuteCustomQueryDynamicAsync(string customQuery, object parameters)
            {
                return QueryContext.ExecuteQueryCustomQueryDynamicAsync(customQuery, parameters);
            }

            public Task<List<T>> ExecuteCustomQueryAsync(string customQuery)
            {
                return QueryContext.ExecuteQueryCustomQueryAsync(customQuery);
            }

            public Task<List<T>> ExecuteCustomQueryAsync(string customQuery, object parameters)
            {
                return QueryContext.ExecuteQueryCustomQueryAsync(customQuery, parameters);
            }
            #endregion

            #region ExecuteDistinct
            public Task<List<T>> ExecuteDistinctAsync()
            {
                return QueryContext.ExecuteQueryDistinctAsync();
            }

            public Task<List<dynamic>> ExecuteDistinctAsync<TFields>(Func<T, TFields> fields)
            {
                return QueryContext.ExecuteQueryDistinctAsync(fields);
            }

            public Task<int> ExecuteDistinctCountAsync()
            {
                return QueryContext.ExecuteDistinctCountAsync();
            }

            public Task<int> ExecuteDistinctCountAsync<TFields>(Func<T, TFields> fields)
            {
                return QueryContext.ExecuteDistinctCountAsync(fields);
            }

            public Task<int> ExecuteCountAsync()
            {
                return QueryContext.ExecuteCountAsync();
            }
            #endregion

            #region ExecuteSimpleQuery
            //public Task<IEnumerable<T>> ExecuteSimpleQueryAsync<T, TFilterObject>(TFilterObject filterObject)
            //{
            //    var q = MereQuery<T>.Create(new List<object> { filterObject });
            //    return q.ExecuteAsync();
            //}

            //public static Task<IEnumerable<T>> ExecuteSimpleQueryAsync<T>(this MereQuery<T> mereQuery, int top)
            //{
            //    var q = MereQuery<T>.Create(top);
            //    return q.ExecuteAsync();
            //}

            //public static Task<IEnumerable<T>> ExecuteSimpleQueryAsync<T>(this MereQuery<T> mereQuery, int top, object filterObject)
            //{
            //    var q = MereQuery<T>.Create(new List<object> { filterObject }, top);
            //    return q.ExecuteAsync();
            //}

            //public static Task<IEnumerable<T>> ExecuteSimpleQueryAsync<T>(this MereQuery<T> mereQuery, string whereSql, object whereObject)
            //{
            //    var q = MereQuery<T>.Create();
            //    q.Where(whereSql, whereObject);
            //    return q.ExecuteAsync();
            //}

            //public static Task<IEnumerable<T>> ExecuteSimpleQueryAsync<T>(this MereQuery<T> mereQuery, int top, string whereSql, object whereObject)
            //{
            //    var q = MereQuery<T>.Create(top);
            //    q.Where(whereSql, whereObject);
            //    return q.ExecuteAsync();
            //}
            #endregion

            #region BulkHelpers
            public Task<int> BulkCopyToAsync<TDest>(int batchSize = 1000) where TDest : new()
            {
                return QueryContext.BulkCopyToAsync<TDest>(batchSize);
            }

            public Task<List<T>> BulkCopyToChunkedParallel<TDest>(int minChunkSize, int maxChunkSize, int threadThrottle = 150, int timeout = 0) where TDest : new()
            {
                return QueryContext.BulkCopyToChunkedParallel<TDest>(minChunkSize, maxChunkSize, threadThrottle, timeout);
            }
            #endregion

            #region ExecuteChunkedParallel
            public Task<List<T>> ExecuteChunkedParallel(Action<int, List<T>> perChunk, int minChunkSize,
                                                               int maxChunkSize = 0, int threadThrottle = 150)
            {
                return QueryContext.ExecuteQueryChunkedParallel(perChunk,  minChunkSize, maxChunkSize, threadThrottle);
            }

            public Task<List<T>> ExecuteChunkedParallel(Func<int, List<T>, List<T>> perChunk, int minChunkSize,
                                                               int maxChunkSize = 0, int threadThrottle = 150)
            {
                return QueryContext.ExecuteQueryChunkedParallel(perChunk, minChunkSize, maxChunkSize, threadThrottle);
            }

            public Task<List<T>> ExecuteChunkedParallel(Action<List<T>> perChunk, int minChunkSize,
                                                               int maxChunkSize = 0, int threadThrottle = 150)
            {
                return QueryContext.ExecuteQueryChunkedParallel(perChunk, minChunkSize, maxChunkSize, threadThrottle);
            }

            public Task<List<T>> ExecuteChunkedParallel(Func<List<T>, List<T>> perChunk, int minChunkSize,
                                                               int maxChunkSize = 0, int threadThrottle = 150)
            {
                return QueryContext.ExecuteQueryChunkedParallel(perChunk, minChunkSize, maxChunkSize, threadThrottle);
            }
            #endregion
            #endregion

            public void Dispose()
            {
                QueryContext.Dispose();
            }
        }
    }
}
