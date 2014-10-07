//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Text;
//using System.Threading.Tasks;
//using Mere.Interfaces;

//namespace Mere.BaseClasses
//{
//    internal abstract class MereQueryBase<T>
//    {
//        //protected MereContext<T> QueryContext { get; set; }
//        public MereQueryBase(MereQuery<T> parent)
//        {
//            Parent = parent;
//        }

//        protected readonly MereQuery<T> Parent;
//        public MereContext<T> QueryContext { get { return Parent._queryContext; } set { Parent._queryContext = value; } }

//        public IMereQueryPost<T> AddFilter<TProp>(Expression<Func<T, TProp>> fieldSelector, SqlOperator sqlOperator,
//                                                  TProp value)
//        {
//            return AddFilter(fieldSelector, sqlOperator, value, false);
//        }

//        public IMereQueryPost<T> AddFilter<TProp>(Expression<Func<T, TProp>> fieldSelector, SqlOperator sqlOperator,
//                                                  TProp value, bool or)
//        {
//            QueryContext.InitFilterAdd(fieldSelector, or);
//            QueryContext.AddFilter(value, sqlOperator);
//            return Parent._post;
//        }

//        public IMereQueryPost<T> AddFilter(string fieldName, SqlOperator sqlOperator, object value)
//        {
//            return AddFilter(fieldName, sqlOperator, value, false);
//        }

//        public IMereQueryPost<T> AddFilter(string fieldName, SqlOperator sqlOperator, object value, bool or)
//        {
//            QueryContext.InitFilterAdd(fieldName, or);
//            QueryContext.AddFilter(value, sqlOperator);
//            return Parent._post;
//        }

//        #region execute to generic

//        #region sync
//        public IEnumerable<TResult> Execute<TResult>()
//        {
//            Parent.PreExecuteChecks();

//            QueryContext.Connection.Open();
//            using (var reader = new MereSqlDataReader<TResult>(QueryContext.Command.ExecuteReader(), QueryContext.SelectMereColumnsList))
//            {
//                while (reader.Read())
//                {
//                    yield return reader;
//                }
//                QueryContext.Connection.Close();
//            }
//        }

//        public List<TResult> ExecuteToList<TResult>(Func<T, TResult> selectFields)
//        {
//            return ExecuteToList(selectFields(new T()));
//        }

//        public List<TResult> ExecuteToList<TResult>() where TResult : class
//        {
//            var sw = Stopwatch.StartNew();
//            long conOpen;
//            long readDone;
//            var selectMereColumnsBefore = QueryContext.SelectMereColumnsList;
//            QueryContext.SelectMereColumnsList = MereUtils.CacheCheckMin<TResult>().SelectMereColumns;
//            Parent.PreExecuteChecks();

//            var toReturn = new List<TResult>();

//            QueryContext.Connection.Open();
//            conOpen = sw.ElapsedMilliseconds;
//            using (var reader = new MereSqlDataReader<TResult>(QueryContext.Command.ExecuteReader(), QueryContext.SelectMereColumnsList))
//            {
//                while (reader.Read())
//                {
//                    toReturn.Add(reader);
//                }
//                readDone = sw.ElapsedMilliseconds;
//                Console.WriteLine("done reading {0} read time: {1}", readDone, readDone - conOpen);
//                QueryContext.Connection.Close();
//            }
//            QueryContext.SelectMereColumnsList = selectMereColumnsBefore;
//            return toReturn;
//        }

//        private List<TResult> ExecuteToList<TResult>(TResult instance)
//        {
//            var sw = Stopwatch.StartNew();
//            long conOpen;
//            long readDone;
//            var selectMereColumnsBefore = QueryContext.SelectMereColumnsList;
//            QueryContext.SelectMereColumnsList = MereUtils.CacheCheckMin<TResult>().SelectMereColumns;
//            Parent.PreExecuteChecks();

//            var toReturn = new List<TResult>();
//            var type = instance.GetType();
//            QueryContext.Connection.Open();
//            conOpen = sw.ElapsedMilliseconds;
//            using (var reader = QueryContext.Command.ExecuteReader())
//            {
//                var properties = TypeDescriptor.GetProperties(instance);
//                while (reader.Read())
//                {
//                    var objIdx = 0;
//                    var objArray = new object[properties.Count];
//                    foreach (PropertyDescriptor info in properties)
//                        objArray[objIdx++] = reader[info.Name];
//                    toReturn.Add((TResult)Activator.CreateInstance(type, objArray));
//                }
//                readDone = sw.ElapsedMilliseconds;
//                Console.WriteLine("done reading {0} read time: {1}", readDone, readDone - conOpen);
//                QueryContext.Connection.Close();
//            }
//            QueryContext.SelectMereColumnsList = selectMereColumnsBefore;
//            return toReturn;
//        }
//        #endregion

//        public async Task<List<TResult>> ExecuteAsync<TResult>()
//        {
//            Parent.PreExecuteChecks();

//            var toReturn = new List<TResult>();

//            await QueryContext.Connection.OpenAsync();
//            using (var reader = new MereSqlDataReader<TResult>(await QueryContext.Command.ExecuteReaderAsync(), QueryContext.SelectMereColumnsList))
//            {
//                while (await reader.ReadAsync())
//                {
//                    toReturn.Add(reader);
//                }
//                QueryContext.Connection.Close();
//            }
//            return toReturn;
//        }
//        #endregion//end execute to generic

