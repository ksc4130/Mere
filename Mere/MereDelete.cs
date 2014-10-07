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
using System.Text;
using System.Threading.Tasks;
using Mere.Interfaces;

namespace Mere
{
    public class MereDelete
    {
        protected MereDelete() { }

        public static IMereDeletePre<T> Create<T>() where T : new()
        {
            return MereDelete<T>.Create();
        }

        public static IMereDeletePre<T> Create<T>(MereDataSource dataSource) where T : new()
        {
            return MereDelete<T>.Create(dataSource);
        }

        #region async
        public static async Task<int> ExecuteByKeyOrIdentityAsync<T>(T toDelete) where T : new()
        {
            var mereTable = MereUtils.CacheCheck<T>();
            var key = mereTable.SelectMereColumns.FirstOrDefault(x => x.IsKey || x.IsIdentity);
            int result;
            using (var myCn = mereTable.GetConnection())
            {
                var cmd = myCn.CreateCommand();
                cmd.Parameters.AddWithValue("@key", key.GetBase(toDelete));
                cmd.CommandText = "DELETE FROM " + mereTable.TableName + " WHERE " + key.ColumnName + "=@key";
                await myCn.OpenAsync();
                result = await cmd.ExecuteNonQueryAsync();
                myCn.Close();
            }

            return result;
        }
        #endregion

        #region sync
        public static int ExecuteByKeyOrIdentity<T>(T toDelete) where T : new()
        {
            var mereTable = MereUtils.CacheCheck<T>();
            var key = mereTable.SelectMereColumns.FirstOrDefault(x => x.IsKey || x.IsIdentity);
            int result;
            using (var myCn = mereTable.GetConnection())
            {
                var cmd = myCn.CreateCommand();
                cmd.Parameters.AddWithValue("@key", key.GetBase(toDelete));
                cmd.CommandText = "DELETE FROM " + mereTable.TableName + " WHERE " + key.ColumnName + "=@key";
                myCn.Open();
                result = cmd.ExecuteNonQuery();
                myCn.Close();
            }

            return result;
        }
        #endregion
    }
    public class MereDelete<T> where T : new()
    {
        private MereDelete()
        {
            _mereContext = new MereContext<T>(MereContextType.Delete);
            _pre = new MereDeletePre<T>(this);
            _post = new MereDeletePost<T>(this);
        }

        private MereDelete(MereDataSource dataSource)
        {
            _mereContext = new MereContext<T>(MereContextType.Delete, dataSource);
            _pre = new MereDeletePre<T>(this);
            _post = new MereDeletePost<T>(this);
        }

        public static IMereDeletePre<T> Create()
        {
            var parent = new MereDelete<T>();
            return parent._pre;
        }

        public static IMereDeletePre<T> Create(MereDataSource dataSource)
        {
            var parent = new MereDelete<T>(dataSource);
            return parent._pre;
        }

        private MereContext<T> _mereContext;
        private readonly IMereDeletePre<T> _pre;
        private readonly IMereDeletePost<T> _post;

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
            _mereContext.Command.CommandText = _mereContext.SqlDelete;
            _mereContext.Command.CommandTimeout = _mereContext.Timeout;
        }
        #endregion

        private class MereDeleteBase<T, TProp> : IMereDeleteBase<T, TProp> where T : new()
        {

            public MereDeleteBase(MereDelete<T> parent)
            {
                _parent = parent;
            }

            private readonly MereDelete<T> _parent;
            private MereContext<T> QueryContext { get { return _parent._mereContext; } }
            #region Creates
            public static IMereDeleteBase<T, TProp> Create(MereDelete<T> parent)
            {
                return new MereDeleteBase<T, TProp>(parent);
            }
            #endregion

            #region filter methods
            public IMereDeletePost<T> EqualTo(TProp value)
            {
                _parent._mereContext.AddFilter(value, SqlOperator.EqualTo);
                return _parent._post;
            }

            public IMereDeletePost<T> EqualToCaseSensitive(TProp value)
            {
                QueryContext.AddFilter(value, SqlOperator.EqualToCaseSensitive);
                return _parent._post;
            }

            public IMereDeletePost<T> NotEqualTo(TProp value)
            {
                QueryContext.AddFilter(value, SqlOperator.NotEqualTo);
                return _parent._post;
            }

            public IMereDeletePost<T> NotEqualToCaseSensitive(TProp value)
            {
                QueryContext.AddFilter(value, SqlOperator.NotEqualToCaseSensitive);
                return _parent._post;
            }

            public IMereDeletePost<T> GreaterThan(TProp value)
            {

                QueryContext.AddFilter(value, SqlOperator.GreaterThan);
                return _parent._post;
            }

