using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Mere.Interfaces;

namespace Mere
{
    public class MereUpdate
    {
        protected MereUpdate() { }

        public static IMereUpdatePre<T> Create<T>() where T : new()
        {
            return MereUpdate<T>.Create();
        }
        public static IMereUpdatePre<T> Create<T>(MereDataSource dataSource) where T : new()
        {
            return MereUpdate<T>.Create(dataSource);
        }
    }

    public class MereUpdate<T> where T : new()
    {

        private MereUpdate()
        {
            _mereContext = new MereContext<T>(MereContextType.UpdateWithUpdateFields);
            _pre = new MereUpdatePre(this);
            _post = new MereUpdatePost(this);
        }

        private MereUpdate(MereDataSource dataSource)
        {
            _mereContext = new MereContext<T>(MereContextType.UpdateWithUpdateFields, dataSource);
            _pre = new MereUpdatePre(this);
            _post = new MereUpdatePost(this);
        }

        public static IMereUpdatePre<T> Create()
        {
            var parent = new MereUpdate<T>();
            return parent._pre;
        }

        public static IMereUpdatePre<T> Create(MereDataSource dataSource)
        {
            var parent = new MereUpdate<T>(dataSource);
            return parent._pre;
        }


        private MereContext<T> _mereContext;
        private readonly IMereUpdatePre<T> _pre;
        private readonly IMereUpdatePost<T> _post;

        #region methods

        private void PreExecuteChecks()
        {
            UpdateConnection();
            UpdateCommand();
        }

        private void UpdateConnection()
        {
            var ds = _mereContext.CurMereDataSource[MereUtils.MereEnvironment];
            _mereContext.ServerName = ds.ServerName;
            _mereContext.DatabaseName = ds.DatabaseName;
            _mereContext.TableName = ds.TableName;
            _mereContext.UserId = ds.UserId;
            _mereContext.Password = ds.Password;
            _mereContext.Connection.ConnectionString = _mereContext.ConnectionString;
        }

        private void UpdateCommand()
        {
            _mereContext.Command.CommandText = _mereContext.SqlUpdateUsingUpdateFields;
            _mereContext.Command.CommandTimeout = _mereContext.Timeout;
        }

        #endregion

        private class MereUpdateBase<T, TProp> : IMereUpdateBase<T, TProp> where T : new()
        {

            public MereUpdateBase(MereUpdate<T> parent)
            {
                _parent = parent;
            }

            private readonly MereUpdate<T> _parent;

            private MereContext<T> QueryContext
            {
                get { return _parent._mereContext; }
            }

            #region Creates

            public static IMereUpdateBase<T, TProp> Create(MereUpdate<T> parent)
            {
                return new MereUpdateBase<T, TProp>(parent);
            }

            #endregion

            #region filter methods

            public IMereUpdatePost<T> EqualTo(TProp value)
            {
                _parent._mereContext.AddFilter(value, SqlOperator.EqualTo);
                return _parent._post;
            }

            public IMereUpdatePost<T> EqualToCaseSensitive(TProp value)
            {
                QueryContext.AddFilter(value, SqlOperator.EqualToCaseSensitive);
                return _parent._post;
            }

            public IMereUpdatePost<T> NotEqualTo(TProp value)
            {
                QueryContext.AddFilter(value, SqlOperator.NotEqualTo);
                return _parent._post;
            }

            public IMereUpdatePost<T> NotEqualToCaseSensitive(TProp value)
            {
                QueryContext.AddFilter(value, SqlOperator.NotEqualToCaseSensitive);
                return _parent._post;
            }

            public IMereUpdatePost<T> GreaterThan(TProp value)
            {

                QueryContext.AddFilter(value, SqlOperator.GreaterThan);
                return _parent._post;
            }

            public IMereUpdatePost<T> GreaterThanOrEqualTo(TProp value)
            {
                QueryContext.AddFilter(value, SqlOperator.GreaterThanOrEqualTo);
                return _parent._post;
            }