//        #region sync executes
//        public IEnumerable<T> Execute()
//        {
//            Parent.PreExecuteChecks();

//            QueryContext.Connection.Open();
//            using (var reader = new MereSqlDataReader<T>(QueryContext.Command.ExecuteReader(), QueryContext.SelectMereColumnsList))
//            {
//                while (reader.Read())
//                {
//                    yield return reader;
//                }
//                QueryContext.Connection.Close();
//            }
//        }

//        public List<T> ExecuteToList()
//        {
//            var sw = Stopwatch.StartNew();
//            long conOpen;
//            long readDone;
//            Parent.PreExecuteChecks();

//            var toReturn = new List<T>();

//            QueryContext.Connection.Open();
//            conOpen = sw.ElapsedMilliseconds;
//            Console.WriteLine("con open {0}", conOpen);
//            using (var reader = new MereSqlDataReader<T>(QueryContext.Command.ExecuteReader(), QueryContext.SelectMereColumnsList))
//            {
//                while (reader.Read())
//                {
//                    toReturn.Add(reader);
//                }
//                readDone = sw.ElapsedMilliseconds;
//                Console.WriteLine("done reading {0} read time: {1}", readDone, readDone - conOpen);
//                QueryContext.Connection.Close();
//            }
//            return toReturn;
//        }

//        public List<IDataRecord> ExecuteToIDataRecordList()
//        {
//            var sw = Stopwatch.StartNew();
//            long conOpen;
//            long readDone;
//            Parent.PreExecuteChecks();

//            var toReturn = new List<IDataRecord>();

//            QueryContext.Connection.Open();
//            conOpen = sw.ElapsedMilliseconds;
//            Console.WriteLine("con open {0}", conOpen);
//            using (var reader = QueryContext.Command.ExecuteReader())
//            {
//                while (reader.Read())
//                {
//                    toReturn.Add(reader);
//                }
//                readDone = sw.ElapsedMilliseconds;
//                Console.WriteLine("done reading {0} read time: {1}", readDone, readDone - conOpen);
//                QueryContext.Connection.Close();
//            }
//            return toReturn;
//        }

//        public T ExecuteFirstOrDefault()
//        {
//            //var queryHasChangedBefore = QueryHasChangeSinceLastExecute;
//            var startingTop = QueryContext.Top;
//            QueryContext.Top = 1;

//            Parent.PreExecuteChecks();

//            var toReturn = default(T);
//            var breakWhile = false;
//            QueryContext.Connection.Open();
//            using (var reader = new MereSqlDataReader<T>(QueryContext.Command.ExecuteReader(),
//                                                      QueryContext.SelectMereColumnsList))
//            {
//                while (reader.Read() && !breakWhile)
//                {
//                    toReturn = reader;

//                    breakWhile = true;
//                }
//                QueryContext.Connection.Close();
//            }

//            QueryContext.Top = startingTop;
//            //QueryHasChangeSinceLastExecute = queryHasChangedBefore;
//            return toReturn;
//        }

//        #region ExecuteExpando
//        public List<dynamic> ExecuteDynamic()
//        {
//            return ExecuteDynamic(null);
//        }

//        public List<dynamic> ExecuteDynamic<TFields>(Func<T, TFields> newFields)
//        {
//            return ExecuteDynamic(newFields(new T()));
//        }

//        public List<dynamic> ExecuteDynamic(object selectFields)
//        {
//            Parent.PreExecuteChecks();

//            if (selectFields != null)
//            {
//                QueryContext.SetFields(x => selectFields);
//            }

//            var toReturn = new List<dynamic>();

//            QueryContext.Connection.Open();
//            using (var reader = new MereSqlDataReader<T>(QueryContext.Command.ExecuteReader(),
//                                                      QueryContext.SelectMereColumnsList))
//            {
//                //using (var rd = QueryContext.Command.ExecuteReader())
//                //{
//                //var reader = new MereSqlDataReader<ExpandoObject>(rd, QueryContext.SelectFieldsHaveMutated);
//                while (reader.Read())
//                {
//                    toReturn.Add((dynamic)reader.Expando());
//                }
//                QueryContext.Connection.Close();

//            }
//            return toReturn;
//        }

//        public List<ExpandoObject> ExecuteExpando()
//        {
//            return ExecuteExpando(null);
//        }

//        public List<ExpandoObject> ExecuteExpando<TFields>(Func<T, TFields> newFields)
//        {
//            return ExecuteExpando(newFields(new T()));
//        }

//        public List<ExpandoObject> ExecuteExpando(object selectFields)
//        {
//            Parent.PreExecuteChecks();

//            if (selectFields != null)
//            {
//                QueryContext.SetFields(x => selectFields);
//            }

//            var toReturn = new List<ExpandoObject>();

//            QueryContext.Connection.Open();
//            using (var reader = new MereSqlDataReader<T>(QueryContext.Command.ExecuteReader(),
//                                                      QueryContext.SelectMereColumnsList))
//            {
//                //using (var rd = QueryContext.Command.ExecuteReader())
//                //{
//                //var reader = new MereSqlDataReader<ExpandoObject>(rd, QueryContext.SelectFieldsHaveMutated);
//                while (reader.Read())
//                {
//                    toReturn.Add(reader.Expando());
//                }
//                QueryContext.Connection.Close();

