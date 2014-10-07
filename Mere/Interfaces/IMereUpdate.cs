using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Mere.Interfaces
{
    public interface IMereUpdateBase<T> where T : new()
    {
        #region properties
        MereContext<T> QueryContext { get; set; }

        #endregion
        #region methods
        IMereUpdatePost<T> AddFilter(string fieldName, SqlOperator sqlOperator, object value);
        IMereUpdatePost<T> AddFilter(string fieldName, SqlOperator sqlOperator, object value, bool or);
        #endregion

        #region to TResult
        //IList<TResult> ExecuteToList<TResult>(Func<T, TResult> selectFields);

        ////IList<TResult> ExecuteToList<TResult>(TResult instance);

        //IList<TResult> ExecuteToList<TResult>() where TResult : class;
        #endregion

        #region sync executes
        int Execute();
        int Execute(T newValues);
        int Execute(ExpandoObject newValues);
        int Execute(IEnumerable<KeyValuePair<string, object>> newValues);
        int Execute(IEnumerable<KeyValuePair<string, object>> newValues, bool useKeyAndOrIdentity);
        #endregion

        #region async executes
        Task<int> ExecuteAsync();
        Task<int> ExecuteAsync(T newValues);
        Task<int> ExecuteAsync(ExpandoObject newValues);
        Task<int> ExecuteAsync(IEnumerable<KeyValuePair<string, object>> newValues);
        Task<int> ExecuteAsync(IEnumerable<KeyValuePair<string, object>> newValues, bool useKeyAndOrIdentity);
        #endregion

    }

    public interface IMereUpdatePre<T> : IMereUpdateBase<T> where T : new()
    {
        IMereUpdatePre<T> Set<TProp>(Expression<Func<T, TProp>> field, TProp value);
        IMereUpdatePre<T> Set(IEnumerable<KeyValuePair<string, object>> fields);
        IMereUpdatePre<T> Set(IEnumerable<KeyValuePair<string, object>> fields, bool filterWithKeyAndOrIdentity);
        IMereUpdatePre<T> SetDatabase(string databaseName);
        IMereUpdatePre<T> SetServer(string serverName);
        IMereUpdatePre<T> SetTable(string tableName);
        IMereUpdatePre<T> SetUserId(string userId);
        IMereUpdatePre<T> SetPassword(string password);
        #region filter methods
        IMereUpdateBase<T, TProp> Where<TProp>(Expression<Func<T, TProp>> fieldSelector);
        IMereUpdatePost<T> Where(string whereSql, object whereValues = null);
        #endregion
    }

    public interface IMereUpdatePost<T> : IMereUpdateBase<T> where T : new()
    {
        IMereUpdatePost<T> Set<TProp>(Expression<Func<T, TProp>> field, TProp value);
        IMereUpdatePost<T> Set(IEnumerable<KeyValuePair<string, object>> fields);
        IMereUpdatePost<T> Set(IEnumerable<KeyValuePair<string, object>> fields, bool filterWithKeyAndOrIdentity);
        IMereUpdatePost<T> SetDatabase(string databaseName);
        IMereUpdatePost<T> SetServer(string serverName);
        IMereUpdatePost<T> SetTable(string tableName);
        IMereUpdatePost<T> SetUserId(string userId);
        IMereUpdatePost<T> SetPassword(string password);
        #region filter methods

        #region AndsAndAndGroups
        //ands

        IMereUpdateBase<T, TProp> And<TProp>(Expression<Func<T, TProp>> fieldSelector);

        IMereUpdatePost<T> And(string whereSql, object whereValues = null);
        //and filter groups
        IMereUpdateBase<T, TProp> AndGroup<TProp>(Expression<Func<T, TProp>> prop);

        IMereUpdatePost<T> AndGroup(string whereSql, object whereValues = null);


        //end and filter groups
        IMereUpdateBase<T, TProp> AndGroupInner<TProp>(Expression<Func<T, TProp>> prop);

        IMereUpdatePost<T> AndGroupInner(string whereSql, object whereValues = null);


        IMereUpdateBase<T, TProp> AndGroupBackup<TProp>(Expression<Func<T, TProp>> prop, int backup = 1);

        IMereUpdatePost<T> AndGroupBackup(string whereSql, object whereValues = null, int backup = 1);

        //                                 int backup = 1);
        //end ands 
        #endregion

        #region OrsAndOrGroups
        //ands

        IMereUpdateBase<T, TProp> Or<TProp>(Expression<Func<T, TProp>> fieldSelector);

        IMereUpdatePost<T> Or(string whereSql, object whereValues = null);

        //and filter groups
        IMereUpdateBase<T, TProp> OrGroup<TProp>(Expression<Func<T, TProp>> prop);

        IMereUpdatePost<T> OrGroup(string whereSql, object whereValues = null);

        //end and filter groups
        IMereUpdateBase<T, TProp> OrGroupInner<TProp>(Expression<Func<T, TProp>> prop);

        IMereUpdatePost<T> OrGroupInner(string whereSql, object whereValues = null);

        IMereUpdateBase<T, TProp> OrGroupBackup<TProp>(Expression<Func<T, TProp>> prop, int backup = 1);

        IMereUpdatePost<T> OrGroupBackup(string whereSql, object whereValues = null, int backup = 1);

        //end ors 
        #endregion

        #endregion
    }

    public interface IMereUpdateBase<T, TProp> where T : new()
    {
        #region filter methods
        IMereUpdatePost<T> EqualTo(TProp value);

        IMereUpdatePost<T> EqualToCaseSensitive(TProp value);

        IMereUpdatePost<T> NotEqualTo(TProp value);

        IMereUpdatePost<T> NotEqualToCaseSensitive(TProp value);

        IMereUpdatePost<T> GreaterThan(TProp value);

        IMereUpdatePost<T> GreaterThanOrEqualTo(TProp value);

        IMereUpdatePost<T> LessThan(TProp value);

        IMereUpdatePost<T> LessThanOrEqualTo(TProp value);

        IMereUpdatePost<T> In<TVal>(TVal values) where TVal : IEnumerable<TProp>;
        IMereUpdatePost<T> In(params TProp[] values);

        IMereUpdatePost<T> InCaseSensitive<TVal>(TVal values) where TVal : IEnumerable<TProp>;
        IMereUpdatePost<T> InCaseSensitive(params TProp[] values);

        IMereUpdatePost<T> NotIn<TVal>(TVal values) where TVal : IEnumerable<TProp>;
        IMereUpdatePost<T> NotIn(params TProp[] values);

        IMereUpdatePost<T> NotInCaseSensitive<TVal>(TVal values) where TVal : IEnumerable<TProp>;
        IMereUpdatePost<T> NotInCaseSensitive(params TProp[] values);

        IMereUpdatePost<T> Between(TProp value1, TProp value2);
        
        IMereUpdatePost<T> NotBetween(TProp value1, TProp value2);

        IMereUpdatePost<T> Contains(TProp value);
        
        IMereUpdatePost<T> NotContains(TProp value);

        IMereUpdatePost<T> StartsWith(TProp value);
        
        IMereUpdatePost<T> NotStartsWith(TProp value);
        
        IMereUpdatePost<T> EndsWith(TProp value);
        
        IMereUpdatePost<T> NotEndsWith(TProp value);

        IMereUpdatePost<T> ContainsCaseSensitive(TProp value);
        
        IMereUpdatePost<T> NotContainsCaseSensitive(TProp value);

        IMereUpdatePost<T> StartsWithCaseSensitive(TProp value);
        
        IMereUpdatePost<T> NotStartsWithCaseSensitive(TProp value);

        IMereUpdatePost<T> EndsWithCaseSensitive(TProp value);
        
        IMereUpdatePost<T> NotEndsWithCaseSensitive(TProp value);

        #endregion

    }
}