            public IMereUpdatePost<T> LessThan(TProp value)
            {
                QueryContext.AddFilter(value, SqlOperator.LessThan);
                return _parent._post;
            }

            public IMereUpdatePost<T> LessThanOrEqualTo(TProp value)
            {
                QueryContext.AddFilter(value, SqlOperator.LessThanOrEqualTo);
                return _parent._post;
            }

            public IMereUpdatePost<T> In<TVal>(TVal values) where TVal : IEnumerable<TProp>
            {
                QueryContext.AddFilterIn(values);
                return _parent._post;
            }

            public IMereUpdatePost<T> In(params TProp[] values)
            {
                QueryContext.AddFilterIn(values);
                return _parent._post;
            }

            public IMereUpdatePost<T> InCaseSensitive<TVal>(TVal values) where TVal : IEnumerable<TProp>
            {
                QueryContext.AddFilterInCaseSensitive(values);
                return _parent._post;
            }

            public IMereUpdatePost<T> InCaseSensitive(params TProp[] values)
            {
                QueryContext.AddFilterInCaseSensitive(values);
                return _parent._post;
            }

            public IMereUpdatePost<T> NotIn<TVal>(TVal values) where TVal : IEnumerable<TProp>
            {
                QueryContext.AddFilterNotIn(values);
                return _parent._post;
            }

            public IMereUpdatePost<T> NotIn(params TProp[] values)
            {
                QueryContext.AddFilterNotIn(values);
                return _parent._post;
            }

            public IMereUpdatePost<T> NotInCaseSensitive<TVal>(TVal values) where TVal : IEnumerable<TProp>
            {
                QueryContext.AddFilterNotInCaseSensitive(values);
                return _parent._post;
            }

            public IMereUpdatePost<T> NotInCaseSensitive(params TProp[] values)
            {
                QueryContext.AddFilterNotInCaseSensitive(values);
                return _parent._post;
            }

            public IMereUpdatePost<T> Between(TProp value1, TProp value2)
            {
                QueryContext.AddFilterBetween(value1, value2);
                return _parent._post;
            }

            public IMereUpdatePost<T> NotBetween(TProp value1, TProp value2)
            {
                QueryContext.AddFilterNotBetween(value1, value2);
                return _parent._post;
            }

            public IMereUpdatePost<T> Contains(TProp value)
            {
                var v = "%" + value + "%";
                QueryContext.AddFilter(v, SqlOperator.Like);
                return _parent._post;
            }

            public IMereUpdatePost<T> NotContains(TProp value)
            {
                var v = "%" + value + "%";
                QueryContext.AddFilter(v, SqlOperator.NotLike);
                return _parent._post;
            }

            public IMereUpdatePost<T> StartsWith(TProp value)
            {
                var v = value + "%";

                QueryContext.AddFilter(v, SqlOperator.Like);
                return _parent._post;
            }

            public IMereUpdatePost<T> NotStartsWith(TProp value)
            {
                var v = value + "%";

                QueryContext.AddFilter(v, SqlOperator.NotLike);
                return _parent._post;
            }

            public IMereUpdatePost<T> EndsWith(TProp value)
            {
                var v = "%" + value;

                QueryContext.AddFilter(v, SqlOperator.Like);
                return _parent._post;
            }

            public IMereUpdatePost<T> NotEndsWith(TProp value)
            {
                var v = "%" + value;

                QueryContext.AddFilter(v, SqlOperator.NotLike);
                return _parent._post;
            }

            public IMereUpdatePost<T> ContainsCaseSensitive(TProp value)
            {
                var v = "%" + value + "%";
                QueryContext.AddFilter(v, SqlOperator.LikeCaseSensitive);
                return _parent._post;
            }

            public IMereUpdatePost<T> NotContainsCaseSensitive(TProp value)
            {
                var v = "%" + value + "%";
                QueryContext.AddFilter(v, SqlOperator.NotLikeCaseSensitive);
                return _parent._post;
            }