//            }
//            return toReturn;
//        }
//        #endregion

//        #region ExecuteCustomQuery
//        public List<dynamic> ExecuteCustomQueryExpandoToList(string customQuery)
//        {
//            return ExecuteCustomQueryExpandoToList(customQuery, null);
//        }

//        public List<dynamic> ExecuteCustomQueryExpandoToList(string customQuery, object parameters)
//        {
//            var toReturn = ExecuteCustomQueryExpando(customQuery, parameters);
//            return toReturn.ToList();
//        }

//        public IEnumerable<dynamic> ExecuteCustomQueryExpando(string customQuery)
//        {
//            return ExecuteCustomQueryExpando(customQuery, null);
//        }

//        public IEnumerable<dynamic> ExecuteCustomQueryExpando(string customQuery, object parameters)
//        {
//            Parent.PreExecuteChecks();

//            if (parameters != null)
//            {
//                customQuery = QueryContext.ParseParmObject(parameters, customQuery, false, true);
//            }
//            QueryContext.Command.CommandText = customQuery;
//            var toReturn = new List<ExpandoObject>();

//            QueryContext.Connection.Open();
//            using (var reader = new MereSqlDataReader<T>(QueryContext.Command.ExecuteReader(),
//                                                      QueryContext.SelectMereColumnsList))
//            {
//                //using (var rd = QueryContext.Command.ExecuteReader())
//                //{
//                //    var reader = new MereSqlDataReader<ExpandoObject>(rd, true);
//                while (reader.Read())
//                {
//                    toReturn.Add(reader.Expando());
//                }
//                QueryContext.Connection.Close();

//            }
//            return toReturn;
//        }

//        public List<T> ExecuteCustomQueryToList(string customQuery)
//        {
//            var toReturn = ExecuteCustomQuery(customQuery);
//            return toReturn.ToList();
//        }

//        public List<T> ExecuteCustomQueryToList(string customQuery, object parameters)
//        {
//            var toReturn = ExecuteCustomQuery(customQuery, parameters);
//            return toReturn.ToList();
//        }

//        public IEnumerable<T> ExecuteCustomQuery(string customQuery)
//        {
//            return ExecuteCustomQuery(customQuery, null);
//        }

//        public IEnumerable<T> ExecuteCustomQuery(string customQuery, object parameters)
//        {
//            Parent.PreExecuteChecks();

//            if (parameters != null)
//            {
//                customQuery = QueryContext.ParseParmObject(parameters, customQuery, false, true);
//            }

//            QueryContext.Command.CommandText = customQuery;

//            var toReturn = new List<T>();

//            QueryContext.Connection.Open();
//            using (var reader = new MereSqlDataReader<T>(QueryContext.Command.ExecuteReader(),
//                                                      QueryContext.SelectMereColumnsList, true))
//            {
//                //using (var rd = QueryContext.Command.ExecuteReader())
//                //{
//                //    //var found = new List<T>();
//                //    var reader = new MereSqlDataReader<T>(rd, true);
//                while (reader.Read())
//                {
//                    //if (found.Count == ChunkSize)
//                    //{
//                    //    Chunk(found.Count, found);

//                    //    found.Clear();
//                    //}

//                    //found.Add(reader);
//                    toReturn.Add(reader);
//                }

//                QueryContext.Connection.Close();

//                //if (found.Count == mereQuery.ChunkSize)
//                //{
//                //    mereQuery.Chunk(found.Count, found);

//                //    found.Clear();
//                //}
//            }

//            return toReturn;
//        }
//        #endregion

//        #region ExecuteDistinct
//        public IEnumerable<object> ExecuteDistinct()
//        {
//            return ExecuteDistinct<object>(x => null);
//        }

//        public IEnumerable<object> ExecuteDistinct<TFields>(Func<T, TFields> fields)
//        {
//            //var queryHasChangedBefore = mereQuery.QueryHasChangeSinceLastExecute;
//            var distinctBefore = QueryContext.Distinct;
//            var selectFieldsListBefore = QueryContext.SelectMereColumnsList;//QueryContext.SelectFieldsList;
//            var selectFieldsHaveMutatedBefore = QueryContext.SelectFieldsHaveMutated;

//            var newFields = fields(new T()) as object;

//            if (newFields != null)
//            {
//                var props = fields(new T()).GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();

//                //QueryContext.SelectFieldsList.Clear();
//                QueryContext.SelectMereColumnsList.Clear();

//                for (var i = 0; i < props.Count; i++)
//                {
//                    var mereColumn = QueryContext.CurMereTableMin.GetMereColumnByPropertyName(props[i].Name);
//                    if (mereColumn == null)
//                        continue;

//                    QueryContext.SelectMereColumnsList.Add(mereColumn);
//                    //QueryContext.SelectFieldsList.Add("[" + mereColumn.ColumnName + "]");
//                }
//            }

//            QueryContext.Distinct = true;

//            Parent.PreExecuteChecks();

