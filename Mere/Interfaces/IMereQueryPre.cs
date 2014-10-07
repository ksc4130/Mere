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
    public interface IMereQueryPre<T> : IMereQueryBase<T> where T : new()
    {
        #region methods
        IMereQueryPre<T> SetFields<TFields>();

        IMereQueryPre<T> SetFields<TFields>(Func<T, TFields> newFields);

        IMereQueryPre<T> SetFields(dynamic fields);

        IMereQueryPre<T> OrderBy<TProp>(Expression<Func<T, TProp>> prop, bool descending = false);

        IMereQueryPre<T> OrderBy<TProp>(IEnumerable<string> fields, bool allDescending = false);

        IMereQueryPre<T> SetDatabase(string databaseName);

        IMereQueryPre<T> SetServer(string serverName);

        IMereQueryPre<T> SetTable(string tableName);

        IMereQueryPre<T> SetUserId(string userId);

        IMereQueryPre<T> SetPassword(string password);

        IMereQueryPre<T> SetTop(int top);
        #endregion

        #region filter methods
        IMereQueryFilter<T, TProp> Where<TProp>(Expression<Func<T, TProp>> fieldSelector);
        IMereQueryPost<T> Where(string whereSql, object whereValues = null);
        #endregion
    }
}