            public IMereUpdatePost<T> StartsWithCaseSensitive(TProp value)
            {
                var v = value + "%";

                QueryContext.AddFilter(v, SqlOperator.LikeCaseSensitive);
                return _parent._post;
            }

            public IMereUpdatePost<T> NotStartsWithCaseSensitive(TProp value)
            {
                var v = value + "%";

                QueryContext.AddFilter(v, SqlOperator.NotLikeCaseSensitive);
                return _parent._post;
            }

            public IMereUpdatePost<T> EndsWithCaseSensitive(TProp value)
            {
                var v = "%" + value;

                QueryContext.AddFilter(v, SqlOperator.LikeCaseSensitive);
                return _parent._post;
            }

            public IMereUpdatePost<T> NotEndsWithCaseSensitive(TProp value)
            {
                var v = "%" + value;

                QueryContext.AddFilter(v, SqlOperator.NotLikeCaseSensitive);
                return _parent._post;
            }

            #endregion
        }

        private class MereUpdatePre : MereUpdateBase, IMereUpdatePre<T>
        {

            public MereUpdatePre(MereUpdate<T> parent) : base(parent) { }

            #region methods
            /// <summary>
            /// Adds set for field
            /// </summary>
            /// <param name="field"></param>
            /// <param name="value"></param>
            /// <returns></returns>
            public IMereUpdatePre<T> Set<TProp>(Expression<Func<T, TProp>> field, TProp value)
            {
                QueryContext.AddUpdateField(field, value);
                return this;
            }

            /// <summary>
            /// Adds sets for all supplied fields ignoring identity fields and filters for identity and/or key
            /// </summary>
            /// <param name="fields"></param>
            /// <returns></returns>
            public IMereUpdatePre<T> Set(IEnumerable<KeyValuePair<string, object>> fields)
            {
                QueryContext.AddUpdateField(fields);

                return this;
            }

            /// <summary>
            /// Adds sets for all supplied fields ignoring identity fields with option to or not to add filters for identity and/or key
            /// </summary>
            /// <param name="fields"></param>
            /// <param name="filterWithKeyAndOrIdentity"></param>
            /// <returns></returns>
            public IMereUpdatePre<T> Set(IEnumerable<KeyValuePair<string, object>> fields, bool filterWithKeyAndOrIdentity)
            {
                QueryContext.AddUpdateField(fields, filterWithKeyAndOrIdentity);

                return this;
            }

            public IMereUpdatePre<T> SetDatabase(string databaseName)
            {
                QueryContext.DatabaseName = databaseName;
                return this;
            }

            public IMereUpdatePre<T> SetServer(string serverName)
            {
                QueryContext.ServerName = serverName;
                return this;
            }

            public IMereUpdatePre<T> SetTable(string tableName)
            {
                QueryContext.TableName = tableName;
                return this;
            }

            public IMereUpdatePre<T> SetUserId(string userId)
            {
                QueryContext.UserId = userId;
                return this;
            }

            public IMereUpdatePre<T> SetPassword(string password)
            {
                QueryContext.Password = password;
                return this;
            }
            #endregion

            #region filter methods

            public IMereUpdateBase<T, TProp> Where<TProp>(Expression<Func<T, TProp>> fieldSelector)
            {
                QueryContext.InitFilterAdd(fieldSelector);
                return new MereUpdateBase<T, TProp>(Parent);
            }

            public IMereUpdatePost<T> Where(string whereSql, object whereValues = null)
            {
                QueryContext.CurFilterOr = false;
                QueryContext.AddFilter(whereSql, whereValues);
                return Parent._post;
            }

            #endregion
        }

        private class MereUpdatePost : MereUpdateBase, IMereUpdatePost<T>
        {
            public MereUpdatePost(MereUpdate<T> parent) : base(parent)
            {
            }

            #region methods
            /// <summary>
            /// Adds set for field
            /// </summary>
            /// <param name="field"></param>
            /// <param name="value"></param>
            /// <returns></returns>
            public IMereUpdatePost<T> Set<TProp>(Expression<Func<T, TProp>> field, TProp value)
            {
                QueryContext.AddUpdateField(field, value);
                return this;
            }