//            QueryContext.Command.CommandText = QueryContext.Sql;
//            var toReturn = new List<object>();
//            QueryContext.Connection.Open();
//            //using (var reader = QueryContext.Command.ExecuteReader())
//            //{
//            using (var reader = new MereSqlDataReader<T>(QueryContext.Command.ExecuteReader(),
//                                                      QueryContext.SelectMereColumnsList))
//            {
//                while (reader.Read())
//                {
//                    toReturn.Add(reader);
//                }
//                QueryContext.Connection.Close();
//            }
//            QueryContext.Command.CommandText = QueryContext.Sql;

//            QueryContext.Distinct = distinctBefore;
//            //QueryContext.SelectFieldsList = selectFieldsListBefore;
//            QueryContext.SelectMereColumnsList = selectFieldsListBefore;
//            //QueryHasChangeSinceLastExecute = queryHasChangedBefore;
//            QueryContext.SelectFieldsHaveMutated = selectFieldsHaveMutatedBefore;

//            return toReturn;
//        }

//        public int ExecuteDistinctCount()
//        {
//            return ExecuteDistinctCount<object>(x => null);
//        }

//        public int ExecuteDistinctCount<TFields>(Func<T, TFields> fields)
//        {
//            //var queryHasChangedBefore = mereQuery.QueryHasChangeSinceLastExecute;
//            var distinctBefore = QueryContext.Distinct;
//            var selectFieldsListBefore = QueryContext.SelectMereColumnsList;//.SelectFieldsList;
//            var selectFieldsHaveMutatedBefore = QueryContext.SelectFieldsHaveMutated;

//            var newFields = fields(new T()) as object;

//            if (newFields != null)
//            {
//                var props = fields(new T()).GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();

//                QueryContext.SelectMereColumnsList.Clear();//.SelectFieldsList.Clear();

//                for (var i = 0; i < props.Count; i++)
//                {
//                    var mereColumn = QueryContext.CurMereTableMin.GetMereColumnByPropertyName(props[i].Name);
//                    if (mereColumn == null)
//                        continue;

//                    QueryContext.SelectMereColumnsList.Add(mereColumn);//.SelectFieldsList.Add("[" + mereColumn.ColumnName + "]");
//                }
//            }

//            QueryContext.Distinct = true;
//            Parent.PreExecuteChecks();

//            QueryContext.Command.CommandText = QueryContext.SqlForCount;
//            var toReturn = 0;
//            QueryContext.Connection.Open();
//            using (var reader = QueryContext.Command.ExecuteReader())
//            {
//                while (reader.Read())
//                {
//                    var r = (IDataRecord)reader;
//                    toReturn = (int)r[0];
//                }
//                QueryContext.Connection.Close();
//            }
//            QueryContext.Command.CommandText = QueryContext.Sql;

//            QueryContext.Distinct = distinctBefore;
//            QueryContext.SelectMereColumnsList = selectFieldsListBefore;//.SelectFieldsList = selectFieldsListBefore;
//            //QueryHasChangeSinceLastExecute = queryHasChangedBefore;
//            QueryContext.SelectFieldsHaveMutated = selectFieldsHaveMutatedBefore;

//            return toReturn;
//        }

//        public int ExecuteCount()
//        {
//            Parent.PreExecuteChecks();

//            QueryContext.Command.CommandText = QueryContext.SqlForCount;
//            var toReturn = 0;
//            QueryContext.Connection.Open();
//            using (var reader = QueryContext.Command.ExecuteReader())
//            {
//                while (reader.Read())
//                {
//                    var r = (IDataRecord)reader;
//                    toReturn = (int)r[0];
//                }
//                QueryContext.Connection.Close();
//            }
//            QueryContext.Command.CommandText = QueryContext.Sql;

//            return toReturn;
//        }
//        #endregion
//        #endregion

//        #region async executes

//        public async Task<List<T>> ExecuteAsync()
//        {
//            Parent.PreExecuteChecks();

//            var toReturn = new List<T>();

//            await QueryContext.Connection.OpenAsync();
//            using (var reader = new MereSqlDataReader<T>(await QueryContext.Command.ExecuteReaderAsync(), QueryContext.SelectMereColumnsList))
//            {
//                while (await reader.ReadAsync())
//                {
//                    toReturn.Add(reader);
//                }
//                QueryContext.Connection.Close();
//            }
//            return toReturn;
//        }

//        public async Task<T> ExecuteFirstOrDefaultAsync()
//        {
//            //var queryHasChangedBefore = QueryHasChangeSinceLastExecute;
//            var startingTop = QueryContext.Top;
//            QueryContext.Top = 1;

//            Parent.PreExecuteChecks();

//            var toReturn = default(T);
//            var breakWhile = false;
//            await QueryContext.Connection.OpenAsync();
//            using (var reader = new MereSqlDataReader<T>(await QueryContext.Command.ExecuteReaderAsync(),
//                                                      QueryContext.SelectMereColumnsList))
//            {
//                while (await reader.ReadAsync() && !breakWhile)
//                {
//                    toReturn = reader;

//                    breakWhile = true;
//                }
//                QueryContext.Connection.Close();
//            }

//            QueryContext.Top = startingTop;
//            //QueryHasChangeSinceLastExecute = queryHasChangedBefore;
//            return toReturn;
//        }

