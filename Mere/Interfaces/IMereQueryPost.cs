using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Mere.Interfaces
{
    public interface IMereQueryPost<T> : IMereQueryBase<T> where T : new()
    {
        #region methods
        IMereQueryPost<T> SetFields<TFields>();

        IMereQueryPost<T> SetFields<TFields>(Func<T, TFields> newFields);

        IMereQueryPost<T> SetFields(dynamic fields);

        IMereQueryPost<T> OrderBy<TProp>(Expression<Func<T, TProp>> prop, bool descending = false);

        IMereQueryPost<T> OrderBy<TProp>(IEnumerable<string> fields, bool allDescending = false);

        IMereQueryPost<T> SetDatabase(string databaseName);

        IMereQueryPost<T> SetServer(string serverName);

        IMereQueryPost<T> SetTable(string tableName);

        IMereQueryPost<T> SetUserId(string userId);

        IMereQueryPost<T> SetPassword(string password);

        IMereQueryPost<T> Top(int top);
        #endregion

        #region filter methods

        #region AndsAndAndGroups
        //ands

        IMereQueryFilter<T, TProp> And<TProp>(Expression<Func<T, TProp>> fieldSelector);

        IMereQueryPost<T> And(string whereSql, object whereValues = null);
        //and filter groups
        IMereQueryFilter<T, TProp> AndGroup<TProp>(Expression<Func<T, TProp>> prop);

        IMereQueryPost<T> AndGroup(string whereSql, object whereValues = null);


        //end and filter groups
        IMereQueryFilter<T, TProp> AndGroupInner<TProp>(Expression<Func<T, TProp>> prop);

        IMereQueryPost<T> AndGroupInner(string whereSql, object whereValues = null);


        IMereQueryFilter<T, TProp> AndGroupBackup<TProp>(Expression<Func<T, TProp>> prop, int backup = 1);

        IMereQueryPost<T> AndGroupBackup(string whereSql, object whereValues = null, int backup = 1);

        //                                 int backup = 1);
        //end ands 
        #endregion

        #region OrsAndOrGroups
        //ands

        IMereQueryFilter<T, TProp> Or<TProp>(Expression<Func<T, TProp>> fieldSelector);

        IMereQueryPost<T> Or(string whereSql, object whereValues = null);

        //and filter groups
        IMereQueryFilter<T, TProp> OrGroup<TProp>(Expression<Func<T, TProp>> prop);

        IMereQueryPost<T> OrGroup(string whereSql, object whereValues = null);

        //end and filter groups
        IMereQueryFilter<T, TProp> OrGroupInner<TProp>(Expression<Func<T, TProp>> prop);

        IMereQueryPost<T> OrGroupInner(string whereSql, object whereValues = null);

        IMereQueryFilter<T, TProp> OrGroupBackup<TProp>(Expression<Func<T, TProp>> prop, int backup = 1);

        IMereQueryPost<T> OrGroupBackup(string whereSql, object whereValues = null, int backup = 1);

        //end ors 
        #endregion

        #endregion
    }
}