            /// <summary>
            /// Adds sets for all supplied fields ignoring identity fields and filters for identity and/or key
            /// </summary>
            /// <param name="fields"></param>
            /// <returns></returns>
            public IMereUpdatePost<T> Set(IEnumerable<KeyValuePair<string, object>> fields)
            {
                QueryContext.AddUpdateField(fields);

                return this;
            }

            /// <summary>
            /// Adds sets for all supplied fields ignoring identity fields with option to or not to add filters for identity and/or key
            /// </summary>
            /// <param name="fields"></param>
            /// <param name="filterWithKeyAndOrIdentity"></param>
            /// <returns></returns>
            public IMereUpdatePost<T> Set(IEnumerable<KeyValuePair<string, object>> fields, bool filterWithKeyAndOrIdentity)
            {
                QueryContext.AddUpdateField(fields, filterWithKeyAndOrIdentity);

                return this;
            }

            public IMereUpdatePost<T> SetDatabase(string databaseName)
            {
                QueryContext.DatabaseName = databaseName;
                return this;
            }

            public IMereUpdatePost<T> SetServer(string serverName)
            {
                QueryContext.ServerName = serverName;
                return this;
            }

            public IMereUpdatePost<T> SetTable(string tableName)
            {
                QueryContext.TableName = tableName;
                return this;
            }

            public IMereUpdatePost<T> SetUserId(string userId)
            {
                QueryContext.UserId = userId;
                return this;
            }

            public IMereUpdatePost<T> SetPassword(string password)
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
            public IMereUpdateBase<T, TProp> And<TProp>(Expression<Func<T, TProp>> fieldSelector)
            {
                QueryContext.InitFilterAdd(fieldSelector);
                return new MereUpdateBase<T, TProp>(Parent);
            }

            /// <summary>
            /// Adding custom and filter/statement with hardcoded values or 
            /// pass in an object with properties named the same as you parameters minus the "@" sign.
            /// </summary>
            /// <param name="whereSql">Custom where filter/statement</param>
            /// <param name="whereValues">Parameter values for custom filter/statement</param>
            /// <returns></returns>
            public IMereUpdatePost<T> And(string whereSql, object whereValues = null)
            {
                QueryContext.CurFilterOr = false;
                QueryContext.AddFilter(whereSql, whereValues);
                return this;
            }

            //and filter groups
            public IMereUpdateBase<T, TProp> AndGroup<TProp>(Expression<Func<T, TProp>> prop)
            {
                QueryContext.InitFilterGroupAdd(prop);
                return new MereUpdateBase<T, TProp>(Parent);
            }

            public IMereUpdatePost<T> AndGroup(string whereSql, object whereValues = null)
            {
                QueryContext.AddFilterGroup(whereSql, whereValues);
                return this;
            }

            //end and filter groups
            public IMereUpdateBase<T, TProp> AndGroupInner<TProp>(Expression<Func<T, TProp>> prop)
            {
                QueryContext.InitFilterGroupAdd(prop, false, true);
                return new MereUpdateBase<T, TProp>(Parent);
            }

            public IMereUpdatePost<T> AndGroupInner(string whereSql, object whereValues = null)
            {
                QueryContext.AddFilterGroup(whereSql, whereValues, false, true);
                return this;
            }

            public IMereUpdateBase<T, TProp> AndGroupBackup<TProp>(Expression<Func<T, TProp>> prop, int backup = 1)
            {
                QueryContext.InitFilterGroupAdd(prop, false, true, backup);
                return new MereUpdateBase<T, TProp>(Parent);
            }

            public IMereUpdatePost<T> AndGroupBackup(string whereSql, object whereValues = null, int backup = 1)
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
            public IMereUpdateBase<T, TProp> Or<TProp>(Expression<Func<T, TProp>> fieldSelector)
            {
                QueryContext.InitFilterAdd(fieldSelector, true);
                return new MereUpdateBase<T, TProp>(Parent);
            }