//        #region ExecuteExpando
//        public Task<List<dynamic>> ExecuteDynamicAsync()
//        {
//            return ExecuteDynamicAsync(null);
//        }

//        public Task<List<dynamic>> ExecuteDynamicAsync<TFields>(Func<T, TFields> newFields)
//        {
//            return ExecuteDynamicAsync(newFields(new T()));
//        }

//        public async Task<List<dynamic>> ExecuteDynamicAsync(object selectFields)
//        {
//            Parent.PreExecuteChecks();

//            if (selectFields != null)
//            {
//                QueryContext.SetFields(x => selectFields);
//            }

//            var toReturn = new List<dynamic>();

//            await QueryContext.Connection.OpenAsync();
//            using (var reader = new MereSqlDataReader<T>(await QueryContext.Command.ExecuteReaderAsync(),
//                                                      QueryContext.SelectMereColumnsList))
//            {
//                //using (var rd = await QueryContext.Command.ExecuteReaderAsync())
//                //{
//                //var reader = new MereSqlDataReader<ExpandoObject>(rd, QueryContext.SelectFieldsHaveMutated);
//                while (await reader.ReadAsync())
//                {
//                    toReturn.Add((dynamic)reader.Expando());
//                }
//                QueryContext.Connection.Close();

//            }
//            return toReturn;
//        }

//        public Task<List<ExpandoObject>> ExecuteExpandoAsync()
//        {
//            return ExecuteExpandoAsync(null);
//        }

//        public Task<List<ExpandoObject>> ExecuteExpandoAsync<TFields>(Func<T, TFields> newFields)
//        {
//            return ExecuteExpandoAsync(newFields(new T()));
//        }

//        public async Task<List<ExpandoObject>> ExecuteExpandoAsync(object selectFields)
//        {
//            Parent.PreExecuteChecks();

//            if (selectFields != null)
//            {
//                QueryContext.SetFields(x => selectFields);
//            }

//            var toReturn = new List<ExpandoObject>();

//            await QueryContext.Connection.OpenAsync();
//            using (var reader = new MereSqlDataReader<T>(await QueryContext.Command.ExecuteReaderAsync(),
//                                                      QueryContext.SelectMereColumnsList))
//            {
//                //using (var rd = await QueryContext.Command.ExecuteReaderAsync())
//                //{
//                //var reader = new MereSqlDataReader<ExpandoObject>(rd, QueryContext.SelectFieldsHaveMutated);
//                while (await reader.ReadAsync())
//                {
//                    toReturn.Add(reader.Expando());
//                }
//                QueryContext.Connection.Close();

//            }
//            return toReturn;
//        }
//        #endregion

//        #region ExecuteCustomQuery
//        public Task<List<dynamic>> ExecuteCustomQueryExpandoToListAsync(string customQuery)
//        {
//            return ExecuteCustomQueryExpandoToListAsync(customQuery, null);
//        }

//        public async Task<List<dynamic>> ExecuteCustomQueryExpandoToListAsync(string customQuery, object parameters)
//        {
//            var toReturn = await ExecuteCustomQueryExpandoAsync(customQuery, parameters);
//            return toReturn.ToList();
//        }

//        public Task<IEnumerable<dynamic>> ExecuteCustomQueryExpandoAsync(string customQuery)
//        {
//            return ExecuteCustomQueryExpandoAsync(customQuery, null);
//        }

//        public async Task<IEnumerable<dynamic>> ExecuteCustomQueryExpandoAsync(string customQuery, object parameters)
//        {
//            Parent.PreExecuteChecks();

//            if (parameters != null)
//            {
//                customQuery = QueryContext.ParseParmObject(parameters, customQuery, false, true);
//            }
//            QueryContext.Command.CommandText = customQuery;
//            var toReturn = new List<ExpandoObject>();

//            await QueryContext.Connection.OpenAsync();
//            using (var reader = new MereSqlDataReader<T>(await QueryContext.Command.ExecuteReaderAsync(),
//                                                      QueryContext.SelectMereColumnsList))
//            {
//                //using (var rd = await QueryContext.Command.ExecuteReaderAsync())
//                //{
//                //    var reader = new MereSqlDataReader<ExpandoObject>(rd, true);
//                while (await reader.ReadAsync())
//                {
//                    toReturn.Add(reader.Expando());
//                }
//                QueryContext.Connection.Close();

//            }
//            return toReturn;
//        }

//        public async Task<List<T>> ExecuteCustomQueryToListAsync(string customQuery)
//        {
//            var toReturn = await ExecuteCustomQueryAsync(customQuery);
//            return toReturn.ToList();
//        }

//        public async Task<List<T>> ExecuteCustomQueryToListAsync(string customQuery, object parameters)
//        {
//            var toReturn = await ExecuteCustomQueryAsync(customQuery, parameters);
//            return toReturn.ToList();
//        }

//        public Task<IEnumerable<T>> ExecuteCustomQueryAsync(string customQuery)
//        {
//            return ExecuteCustomQueryAsync(customQuery, null);
//        }

//        public async Task<IEnumerable<T>> ExecuteCustomQueryAsync(string customQuery, object parameters)
//        {
//            Parent.PreExecuteChecks();

//            if (parameters != null)
//            {
//                customQuery = QueryContext.ParseParmObject(parameters, customQuery, false, true);
//            }