            public IMereDeletePost<T> GreaterThanOrEqualTo(TProp value)
            {
                QueryContext.AddFilter(value, SqlOperator.GreaterThanOrEqualTo);
                return _parent._post;
            }

            public IMereDeletePost<T> LessThan(TProp value)
            {
                QueryContext.AddFilter(value, SqlOperator.LessThan);
                return _parent._post;
            }

            public IMereDeletePost<T> LessThanOrEqualTo(TProp value)
            {
                QueryContext.AddFilter(value, SqlOperator.LessThanOrEqualTo);
                return _parent._post;
            }

            public IMereDeletePost<T> In<TVal>(TVal values) where TVal : IEnumerable<TProp>
            {
                QueryContext.AddFilterIn(values);
                return _parent._post;
            }

            public IMereDeletePost<T> In(params TProp[] values)
            {
                QueryContext.AddFilterIn(values);
                return _parent._post;
            }

            public IMereDeletePost<T> InCaseSensitive<TVal>(TVal values) where TVal : IEnumerable<TProp>
            {
                QueryContext.AddFilterInCaseSensitive(values);
                return _parent._post;
            }

            public IMereDeletePost<T> InCaseSensitive(params TProp[] values)
            {
                QueryContext.AddFilterInCaseSensitive(values);
                return _parent._post;
            }

            public IMereDeletePost<T> NotIn<TVal>(TVal values) where TVal : IEnumerable<TProp>
            {
                QueryContext.AddFilterNotIn(values);
                return _parent._post;
            }

            public IMereDeletePost<T> NotIn(params TProp[] values)
            {
                QueryContext.AddFilterNotIn(values);
                return _parent._post;
            }

            public IMereDeletePost<T> NotInCaseSensitive<TVal>(TVal values) where TVal : IEnumerable<TProp>
            {
                QueryContext.AddFilterNotInCaseSensitive(values);
                return _parent._post;
            }

            public IMereDeletePost<T> NotInCaseSensitive(params TProp[] values)
            {
                QueryContext.AddFilterNotInCaseSensitive(values);
                return _parent._post;
            }

            public IMereDeletePost<T> Between(TProp value1, TProp value2)
            {
                QueryContext.AddFilterBetween(value1, value2);
                return _parent._post;
            }

            public IMereDeletePost<T> NotBetween(TProp value1, TProp value2)
            {
                QueryContext.AddFilterNotBetween(value1, value2);
                return _parent._post;
            }


            public IMereDeletePost<T> Contains(TProp value)
            {
                var v = "%" + value + "%";
                QueryContext.AddFilter(v, SqlOperator.Like);
                return _parent._post;
            }

            public IMereDeletePost<T> NotContains(TProp value)
            {
                var v = "%" + value + "%";
                QueryContext.AddFilter(v, SqlOperator.NotLike);
                return _parent._post;
            }

            public IMereDeletePost<T> StartsWith(TProp value)
            {
                var v = value + "%";

                QueryContext.AddFilter(v, SqlOperator.Like);
                return _parent._post;
            }

            public IMereDeletePost<T> NotStartsWith(TProp value)
            {
                var v = value + "%";

                QueryContext.AddFilter(v, SqlOperator.NotLike);
                return _parent._post;
            }

            public IMereDeletePost<T> EndsWith(TProp value)
            {
                var v = "%" + value;

                QueryContext.AddFilter(v, SqlOperator.Like);
                return _parent._post;
            }

            public IMereDeletePost<T> NotEndsWith(TProp value)
            {
                var v = "%" + value;

                QueryContext.AddFilter(v, SqlOperator.NotLike);
                return _parent._post;
            }

            public IMereDeletePost<T> ContainsCaseSensitive(TProp value)
            {
                var v = "%" + value + "%";
                QueryContext.AddFilter(v, SqlOperator.LikeCaseSensitive);
                return _parent._post;
            }

            public IMereDeletePost<T> NotContainsCaseSensitive(TProp value)
            {
                var v = "%" + value + "%";
                QueryContext.AddFilter(v, SqlOperator.NotLikeCaseSensitive);
                return _parent._post;
            }

            public IMereDeletePost<T> StartsWithCaseSensitive(TProp value)
            {
                var v = value + "%";

                QueryContext.AddFilter(v, SqlOperator.LikeCaseSensitive);
                return _parent._post;
            }

            public IMereDeletePost<T> NotStartsWithCaseSensitive(TProp value)
            {
                var v = value + "%";

                QueryContext.AddFilter(v, SqlOperator.NotLikeCaseSensitive);
                return _parent._post;
            }