            /// <summary>
            /// Adding custom and filter/statement with hardcoded values or 
            /// pass in an object with properties named the same as you parameters minus the "@" sign.
            /// </summary>
            /// <param name="whereSql">Custom where filter/statement</param>
            /// <param name="whereValues">Parameter values for custom filter/statement</param>
            /// <returns></returns>
            public IMereUpdatePost<T> Or(string whereSql, object whereValues = null)
            {
                QueryContext.AddFilter(whereSql, whereValues, true);
                return this;
            }

            //and filter groups
            public IMereUpdateBase<T, TProp> OrGroup<TProp>(Expression<Func<T, TProp>> prop)
            {
                QueryContext.InitFilterGroupAdd(prop, true);
                return new MereUpdateBase<T, TProp>(Parent);
            }

            public IMereUpdatePost<T> OrGroup(string whereSql, object whereValues = null)
            {
                QueryContext.AddFilterGroup(whereSql, whereValues, true);
                return this;
            }

            //end and filter groups
            public IMereUpdateBase<T, TProp> OrGroupInner<TProp>(Expression<Func<T, TProp>> prop)
            {
                QueryContext.InitFilterGroupAdd(prop, true, true);
                return new MereUpdateBase<T, TProp>(Parent);
            }

            public IMereUpdatePost<T> OrGroupInner(string whereSql, object whereValues = null)
            {
                QueryContext.AddFilterGroup(whereSql, whereValues, true, true);
                return this;
            }

            public IMereUpdateBase<T, TProp> OrGroupBackup<TProp>(Expression<Func<T, TProp>> prop, int backup = 1)
            {
                QueryContext.InitFilterGroupAdd(prop, true, true, backup);
                return new MereUpdateBase<T, TProp>(Parent);
            }

            public IMereUpdatePost<T> OrGroupBackup(string whereSql, object whereValues = null, int backup = 1)
            {
                QueryContext.AddFilterGroup(whereSql, whereValues, true, true, backup);
                return this;
            }

            //end ors 

            #endregion

            #endregion

        }

        private abstract class MereUpdateBase : IMereUpdateBase<T>
        {
            //protected MereUpdateContext<T> QueryContext { get; set; }
            public MereUpdateBase(MereUpdate<T> parent)
            {
                Parent = parent;
            }

            protected readonly MereUpdate<T> Parent;

            public MereContext<T> QueryContext { get { return Parent._mereContext; } set { Parent._mereContext = value; } }

            public IMereUpdatePost<T> AddFilter<TProp>(Expression<Func<T, TProp>> fieldSelector, SqlOperator sqlOperator,
                                                    TProp value)
            {
                return AddFilter(fieldSelector, sqlOperator, value, false);
            }

            public IMereUpdatePost<T> AddFilter<TProp>(Expression<Func<T, TProp>> fieldSelector, SqlOperator sqlOperator,
                                                      TProp value, bool or)
            {
                QueryContext.InitFilterAdd(fieldSelector, or);
                QueryContext.AddFilter(value, sqlOperator);
                return Parent._post;
            }

            public IMereUpdatePost<T> AddFilter(string fieldName, SqlOperator sqlOperator, object value)
            {
                return AddFilter(fieldName, sqlOperator, value, false);
            }

            public IMereUpdatePost<T> AddFilter(string fieldName, SqlOperator sqlOperator, object value, bool or)
            {
                QueryContext.InitFilterAdd(fieldName, or);
                QueryContext.AddFilter(value, sqlOperator);
                return Parent._post;
            }

            #region sync executes
            public int Execute()
            {
                return QueryContext.ExecuteUpdate();
            }

            public int Execute(T newValues)
            {
                return QueryContext.ExecuteUpdate(newValues);
            }

            public int Execute(ExpandoObject newValues)
            {
                return QueryContext.ExecuteUpdate((IDictionary<string, object>)newValues);
            }