//            QueryContext.Command.CommandText = customQuery;

//            var toReturn = new List<T>();

//            await QueryContext.Connection.OpenAsync();
//            using (var reader = new MereSqlDataReader<T>(await QueryContext.Command.ExecuteReaderAsync(),
//                                                      QueryContext.SelectMereColumnsList, true))
//            {
//                //using (var rd = await QueryContext.Command.ExecuteReaderAsync())
//                //{
//                //    //var found = new List<T>();
//                //    var reader = new MereSqlDataReader<T>(rd, true);
//                while (await reader.ReadAsync())
//                {
//                    //if (found.Count == ChunkSize)
//                    //{
//                    //    Chunk(found.Count, found);

//                    //    found.Clear();
//                    //}

//                    //found.Add(reader);
//                    toReturn.Add(reader);
//                }

//                QueryContext.Connection.Close();

//                //if (found.Count == mereQuery.ChunkSize)
//                //{
//                //    mereQuery.Chunk(found.Count, found);

//                //    found.Clear();
//                //}
//            }

//            return toReturn;
//        }
//        #endregion

//        #region ExecuteDistinct
//        public Task<IEnumerable<object>> ExecuteDistinctAsync()
//        {
//            return ExecuteDistinctAsync<object>(x => null);
//        }

//        public async Task<IEnumerable<object>> ExecuteDistinctAsync<TFields>(Func<T, TFields> fields)
//        {
//            //var queryHasChangedBefore = mereQuery.QueryHasChangeSinceLastExecute;
//            var distinctBefore = QueryContext.Distinct;
//            var selectFieldsListBefore = QueryContext.SelectMereColumnsList;//QueryContext.SelectFieldsList;
//            var selectFieldsHaveMutatedBefore = QueryContext.SelectFieldsHaveMutated;

//            var newFields = fields(new T()) as object;

//            if (newFields != null)
//            {
//                var props = fields(new T()).GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();

//                //QueryContext.SelectFieldsList.Clear();
//                QueryContext.SelectMereColumnsList.Clear();

//                for (var i = 0; i < props.Count; i++)
//                {
//                    var mereColumn = QueryContext.CurMereTableMin.GetMereColumnByPropertyName(props[i].Name);
//                    if (mereColumn == null)
//                        continue;

//                    QueryContext.SelectMereColumnsList.Add(mereColumn);
//                    //QueryContext.SelectFieldsList.Add("[" + mereColumn.ColumnName + "]");
//                }
//            }

//            QueryContext.Distinct = true;

//            Parent.PreExecuteChecks();

//            QueryContext.Command.CommandText = QueryContext.Sql;
//            var toReturn = new List<object>();
//            await QueryContext.Connection.OpenAsync();
//            //using (var reader = await QueryContext.Command.ExecuteReaderAsync())
//            //{
//            using (var reader = new MereSqlDataReader<T>(await QueryContext.Command.ExecuteReaderAsync(),
//                                                      QueryContext.SelectMereColumnsList))
//            {
//                while (await reader.ReadAsync())
//                {
//                    toReturn.Add(reader);
//                }
//                QueryContext.Connection.Close();
//            }
//            QueryContext.Command.CommandText = QueryContext.Sql;

//            QueryContext.Distinct = distinctBefore;
//            //QueryContext.SelectFieldsList = selectFieldsListBefore;
//            QueryContext.SelectMereColumnsList = selectFieldsListBefore;
//            //QueryHasChangeSinceLastExecute = queryHasChangedBefore;
//            QueryContext.SelectFieldsHaveMutated = selectFieldsHaveMutatedBefore;

//            return toReturn;
//        }

//        public Task<int> ExecuteDistinctCountAsync()
//        {
//            return ExecuteDistinctCountAsync<object>(x => null);
//        }

//        public async Task<int> ExecuteDistinctCountAsync<TFields>(Func<T, TFields> fields)
//        {
//            //var queryHasChangedBefore = mereQuery.QueryHasChangeSinceLastExecute;
//            var distinctBefore = QueryContext.Distinct;
//            var selectFieldsListBefore = QueryContext.SelectMereColumnsList;//.SelectFieldsList;
//            var selectFieldsHaveMutatedBefore = QueryContext.SelectFieldsHaveMutated;

//            var newFields = fields(new T()) as object;

//            if (newFields != null)
//            {
//                var props = fields(new T()).GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();

//                QueryContext.SelectMereColumnsList.Clear();//.SelectFieldsList.Clear();

//                for (var i = 0; i < props.Count; i++)
//                {
//                    var mereColumn = QueryContext.CurMereTableMin.GetMereColumnByPropertyName(props[i].Name);
//                    if (mereColumn == null)
//                        continue;

//                    QueryContext.SelectMereColumnsList.Add(mereColumn);//.SelectFieldsList.Add("[" + mereColumn.ColumnName + "]");
//                }
//            }

//            QueryContext.Distinct = true;
//            Parent.PreExecuteChecks();

//            QueryContext.Command.CommandText = QueryContext.SqlForCount;
//            var toReturn = 0;
//            await QueryContext.Connection.OpenAsync();
//            using (var reader = await QueryContext.Command.ExecuteReaderAsync())
//            {
//                while (await reader.ReadAsync())
//                {
//                    var r = (IDataRecord)reader;
//                    toReturn = (int)r[0];
//                }
//                QueryContext.Connection.Close();
//            }
//            QueryContext.Command.CommandText = QueryContext.Sql;