            public IMereDeletePost<T> EndsWithCaseSensitive(TProp value)
            {
                var v = "%" + value;

                QueryContext.AddFilter(v, SqlOperator.LikeCaseSensitive);
                return _parent._post;
            }

            public IMereDeletePost<T> NotEndsWithCaseSensitive(TProp value)
            {
                var v = "%" + value;

                QueryContext.AddFilter(v, SqlOperator.NotLikeCaseSensitive);
                return _parent._post;
            }

            #endregion
        }

        private class MereDeletePre<T> : MereDeleteBase<T>, IMereDeletePre<T> where T : new()
        {

            public MereDeletePre(MereDelete<T> parent) : base(parent) { }

            #region filter methods
            public IMereDeleteBase<T, TProp> Where<TProp>(Expression<Func<T, TProp>> fieldSelector)
            {
                QueryContext.InitFilterAdd(fieldSelector);
                return new MereDeleteBase<T, TProp>(Parent);
            }

            public IMereDeletePost<T> Where(string whereSql, object whereValues = null)
            {
                QueryContext.AddFilter(whereSql, whereValues);
                return Parent._post;
            }

            #endregion
        }

        private class MereDeletePost<T> : MereDeleteBase<T>, IMereDeletePost<T> where T : new()
        {
            public MereDeletePost(MereDelete<T> parent) : base(parent) { }

            #region filter methods

            #region AndsAndAndGroups
            //ands
            /// <summary>
            /// Initiate adding an and filter by selecting the field to filter on
            /// </summary>
            /// <typeparam name="TProp">Selected property type</typeparam>
            /// <param name="fieldSelector">Lambda exepression to select field to filter on</param>
            /// <returns>QueryObject to add operator and value</returns>
            public IMereDeleteBase<T, TProp> And<TProp>(Expression<Func<T, TProp>> fieldSelector)
            {
                QueryContext.InitFilterAdd(fieldSelector);
                return new MereDeleteBase<T, TProp>(Parent);
            }

            /// <summary>
            /// Adding custom and filter/statement with hardcoded values or 
            /// pass in an object with properties named the same as you parameters minus the "@" sign.
            /// </summary>
            /// <param name="whereSql">Custom where filter/statement</param>
            /// <param name="whereValues">Parameter values for custom filter/statement</param>
            /// <returns></returns>
            public IMereDeletePost<T> And(string whereSql, object whereValues = null)
            {
                QueryContext.AddFilter(whereSql, whereValues);
                return this;
            }

            //and filter groups
            public IMereDeleteBase<T, TProp> AndGroup<TProp>(Expression<Func<T, TProp>> prop)
            {
                QueryContext.InitFilterGroupAdd(prop);
                return new MereDeleteBase<T, TProp>(Parent);
            }

            public IMereDeletePost<T> AndGroup(string whereSql, object whereValues = null)
            {
                QueryContext.AddFilterGroup(whereSql, whereValues);
                return this;
            }

            //end and filter groups
            public IMereDeleteBase<T, TProp> AndGroupInner<TProp>(Expression<Func<T, TProp>> prop)
            {
                QueryContext.InitFilterGroupAdd(prop, false, true);
                return new MereDeleteBase<T, TProp>(Parent);
            }

            public IMereDeletePost<T> AndGroupInner(string whereSql, object whereValues = null)
            {
                QueryContext.AddFilterGroup(whereSql, whereValues, false, true);
                return this;
            }

            public IMereDeleteBase<T, TProp> AndGroupBackup<TProp>(Expression<Func<T, TProp>> prop, int backup = 1)
            {
                QueryContext.InitFilterGroupAdd(prop, false, true, backup);
                return new MereDeleteBase<T, TProp>(Parent);
            }

            public IMereDeletePost<T> AndGroupBackup(string whereSql, object whereValues = null, int backup = 1)
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
            public IMereDeleteBase<T, TProp> Or<TProp>(Expression<Func<T, TProp>> fieldSelector)
            {
                QueryContext.InitFilterAdd(fieldSelector, true);
                return new MereDeleteBase<T, TProp>(Parent);
            }

            /// <summary>
            /// Adding custom and filter/statement with hardcoded values or 
            /// pass in an object with properties named the same as you parameters minus the "@" sign.
            /// </summary>
            /// <param name="whereSql">Custom where filter/statement</param>
            /// <param name="whereValues">Parameter values for custom filter/statement</param>
            /// <returns></returns>
            public IMereDeletePost<T> Or(string whereSql, object whereValues = null)
            {
                QueryContext.AddFilter(whereSql, whereValues, true);
                return this;
            }