            /// <summary>
            /// Executes update using supplied fields adding filters for key and/or identity fields
            /// </summary>
            /// <param name="newValues"></param>
            /// <returns></returns>
            public int Execute(IEnumerable<KeyValuePair<string, object>> newValues)
            {
                return QueryContext.ExecuteUpdate(newValues, true);
            }

            /// <summary>
            /// Executes update using supplied fields with option for adding filters for key and/or identity fields
            /// </summary>
            /// <param name="newValues"></param>
            /// <param name="useKeyAndOrIdentity"></param>
            /// <returns></returns>
            public int Execute(IEnumerable<KeyValuePair<string, object>> newValues, bool useKeyAndOrIdentity)
            {
                return QueryContext.ExecuteUpdate(newValues, useKeyAndOrIdentity);
            }
            #endregion

            #region async executes
            public Task<int> ExecuteAsync()
            {
                return QueryContext.ExecuteUpdateAsync();
            }

            public Task<int> ExecuteAsync(T newValues)
            {
                return QueryContext.ExecuteUpdateAsync(newValues);
            }

            public Task<int> ExecuteAsync(ExpandoObject newValues)
            {
                return QueryContext.ExecuteUpdateAsync((IDictionary<string, object>)newValues);
            }

            /// <summary>
            /// Executes update using supplied fields adding filters for key and/or identity fields
            /// </summary>
            /// <param name="newValues"></param>
            /// <returns></returns>
            public Task<int> ExecuteAsync(IEnumerable<KeyValuePair<string, object>> newValues)
            {
                return QueryContext.ExecuteUpdateAsync(newValues, true);
            }


            /// <summary>
            /// Executes update using supplied fields with option for adding filters for key and/or identity fields
            /// </summary>
            /// <param name="newValues"></param>
            /// <param name="useKeyAndOrIdentity"></param>
            /// <returns></returns>
            public Task<int> ExecuteAsync(IEnumerable<KeyValuePair<string, object>> newValues, bool useKeyAndOrIdentity)
            {
                return QueryContext.ExecuteUpdateAsync(newValues, useKeyAndOrIdentity);
            }
            #endregion

            #region old executes
            //#region sync executes
            //public int Execute()
            //{
            //    Parent.PreExecuteChecks();

            //    foreach (var param in QueryContext.UpdateFieldsDictionary)
            //    {
            //        QueryContext.Command.Parameters.AddWithValue("@" + param.Key, param.Value ?? DBNull.Value);
            //    }

            //    QueryContext.Connection.Open();
            //    var toReturn = QueryContext.Command.ExecuteNonQuery();
            //    QueryContext.Connection.Close();
            //    return toReturn;
            //}

            //public int Execute(T newValues)
            //{
            //    Parent.PreExecuteChecks();

            //    foreach (var mereColumn in QueryContext.CurMereTableMin.SelectMereColumnsNoIdentity())
            //    {
            //        QueryContext.Command.Parameters.AddWithValue("@" + mereColumn.ColumnName, mereColumn.Get(newValues) ?? DBNull.Value);
            //    }

            //    QueryContext.Command.CommandText = QueryContext.SqlUpdate;

            //    QueryContext.Connection.Open();
            //    var toReturn = QueryContext.Command.ExecuteNonQuery();
            //    QueryContext.Connection.Close();
            //    QueryContext.Command.CommandText = QueryContext.SqlUpdateUsingUpdateFields;
            //    return toReturn;
            //}

            //public int Execute(ExpandoObject newValues)
            //{
            //    return Execute((IDictionary<string, object>)newValues);
            //}

            ///// <summary>
            ///// Executes update using supplied fields adding filters for key and/or identity fields
            ///// </summary>
            ///// <param name="newValues"></param>
            ///// <returns></returns>
            //public int Execute(IEnumerable<KeyValuePair<string, object>> newValues)
            //{
            //    return Execute(newValues, true);
            //}