//            QueryContext.Distinct = distinctBefore;
//            QueryContext.SelectMereColumnsList = selectFieldsListBefore;//.SelectFieldsList = selectFieldsListBefore;
//            //QueryHasChangeSinceLastExecute = queryHasChangedBefore;
//            QueryContext.SelectFieldsHaveMutated = selectFieldsHaveMutatedBefore;

//            return toReturn;
//        }

//        public async Task<int> ExecuteCountAsync()
//        {
//            Parent.PreExecuteChecks();

//            QueryContext.Command.CommandText = QueryContext.SqlForCount;
//            var toReturn = 0;
//            await QueryContext.Connection.OpenAsync();
//            using (var reader = await QueryContext.Command.ExecuteReaderAsync())
//            {
//                while (await reader.ReadAsync())
//                {
//                    var r = (IDataRecord)reader;
//                    toReturn = (int)r[0];
//                }
//                QueryContext.Connection.Close();
//            }
//            QueryContext.Command.CommandText = QueryContext.Sql;

//            return toReturn;
//        }
//        #endregion

//        #region ExecuteSimpleQuery
//        //public Task<IEnumerable<T>> ExecuteSimpleQueryAsync<T, TFilterObject>(TFilterObject filterObject)
//        //{
//        //    var q = MereQuery<T>.Create(new List<object> { filterObject });
//        //    return q.ExecuteAsync();
//        //}

//        //public static Task<IEnumerable<T>> ExecuteSimpleQueryAsync<T>(this MereQuery<T> mereQuery, int top)
//        //{
//        //    var q = MereQuery<T>.Create(top);
//        //    return q.ExecuteAsync();
//        //}

//        //public static Task<IEnumerable<T>> ExecuteSimpleQueryAsync<T>(this MereQuery<T> mereQuery, int top, object filterObject)
//        //{
//        //    var q = MereQuery<T>.Create(new List<object> { filterObject }, top);
//        //    return q.ExecuteAsync();
//        //}

//        //public static Task<IEnumerable<T>> ExecuteSimpleQueryAsync<T>(this MereQuery<T> mereQuery, string whereSql, object whereObject)
//        //{
//        //    var q = MereQuery<T>.Create();
//        //    q.Where(whereSql, whereObject);
//        //    return q.ExecuteAsync();
//        //}

//        //public static Task<IEnumerable<T>> ExecuteSimpleQueryAsync<T>(this MereQuery<T> mereQuery, int top, string whereSql, object whereObject)
//        //{
//        //    var q = MereQuery<T>.Create(top);
//        //    q.Where(whereSql, whereObject);
//        //    return q.ExecuteAsync();
//        //}
//        #endregion

//        #region BulkHelpers
//        public async Task<int> BulkCopyToAsync<TDest>(int batchSize = 1000)
//        {
//            Parent.PreExecuteChecks();

//            var mereTableDest = MereUtils.CacheCheckFile<TDest>();
//            var destConn = mereTableDest.GetConnection();
//            var cnt = 0;


//            await QueryContext.Connection.OpenAsync();
//            await destConn.OpenAsync();

//            using (var reader = await QueryContext.Command.ExecuteReaderAsync())
//            {

//                using (var s = new SqlBulkCopy(destConn))
//                {
//                    foreach (var col in QueryContext.SelectMereColumnsList)
//                    {
//                        s.ColumnMappings.Add(col.ColumnName, col.ColumnName);
//                    }

//                    s.EnableStreaming = true;
//                    s.BatchSize = batchSize;
//                    s.BulkCopyTimeout = QueryContext.Timeout;
//                    s.DestinationTableName = mereTableDest.TableName;

//                    await s.WriteToServerAsync(reader);

//                    s.Close();

//                    //if (RowsCopiedField != null)
//                    //    cnt = (int)RowsCopiedField.GetValue(s);
//                }
//            }

//            QueryContext.Connection.Close();
//            destConn.Close();
//            return cnt;
//        }

//        public async Task<IEnumerable<T>> BulkCopyToChunkedParallel<TDest>(int minChunkSize, int maxChunkSize, int threadThrottle = 150, int timeout = 0)
//        {
//            Parent.PreExecuteChecks();

//            var mereTableDest = MereUtils.CacheCheckFile<TDest>();

//            return await ExecuteChunkedParallel(
//                (pCnt, chunk) =>
//                {
//                    var internalChunk = chunk.ToList();
//                    var destConn = mereTableDest.GetConnection();
//                    destConn.Open();
//                    using (var s = new SqlBulkCopy(destConn))
//                    {
//                        foreach (var col in mereTableDest.PropertyHelpersColumnName.Keys)
//                        {
//                            s.ColumnMappings.Add(col, col);
//                        }
//                        s.EnableStreaming = true;
//                        s.BatchSize = internalChunk.Count;
//                        s.BulkCopyTimeout = timeout;
//                        s.DestinationTableName = mereTableDest.TableName;

//                        s.WriteToServer(new MereDataReader<T>(internalChunk));
//                        s.Close();