            //and filter groups
            public IMereDeleteBase<T, TProp> OrGroup<TProp>(Expression<Func<T, TProp>> prop)
            {
                QueryContext.InitFilterGroupAdd(prop, true);
                return new MereDeleteBase<T, TProp>(Parent);
            }

            public IMereDeletePost<T> OrGroup(string whereSql, object whereValues = null)
            {
                QueryContext.AddFilterGroup(whereSql, whereValues, true);
                return this;
            }

            //end and filter groups
            public IMereDeleteBase<T, TProp> OrGroupInner<TProp>(Expression<Func<T, TProp>> prop)
            {
                QueryContext.InitFilterGroupAdd(prop, true, true);
                return new MereDeleteBase<T, TProp>(Parent);
            }

            public IMereDeletePost<T> OrGroupInner(string whereSql, object whereValues = null)
            {
                QueryContext.AddFilterGroup(whereSql, whereValues, true, true);
                return this;
            }

            public IMereDeleteBase<T, TProp> OrGroupBackup<TProp>(Expression<Func<T, TProp>> prop, int backup = 1)
            {
                QueryContext.InitFilterGroupAdd(prop, true, true, backup);
                return new MereDeleteBase<T, TProp>(Parent);
            }

            public IMereDeletePost<T> OrGroupBackup(string whereSql, object whereValues = null, int backup = 1)
            {
                QueryContext.AddFilterGroup(whereSql, whereValues, true, true, backup);
                return this;
            }

            //end ors 
            #endregion

            #endregion
        }

        private abstract class MereDeleteBase<T> : IMereDeleteBase<T> where T : new()
        {
            //protected MereContext<T> QueryContext { get; set; }
            public MereDeleteBase(MereDelete<T> parent)
            {
                Parent = parent;
            }

            protected readonly MereDelete<T> Parent;
            public MereContext<T> QueryContext { get { return Parent._mereContext; } set { Parent._mereContext = value; } }

            public IMereDeletePost<T> AddFilter<TProp>(Expression<Func<T, TProp>> fieldSelector, SqlOperator sqlOperator,
                                                     TProp value)
            {
                return AddFilter(fieldSelector, sqlOperator, value, false);
            }

            public IMereDeletePost<T> AddFilter<TProp>(Expression<Func<T, TProp>> fieldSelector, SqlOperator sqlOperator,
                                                      TProp value, bool or)
            {
                QueryContext.InitFilterAdd(fieldSelector, or);
                QueryContext.AddFilter(value, sqlOperator);
                return Parent._post;
            }

            public IMereDeletePost<T> AddFilter(string fieldName, SqlOperator sqlOperator, object value)
            {
                return AddFilter(fieldName, sqlOperator, value, false);
            }

            public IMereDeletePost<T> AddFilter(string fieldName, SqlOperator sqlOperator, object value, bool or)
            {
                QueryContext.InitFilterAdd(fieldName, or);
                QueryContext.AddFilter(value, sqlOperator);
                return Parent._post;
            }

            #region execute to generic

            //#region sync
            ////public IEnumerable<TResult> Execute<TResult>()
            ////{
            ////    Parent.PreExecuteChecks();

            ////    QueryContext.Connection.Open();
            ////    using (var reader = new MereSqlDataReader<TResult>(QueryContext.Command.ExecuteReader(), QueryContext.SelectMereColumnsList))
            ////    {
            ////        while (reader.Read())
            ////        {
            ////            yield return reader;
            ////        }
            ////        QueryContext.Connection.Close();
            ////    }
            ////}
            //#endregion

            //public async Task<IList<TResult>> ExecuteAsync<TResult>()
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
            #endregion//end execute to generic

            #region sync executes
            public int Execute()
            {
                return QueryContext.ExecuteDelete();
            }
            #endregion

            #region async executes
            public Task<int> ExecuteAsync()
            {
                return QueryContext.ExecuteDeleteAsync();
            }
            #endregion

            #region old executes
            //#region sync executes
            //public int Execute()
            //{
            //    Parent.PreExecuteChecks();
            //    QueryContext.Connection.Open();
            //    var toReturn = QueryContext.Command.ExecuteNonQuery();
            //    QueryContext.Connection.Close();
            //    return toReturn;
            //}
            //#endregion

            //#region async executes
            //public async Task<int> ExecuteAsync()
            //{
            //    Parent.PreExecuteChecks();
            //    await QueryContext.Connection.OpenAsync();
            //    var toReturn = await QueryContext.Command.ExecuteNonQueryAsync();
            //    QueryContext.Connection.Close();
            //    return toReturn;
            //}
            //#endregion
            #endregion
            
        }
    }
}