            ///// <summary>
            ///// Executes update using supplied fields with option for adding filters for key and/or identity fields
            ///// </summary>
            ///// <param name="newValues"></param>
            ///// <param name="useKeyAndOrIdentity"></param>
            ///// <returns></returns>
            //public int Execute(IEnumerable<KeyValuePair<string, object>> newValues, bool useKeyAndOrIdentity)
            //{
            //    QueryContext.UpdateFieldsDictionary.Clear();
            //    QueryContext.AddUpdateField(newValues, useKeyAndOrIdentity);

            //    //make sure prechecks are done after changing update fields 
            //    //Parent.PreExecuteChecks();

            //    //foreach (var mereColumn in QueryContext.CurMereTableMin.SelectMereColumnsNoIdentity())
            //    //{
            //    //    QueryContext.Command.Parameters.AddWithValue("@" + mereColumn.ColumnName, mereColumn.Get(newValues));
            //    //}

            //    //QueryContext.Connection.Open();
            //    //var toReturn = QueryContext.Command.ExecuteNonQuery();
            //    //QueryContext.Connection.Close();
            //    //return toReturn;
            //    return Execute();
            //}
            //#endregion

            //#region async executes
            //public async Task<int> ExecuteAsync()
            //{
            //    Parent.PreExecuteChecks();

            //    foreach (var param in QueryContext.UpdateFieldsDictionary)
            //    {
            //        QueryContext.Command.Parameters.AddWithValue("@" + param.Key, param.Value ?? DBNull.Value);
            //    }

            //    await QueryContext.Connection.OpenAsync();
            //    var toReturn = await QueryContext.Command.ExecuteNonQueryAsync();
            //    QueryContext.Connection.Close();
            //    return toReturn;
            //}

            //public async Task<int> ExecuteAsync(T newValues)
            //{
            //    Parent.PreExecuteChecks();

            //    foreach (var mereColumn in QueryContext.CurMereTableMin.SelectMereColumnsNoIdentity())
            //    {
            //        QueryContext.Command.Parameters.AddWithValue("@" + mereColumn.ColumnName, mereColumn.Get(newValues) ?? DBNull.Value);
            //    }

            //    QueryContext.Command.CommandText = QueryContext.SqlUpdate;

            //    await QueryContext.Connection.OpenAsync();
            //    var toReturn = await QueryContext.Command.ExecuteNonQueryAsync();
            //    QueryContext.Connection.Close();
            //    return toReturn;
            //}

            //public Task<int> ExecuteAsync(ExpandoObject newValues)
            //{
            //    return ExecuteAsync((IDictionary<string, object>)newValues);
            //}

            ///// <summary>
            ///// Executes update using supplied fields adding filters for key and/or identity fields
            ///// </summary>
            ///// <param name="newValues"></param>
            ///// <returns></returns>
            //public Task<int> ExecuteAsync(IEnumerable<KeyValuePair<string, object>> newValues)
            //{
            //    return ExecuteAsync(newValues, true);
            //}


            ///// <summary>
            ///// Executes update using supplied fields with option for adding filters for key and/or identity fields
            ///// </summary>
            ///// <param name="newValues"></param>
            ///// <param name="useKeyAndOrIdentity"></param>
            ///// <returns></returns>
            //public Task<int> ExecuteAsync(IEnumerable<KeyValuePair<string, object>> newValues, bool useKeyAndOrIdentity)
            //{
            //    QueryContext.UpdateFieldsDictionary.Clear();

            //    QueryContext.AddUpdateField(newValues, useKeyAndOrIdentity);

            //    //make sure prechecks are done after changing update fields 
            //    //Parent.PreExecuteChecks();

            //    //foreach (var mereColumn in QueryContext.CurMereTableMin.SelectMereColumnsNoIdentity())
            //    //{
            //    //    QueryContext.Command.Parameters.AddWithValue("@" + mereColumn.ColumnName, mereColumn.Get(newValues));
            //    //}

            //    //await QueryContext.Connection.OpenAsync();
            //    //var toReturn = await QueryContext.Command.ExecuteNonQueryAsync();
            //    //QueryContext.Connection.Close();
            //    //return toReturn;

            //    return ExecuteAsync();
            //}
            //#endregion
            #endregion
        }
    }
}