//                    }
//                    destConn.Close();
//                }, minChunkSize, maxChunkSize, threadThrottle);
//        }
//        #endregion

//        #region ExecuteChunkedParallel
//        public Task<IEnumerable<T>> ExecuteChunkedParallel(Action<int, IEnumerable<T>> perChunk, int minChunkSize,
//                                                           int maxChunkSize = 0, int threadThrottle = 150)
//        {
//            return ExecuteChunkedParallelInternal(perChunk, null, null, null, minChunkSize, maxChunkSize, threadThrottle);
//        }

//        public Task<IEnumerable<T>> ExecuteChunkedParallel(Func<int, IEnumerable<T>, IEnumerable<T>> perChunk, int minChunkSize,
//                                                           int maxChunkSize = 0, int threadThrottle = 150)
//        {
//            return ExecuteChunkedParallelInternal(null, null, perChunk, null, minChunkSize, maxChunkSize, threadThrottle);
//        }

//        public Task<IEnumerable<T>> ExecuteChunkedParallel(Action<IEnumerable<T>> perChunk, int minChunkSize,
//                                                           int maxChunkSize = 0, int threadThrottle = 150)
//        {
//            return ExecuteChunkedParallelInternal(null, perChunk, null, null, minChunkSize, maxChunkSize, threadThrottle);
//        }

//        public Task<IEnumerable<T>> ExecuteChunkedParallel(Func<IEnumerable<T>, IEnumerable<T>> perChunk, int minChunkSize,
//                                                           int maxChunkSize = 0, int threadThrottle = 150)
//        {
//            return ExecuteChunkedParallelInternal(null, null, null, perChunk, minChunkSize, maxChunkSize, threadThrottle);
//        }

//        private async Task<IEnumerable<T>> ExecuteChunkedParallelInternal(
//            Action<int, IEnumerable<T>> perChunkAct,
//            Action<IEnumerable<T>> perChunkAct2,
//            Func<int, IEnumerable<T>, IEnumerable<T>> perChunkFunc,
//            Func<IEnumerable<T>, IEnumerable<T>> perChunkFunc2,
//            int minChunkSize, int maxChunkSize = 0, int threadThrottle = 150)
//        {
//            var taskWatcher = TaskWatcher.StartNew(threadThrottle == 0 ? 1 : threadThrottle);

//            var toReturn = new List<T>();
//            var found = new List<T>();
//            var processedCnt = 0;

//            Parent.PreExecuteChecks();

//            var myCn = QueryContext.Connection;

//            await myCn.OpenAsync();
//            using (var reader = new MereSqlDataReader<T>(await QueryContext.Command.ExecuteReaderAsync(),
//                                                      QueryContext.SelectMereColumnsList))
//            {
//                //using (var rd = await QueryContext.Command.ExecuteReaderAsync())
//                //{
//                //    var reader = new MereSqlDataReader<T>(rd, (!QueryContext.SelectFieldsHaveMutated));

//                while (await reader.ReadAsync())
//                {

//                    processedCnt++;

//                    found.Add(reader);

//                    if (((minChunkSize <= 1 || processedCnt != 1) && (processedCnt % minChunkSize == 0 && !taskWatcher.HasPendingTasks) || (found.Count >= maxChunkSize)))
//                    {
//                        var chunk = new List<T>();
//                        chunk.AddRange(found);
//                        var cnt = processedCnt;
//                        taskWatcher.AddTask(new Task(() => toReturn.AddRange(InternalParallelHelper(perChunkAct, perChunkAct2,
//                                                                                                    perChunkFunc, perChunkFunc2,
//                                                                                                    cnt, chunk))));

//                        found = new List<T>();
//                    }

//                }

//                if (found.Count > 0)
//                {
//                    taskWatcher.AddTask(new Task(() => toReturn.AddRange(InternalParallelHelper(perChunkAct, perChunkAct2, perChunkFunc, perChunkFunc2,
//                                           processedCnt, found))));
//                }

//                QueryContext.Connection.Close();

//                while (taskWatcher.HasTasks)
//                {
//                    await Task.Delay(10);
//                }

//                //mereSelect.QueryHasChangeSinceLastExecute = false;
//                return toReturn;
//            }
//        }

//        private static List<T> InternalParallelHelper<T>(Action<int, IEnumerable<T>> perChunkAct,
//            Action<IEnumerable<T>> perChunkAct2,
//            Func<int, IEnumerable<T>, IEnumerable<T>> perChunkFunc,
//            Func<IEnumerable<T>, IEnumerable<T>> perChunkFunc2,
//            int processedCnt, List<T> chunk)
//        {
//            var toReturn = new List<T>();
//            if (perChunkAct != null)
//            {
//                perChunkAct(processedCnt, chunk);
//                toReturn.AddRange(chunk);
//            }
//            else if (perChunkFunc != null)
//            {
//                var processed = perChunkFunc(processedCnt, chunk);
//                toReturn.AddRange(processed);
//            } if (perChunkAct2 != null)
//            {
//                perChunkAct2(chunk);
//                toReturn.AddRange(chunk);
//            }
//            else if (perChunkFunc2 != null)
//            {
//                var processed = perChunkFunc2(chunk);
//                toReturn.AddRange(processed);
//            }
//            return toReturn;
//        }
//        #endregion
//        #endregion
//    }
//}
