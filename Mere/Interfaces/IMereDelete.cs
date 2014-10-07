using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Mere.Interfaces
{
    public interface IMereDeleteBase<T> where T : new()
    {
        #region properties
        MereContext<T> QueryContext { get; set; }

        #endregion
        #region methods
        IMereDeletePost<T> AddFilter(string fieldName, SqlOperator sqlOperator, object value);
        IMereDeletePost<T> AddFilter(string fieldName, SqlOperator sqlOperator, object value, bool or);
        #endregion

        #region to TResult
        //IList<TResult> ExecuteToList<TResult>(Func<T, TResult> selectFields);

        ////IList<TResult> ExecuteToList<TResult>(TResult instance);

        //IList<TResult> ExecuteToList<TResult>() where TResult : class;
        #endregion

        #region sync executes
        int Execute();
        #endregion

        #region async executes
        Task<int> ExecuteAsync();
        #endregion

    }

    public interface IMereDeletePre<T> : IMereDeleteBase<T> where T : new()
    {
        #region filter methods
        IMereDeleteBase<T, TProp> Where<TProp>(Expression<Func<T, TProp>> fieldSelector);
        IMereDeletePost<T> Where(string whereSql, object whereValues = null);
        #endregion
    }

    public interface IMereDeletePost<T> : IMereDeleteBase<T> where T : new()
    {
        #region filter methods

        #region AndsAndAndGroups
        //ands

        IMereDeleteBase<T, TProp> And<TProp>(Expression<Func<T, TProp>> fieldSelector);

        IMereDeletePost<T> And(string whereSql, object whereValues = null);
        //and filter groups
        IMereDeleteBase<T, TProp> AndGroup<TProp>(Expression<Func<T, TProp>> prop);

        IMereDeletePost<T> AndGroup(string whereSql, object whereValues = null);


        //end and filter groups
        IMereDeleteBase<T, TProp> AndGroupInner<TProp>(Expression<Func<T, TProp>> prop);

        IMereDeletePost<T> AndGroupInner(string whereSql, object whereValues = null);


        IMereDeleteBase<T, TProp> AndGroupBackup<TProp>(Expression<Func<T, TProp>> prop, int backup = 1);

        IMereDeletePost<T> AndGroupBackup(string whereSql, object whereValues = null, int backup = 1);

        //                                 int backup = 1);
        //end ands 
        #endregion

        #region OrsAndOrGroups
        //ands

        IMereDeleteBase<T, TProp> Or<TProp>(Expression<Func<T, TProp>> fieldSelector);

        IMereDeletePost<T> Or(string whereSql, object whereValues = null);

        //and filter groups
        IMereDeleteBase<T, TProp> OrGroup<TProp>(Expression<Func<T, TProp>> prop);

        IMereDeletePost<T> OrGroup(string whereSql, object whereValues = null);

        //end and filter groups
        IMereDeleteBase<T, TProp> OrGroupInner<TProp>(Expression<Func<T, TProp>> prop);

        IMereDeletePost<T> OrGroupInner(string whereSql, object whereValues = null);

        IMereDeleteBase<T, TProp> OrGroupBackup<TProp>(Expression<Func<T, TProp>> prop, int backup = 1);

        IMereDeletePost<T> OrGroupBackup(string whereSql, object whereValues = null, int backup = 1);

        //end ors 
        #endregion

        #endregion
    }

    public interface IMereDeleteBase<T, TProp> where T : new()
    {
        #region filter methods
        IMereDeletePost<T> EqualTo(TProp value);

        IMereDeletePost<T> EqualToCaseSensitive(TProp value);

        IMereDeletePost<T> NotEqualTo(TProp value);

        IMereDeletePost<T> NotEqualToCaseSensitive(TProp value);

        IMereDeletePost<T> GreaterThan(TProp value);

        IMereDeletePost<T> GreaterThanOrEqualTo(TProp value);

        IMereDeletePost<T> LessThan(TProp value);

        IMereDeletePost<T> LessThanOrEqualTo(TProp value);

        IMereDeletePost<T> In<TVal>(TVal values) where TVal : IEnumerable<TProp>;
        IMereDeletePost<T> In(params TProp[] values);

        IMereDeletePost<T> InCaseSensitive<TVal>(TVal values) where TVal : IEnumerable<TProp>;
        IMereDeletePost<T> InCaseSensitive(params TProp[] values);

        IMereDeletePost<T> NotIn<TVal>(TVal values) where TVal : IEnumerable<TProp>;
        IMereDeletePost<T> NotIn(params TProp[] values);

        IMereDeletePost<T> NotInCaseSensitive<TVal>(TVal values) where TVal : IEnumerable<TProp>;
        IMereDeletePost<T> NotInCaseSensitive(params TProp[] values);

        IMereDeletePost<T> Between(TProp value1, TProp value2);
        
        IMereDeletePost<T> NotBetween(TProp value1, TProp value2);

        IMereDeletePost<T> Contains(TProp value);

        IMereDeletePost<T> NotContains(TProp value);

        IMereDeletePost<T> StartsWith(TProp value);
        
        IMereDeletePost<T> NotStartsWith(TProp value);

        IMereDeletePost<T> EndsWith(TProp value);
        
        IMereDeletePost<T> NotEndsWith(TProp value);

        IMereDeletePost<T> ContainsCaseSensitive(TProp value);
        
        IMereDeletePost<T> NotContainsCaseSensitive(TProp value);

        IMereDeletePost<T> StartsWithCaseSensitive(TProp value);
        
        IMereDeletePost<T> NotStartsWithCaseSensitive(TProp value);

        IMereDeletePost<T> EndsWithCaseSensitive(TProp value);
        
        IMereDeletePost<T> NotEndsWithCaseSensitive(TProp value);

        #endregion
    }
}
