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
    public interface IMereQueryBase<T> : IDisposable where T : new()
    {
        MereContext<T> QueryContext { get; set; }

        #region methods
        IMereQueryPost<T> AddFilter<TProp>(Expression<Func<T, TProp>> fieldSelector, SqlOperator sqlOperator, TProp value);
        IMereQueryPost<T> AddFilter<TProp>(Expression<Func<T, TProp>> fieldSelector, SqlOperator sqlOperator, TProp value, bool or);
        
        IMereQueryPost<T> AddFilter(string fieldName, SqlOperator sqlOperator, object value);
        IMereQueryPost<T> AddFilter(string fieldName, SqlOperator sqlOperator, object value, bool or);
        #endregion

        #region to TResult
        #region sync

        //IEnumerable<TResult> Execute<TResult>();

        //List<TResult> ExecuteToList<TResult>(Func<T, TResult> selectFields);

        ////List<TResult> ExecuteToList<TResult>(TResult instance);

        //List<TResult> ExecuteToList<TResult>() where TResult : class;
        #endregion

        #region async

        #endregion
        
        #endregion

        #region sync executes

        IEnumerable<T> Execute();

        List<T> ExecuteToList();

        List<IDataRecord> ExecuteToIDataRecordList();

        T ExecuteFirstOrDefault();

        #region ExecuteExpando

        IEnumerable<dynamic> ExecuteDynamic();

        IEnumerable<dynamic> ExecuteDynamic<TFields>(Func<T, TFields> newFields);

        IEnumerable<dynamic> ExecuteDynamic(object selectFields);

        List<dynamic> ExecuteDynamicToList();

        List<dynamic> ExecuteDynamicToList<TFields>(Func<T, TFields> newFields);

        List<dynamic> ExecuteDynamicToList(object selectFields);

        IEnumerable<ExpandoObject> ExecuteExpando();

        IEnumerable<ExpandoObject> ExecuteExpando<TFields>(Func<T, TFields> newFields);

        IEnumerable<ExpandoObject> ExecuteExpando(object selectFields);

        List<ExpandoObject> ExecuteExpandoToList();

        List<ExpandoObject> ExecuteExpandoToList<TFields>(Func<T, TFields> newFields);

        List<ExpandoObject> ExecuteExpandoToList(object selectFields);
        #endregion

        #region ExecuteCustomQuery

        List<dynamic> ExecuteCustomQueryDynamicToList(string customQuery);

        List<dynamic> ExecuteCustomQueryDynamicToList(string customQuery, object parameters);

        IEnumerable<dynamic> ExecuteCustomQueryDynamic(string customQuery);

        IEnumerable<dynamic> ExecuteCustomQueryDynamic(string customQuery, object parameters);

        List<T> ExecuteCustomQueryToList(string customQuery);

        List<T> ExecuteCustomQueryToList(string customQuery, object parameters);

        IEnumerable<T> ExecuteCustomQuery(string customQuery);

        IEnumerable<T> ExecuteCustomQuery(string customQuery, object parameters);
        #endregion

        #region ExecuteDistinct

        IEnumerable<T> ExecuteDistinct();

        IEnumerable<object> ExecuteDistinct<TFields>(Func<T, TFields> fields);

        int ExecuteDistinctCount();

        int ExecuteDistinctCount<TFields>(Func<T, TFields> fields);

        int ExecuteCount();
        #endregion
        #endregion

        #region async executes

        Task<List<T>> ExecuteAsync();

        Task<int> BulkCopyToAsync<TDest>(int batchSize = 1000) where TDest : new();

        Task<List<T>> BulkCopyToChunkedParallel<TDest>(int minChunkSize, int maxChunkSize,
                                                              int threadThrottle = 150, int timeout = 0) where TDest : new();

        Task<List<T>> ExecuteChunkedParallel(Action<int, List<T>> perChunk, int minChunkSize,
                                                    int maxChunkSize = 0, int threadThrottle = 150);

        Task<List<T>> ExecuteChunkedParallel(Func<int, List<T>, List<T>> perChunk, int minChunkSize,
                                                    int maxChunkSize = 0, int threadThrottle = 150);

        Task<List<T>> ExecuteChunkedParallel(Action<List<T>> perChunk, int minChunkSize,
                                                    int maxChunkSize = 0, int threadThrottle = 150);

        Task<List<T>> ExecuteChunkedParallel(Func<List<T>, List<T>> perChunk, int minChunkSize,
                                                    int maxChunkSize = 0, int threadThrottle = 150);

        Task<T> ExecuteFirstOrDefaultAsync();

        Task<List<dynamic>> ExecuteDynamicAsync();

        Task<List<dynamic>> ExecuteDynamicAsync<TFields>(Func<T, TFields> newFields);

        Task<List<dynamic>> ExecuteDynamicAsync(object selectFields);

        Task<List<ExpandoObject>> ExecuteExpandoAsync();

        Task<List<ExpandoObject>> ExecuteExpandoAsync<TFields>(Func<T, TFields> newFields);

        Task<List<ExpandoObject>> ExecuteExpandoAsync(object selectFields);

        Task<List<ExpandoObject>> ExecuteCustomQueryExpandoAsync(string customQuery);

        Task<List<ExpandoObject>> ExecuteCustomQueryExpandoAsync(string customQuery, object parameters);

        Task<List<dynamic>> ExecuteCustomQueryDynamicAsync(string customQuery);

        Task<List<dynamic>> ExecuteCustomQueryDynamicAsync(string customQuery, object parameters);

        Task<List<T>> ExecuteCustomQueryAsync(string customQuery);

        Task<List<T>> ExecuteCustomQueryAsync(string customQuery, object parameters);

        Task<List<T>> ExecuteDistinctAsync();

        Task<List<dynamic>> ExecuteDistinctAsync<TFields>(Func<T, TFields> fields);

        Task<int> ExecuteDistinctCountAsync();

        Task<int> ExecuteDistinctCountAsync<TFields>(Func<T, TFields> fields);

        Task<int> ExecuteCountAsync();
        #endregion
    }
}
