using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Mere
{
    public class MereContext<T> : IDisposable where T : new()
    {
        #region constructors
        public MereContext() :
            this(MereContextType.Query, MereUtils.GetDataSource<T>()) { }

        public MereContext(MereContextType contextType) :
            this(contextType, MereUtils.GetDataSource<T>()) { }

        //main/base
        public MereContext(MereContextType contextType, MereDataSource dataSource)
        {
            CurMereContextType = contextType;
            CurEnvironment = MereUtils.MereEnvironment;
            BaseMereDataSource = dataSource;

            MereEnvironmentSubscriptionId = MereUtils.OnMereEnvironmentChanged(EnvironmentChanged);
            EnvironmentChanged(CurEnvironment);

            CurMereTableMin = MereUtils.CacheCheck<T>();
            Connection = CurMereTableMin.GetConnection();
            ParamNames = new List<string>();
            Parameters = new List<SqlParameter>();
            _connectionStringBase = "Data Source={0};Initial Catalog={1};User ID={2};Password={3};Asynchronous Processing=True;MultipleActiveResultSets=True;Timeout={4}";

            OrderByList = new List<string>();
            UpdateFieldsDictionary = new Dictionary<string, object>();
            SelectMereColumnsList = CurMereTableMin.SelectMereColumns;
        }

        public MereContext(MereContextType contextType, MereDataSource dataSource, int top) :
            this(contextType, dataSource)
        {
            Top = top;
        }

        public MereContext(MereDataSource dataSource) :
            this(MereContextType.Query, dataSource) { }

        public MereContext(int top)
            : this()
        {
            Top = top;
        }
        public MereContext(MereDataSource dataSource, int top) :
            this(MereContextType.Query, dataSource)
        {
            Top = top;
        }
        #endregion

        #region properties
        public int MereEnvironmentSubscriptionId { get; set; }
        public string CurEnvironment { get; set; }
        public MereContextType CurMereContextType { get; set; }
        public MereDataSource CurMereDataSource { get; set; }
        public MereDataSource BaseMereDataSource { get; set; }
        public List<SqlParameter> Parameters { get; set; }
        public SqlCommand Command { get; set; }
        public SqlConnection Connection { get; set; }
        public string TableName { get; set; }
        public string DatabaseName { get; set; }
        private readonly string _connectionStringBase;
        public string ConnectionStringBase { get { return _connectionStringBase; } }
        public string ConnectionString
        {
            get
            {
                return ConnectionStringBase == null ? "" : string.Format(ConnectionStringBase, ServerName, DatabaseName,
                                     UserId, Password, Timeout);
            }
        }

        public string ServerName { get; set; }
        public int Timeout { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }

        public MereTableMin<T> CurMereTableMin { get; set; }

        public bool BuildingFilterGroup { get; set; }
        public bool CloseFilterGroup { get; set; }

        public MereFilterGroup CurMereFilterGroup { get; set; }

        public MereColumn CurFilterMereColumn { get; set; }
        public string CurFilterColumnName { get; set; }
        public string CurFilterPropertyName { get; set; }
        public bool CurFilterOr { get; set; }
        public bool CurFilterGroupOr { get; set; }
        public bool CurFilterInnerGroup { get; set; }
        public int CurFilterBackup { get; set; }

        public List<string> ParamNames { get; set; }

        public List<MereFilterGroup> WhereChunks { get; set; }

        public string WhereStr
        {
            get
            {

                if (CurMereFilterGroup != null && CurMereFilterGroup.HasFilters)
                {
                    var whereChunks = new List<MereFilterGroup>();
                    if (WhereChunks != null && WhereChunks.Any())
                        whereChunks.AddRange(WhereChunks);

                    whereChunks.Add(CurMereFilterGroup);

                    return whereChunks.Count > 0 ? " WHERE " + whereChunks.First().WhereString + string.Join(" ", whereChunks.Skip(1).Select(s => s.AndOr + s.WhereString)) : "";
                }

                return WhereChunks != null && WhereChunks.Count > 0 ? " WHERE " + WhereChunks.First().WhereString + string.Join(" ", WhereChunks.Skip(1).Select(s => s.AndOr + s.WhereString)) : "";
            }
        }

        private string _sql;
        public string Sql
        {
            get
            {
                switch (CurMereContextType)
                {
                    case MereContextType.Query:
                        return SqlSelect;
                    case MereContextType.Custom:
                        return _sql ?? SqlSelect;
                    case MereContextType.Delete:
                        return SqlDelete;
                    case MereContextType.Insert:
                        return SqlInsert;
                    case MereContextType.NonQuery:
                        return SqlSelect;
                    case MereContextType.Save:
                        return CurMereTableMin.GetUpsertSqlWithKey();
                    case MereContextType.Update:
                        return SqlUpdate;
                    case MereContextType.UpdateWithUpdateFields:
                        return SqlUpdateUsingUpdateFields;
                    default:
                        return SqlSelect;
                }
            }
            set { _sql = value; }
        }

        #region query
        public bool SelectFieldsHaveMutated { get; set; }
        private string _selectFields;
        public string SelectFields
        {
            get { return _selectFields; }
            //get { return SelectFieldsList != null ? string.Join(", ", SelectFieldsList) : null; }
            //set
            //{
            //    if (value != null)
            //    {
            //        SelectFieldsList = value.Split(',').ToList();
            //    }
            //    else
            //    {
            //        SelectFieldsList = null;
            //    }
            //}
        }

        //public List<string> SelectFieldsList { get; set; }
        private List<MereColumn> _selectMereColumnsList;
        public List<MereColumn> SelectMereColumnsList
        {
            get { return _selectMereColumnsList; }
            set
            {
                _selectFields = value != null ? string.Join(", ", value.Select(x => "[" + x.ColumnName + "]")) : null;
                _selectMereColumnsList = value;
            }
        }

        private int? _top;
        public int? Top
        {
            get { return _top; }
            set
            {
                //QueryHasChangeSinceLastExecute = true;
                _top = value;
            }
        }

        private bool _distinct;
        public bool Distinct
        {
            get { return _distinct; }
            set
            {
                //QueryHasChangeSinceLastExecute = true;
                _distinct = value;
            }
        }

        public List<string> OrderByList { get; set; }

        public string OrderByStr
        {
            get { return OrderByList != null && OrderByList.Any() ? " ORDER BY " + string.Join(", ", OrderByList) : ""; }
        }

        public string SqlSelect
        {
            get
            {
                var distinct = Distinct ? " DISTINCT " : "";
                return Top != null
                           ? "SELECT " + distinct + " TOP " + Top + " " + SelectFields + " FROM " + TableName + WhereStr + OrderByStr
                           : "SELECT " + distinct + " " + SelectFields + " FROM " + TableName + WhereStr + OrderByStr;
            }
        }
        #endregion

        #region insert
        public string SqlInsert { get { return CurMereTableMin.SqlInsert; } }
        #endregion

        #region delete
        public string SqlDelete { get { return "DELETE FROM " + CurMereTableMin.TableName + WhereStr; } }
        #endregion

        #region update
        public string UpdateFields
        {
            get { return (UpdateFieldsDictionary != null && UpdateFieldsDictionary.Any()) ? "UPDATE " + TableName + " SET " + string.Join(", ", UpdateFieldsDictionary.Select(x => x.Key + "=@" + x.Key)) : SqlUpdateNotUsingUpdateFields; }
        }
        public Dictionary<string, object> UpdateFieldsDictionary { get; set; }
        public string SqlUpdateUsingUpdateFields { get { return UpdateFields + WhereStr; } }
        public string SqlUpdateNotUsingUpdateFields { get { return CurMereTableMin.SqlUpdateWithoutKey; } }
        public string SqlUpdate { get { return CurMereTableMin.SqlUpdateWithoutKey + WhereStr; } }

        public string SqlForCount
        {
            get
            {
                return "SELECT COUNT(0) AS Count FROM " + TableName + WhereStr;
            }
        }
        #endregion
        #endregion

        #region base methods

        public void EnvironmentChanged(string environment)
        {
            var mereTable = MereUtils.CacheCheck<T>();
            if (CurMereTableMin == null) CurMereTableMin = mereTable;

            CurMereDataSource = BaseMereDataSource[environment];
            ServerName = CurMereDataSource.ServerName ?? CurMereTableMin.ServerName;
            DatabaseName = CurMereDataSource.DatabaseName ?? CurMereTableMin.DatabaseName;
            TableName = CurMereDataSource.TableName ?? CurMereTableMin.TableName;

            UserId = CurMereDataSource.UserId ?? CurMereTableMin.UserId;
            Password = CurMereDataSource.Password ?? CurMereTableMin.Password;
        }

        public void PreExecuteChecks()
        {
            UpdateConnection();
            UpdateCommand();
        }

        private void UpdateConnection()
        {
            Connection = new SqlConnection(ConnectionString);
        }

        private void UpdateCommand()
        {
            Command = Connection.CreateCommand();
            for (var i = 0; i < Parameters.Count; i++)
                Command.Parameters.Add(Parameters);

            Command.CommandText = Sql;
            Command.CommandTimeout = Timeout;
        }

        public SqlConnection GetConnection()
        {
            Debug.WriteLine("Get connection TableName: {0}", TableName);
            return new SqlConnection(ConnectionString);
        }

        public SqlCommand GetCommand()
        {
            return GetCommand(Parameters);
        }

        public SqlCommand GetCommand(bool includeContextsParameters)
        {
            if (includeContextsParameters)
                return GetCommand(Parameters);

            return GetCommand(null);
        }

        public SqlCommand GetCommand(List<SqlParameter> parameters)
        {
            var conn = GetConnection();
            var cmd = conn.CreateCommand();
            if (parameters != null)
                for (var i = 0; i < parameters.Count; i++)
                    cmd.Parameters.Add(new SqlParameter(parameters[i].ParameterName, parameters[i].Value));

            cmd.CommandText = Sql;
            cmd.CommandTimeout = Timeout;

            return cmd;
        }

        public void Credentials(string serverName, string databaseName, string userId, string password)
        {
            ServerName = serverName;
            DatabaseName = databaseName;
            UserId = userId;
            Password = password;
        }

        public void Database(string databaseName)
        {
            DatabaseName = databaseName;
        }

        public void Server(string serverName)
        {
            ServerName = serverName;
        }

        public void User(string userId)
        {
            UserId = userId;
        }

        public void PasswordSetter(string password)
        {
            Password = password;
        }

        #endregion

        #region methods

        public void SetFields<TFields>()
        {
            var props = typeof(TFields).GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();
            //             var props = new TFields().GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();
            //             var props = Activator.CreateInstance<TFields>().GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();

            var newColumnsList = new List<MereColumn>();
            for (var i = 0; i < props.Count; i++)
            {
                var mereColumn = CurMereTableMin.GetMereColumn(props[i].Name);
                if (mereColumn == null)
                    continue;

                newColumnsList.Add(mereColumn);
            }
            SelectMereColumnsList = newColumnsList;
            SelectFieldsHaveMutated = true;
        }


        public void SetFields<TFields>(Func<T, TFields> newFields)
        {
            var props = typeof(TFields).GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();
            //            var props = newFields(new T()).GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();

            var newColumnsList = new List<MereColumn>();
            for (var i = 0; i < props.Count; i++)
            {
                var mereColumn = CurMereTableMin.GetMereColumnByPropertyName(props[i].Name);
                if (mereColumn == null)
                    continue;

                newColumnsList.Add(mereColumn);
            }
            SelectMereColumnsList = newColumnsList;
            SelectFieldsHaveMutated = true;
        }

        public void SetFields(dynamic fields)
        {
            var newColumnsList = new List<MereColumn>();

            foreach (var d in fields)
            {
                var mereColumn = CurMereTableMin.GetMereColumn(d.Name);
                if (mereColumn == null)
                    continue;

                newColumnsList.Add(mereColumn);
            }
            SelectMereColumnsList = newColumnsList;
            SelectFieldsHaveMutated = true;
        }
        #endregion

        #region update methods
        public void AddUpdateField<TProp>(Expression<Func<T, TProp>> field, TProp value)
        {
            var colName = CurMereTableMin.GetMereColumn(field).ColumnName;
            AddUpdateField(colName, value);
        }

        public void AddUpdateField(string colName, object value)
        {
            if (UpdateFieldsDictionary.ContainsKey(colName))
                UpdateFieldsDictionary[colName] = value;
            else
                UpdateFieldsDictionary.Add(colName, value);
        }

        public void AddUpdateField(IEnumerable<KeyValuePair<string, object>> fields)
        {
            AddUpdateField(fields, true);
        }

        public void AddUpdateField(IEnumerable<KeyValuePair<string, object>> fields, bool filterWithKeyAndOrIdentity)
        {
            foreach (var p in fields)
            {
                var mereColumn = CurMereTableMin.GetMereColumnByColumnName(p.Key);

                if (mereColumn == null)
                    continue;

                if (mereColumn.IsIdentity || mereColumn.IsKey)
                {
                    if (filterWithKeyAndOrIdentity)
                    {
                        InitFilterAdd(mereColumn.ColumnName);
                        AddFilter(p.Value, SqlOperator.EqualTo);
                    }

                    if (mereColumn.IsIdentity)
                        continue;
                }

                AddUpdateField(mereColumn.ColumnName, p.Value);
            }
        }
        #endregion

        #region filter methods
        public void ResetCurFilterData()
        {
            CurFilterMereColumn = null;
            CurFilterColumnName = null;
            CurFilterPropertyName = null;
            CurFilterInnerGroup = false;
            CurFilterBackup = 0;
            CurFilterOr = false;
            CurFilterGroupOr = false;
        }

        public void InitFilterAdd(string colName, bool or = false)
        {
            CurFilterMereColumn = CurMereTableMin.GetMereColumnByColumnName(colName);
            if (CurFilterMereColumn == null)
            {
                ResetCurFilterData();
                throw new Exception("Invalid column name");
            }

            CurFilterColumnName = CurFilterMereColumn.ColumnName;
            CurFilterPropertyName = CurFilterMereColumn.PropertyName;
            CurFilterOr = or;
            CurFilterBackup = 0;
            CurFilterInnerGroup = false;
        }
        public void InitFilterAdd<TProp>(Expression<Func<T, TProp>> prop, bool or = false)
        {
            CurFilterMereColumn = CurMereTableMin.GetMereColumn(prop);
            if (CurFilterMereColumn == null)
            {
                ResetCurFilterData();
                return;
            }
            CurFilterColumnName = CurFilterMereColumn.ColumnName;
            CurFilterPropertyName = CurFilterMereColumn.PropertyName;
            CurFilterOr = or;
            CurFilterBackup = 0;
            CurFilterInnerGroup = false;
        }

        public void InitFilterGroupAdd<TProp>(Expression<Func<T, TProp>> prop, bool or = false, bool innerGroup = false, int backup = 0)
        {
            if (!innerGroup)
            {
                CloseFilterGroup = true;
                BuildingFilterGroup = true;
            }
            CurFilterGroupOr = or;
            CurFilterOr = or;
            CurFilterMereColumn = CurMereTableMin.GetMereColumn(prop);
            if (CurFilterMereColumn == null)
            {
                ResetCurFilterData();
                return;
            }
            CurFilterColumnName = CurFilterMereColumn.ColumnName;
            CurFilterPropertyName = CurFilterMereColumn.PropertyName;
            CurFilterInnerGroup = innerGroup;
            CurFilterBackup = backup;
        }

        public void AddFilter(string whereSql, object whereValues = null, bool or = false)
        {
            var p = Parameters;
            var self = this;
            //made up update added false for param only and added everything after 5-12-2014
            MereUtils.ParseParmObject(ref self, ref p, whereValues, whereSql, or, false, CurFilterInnerGroup, CurFilterBackup);
            ResetCurFilterData();
        }

        public void AddFilter(object value, SqlOperator sqlOperator)
        {

            if (CloseFilterGroup)
            {
                if (CurMereFilterGroup != null && CurMereFilterGroup.HasFilters)
                {

                    WhereChunks.Add(CurMereFilterGroup);
                    CurMereFilterGroup = new MereFilterGroup(CurFilterOr);
                }

                CloseFilterGroup = false;

            }

            var p = Parameters;
            var self = this;
            var toAdd = MereUtils.ParseParamValue(ref self, ref p, CurFilterPropertyName, value, null, SqlOperatorHelper.GetString(sqlOperator));

            //could throw error
            if (toAdd == null)
                return;

            if (WhereChunks == null)
                WhereChunks = new List<MereFilterGroup>();

            if (CurMereFilterGroup == null)
                CurMereFilterGroup = new MereFilterGroup(CurFilterOr);

            CurMereFilterGroup.AddFilter(CurFilterOr ? " OR " : " AND ", toAdd, CurFilterInnerGroup, CurFilterBackup);

            ResetCurFilterData();

        }

        public void AddFilterGroup(string whereSql, object whereValues = null, bool or = false, bool innerGroup = false, int backup = 0)
        {
            if (!innerGroup)
            {
                CloseFilterGroup = true;
                BuildingFilterGroup = true;
            }

            var p = Parameters;
            var self = this;
            MereUtils.ParseParmObject(ref self, ref p, whereValues, whereSql, CurFilterOr, false, innerGroup, backup);
            ResetCurFilterData();
        }

        public void InitFilterGroupAdd(Expression<Func<T, object>> prop, bool or = false, bool innerGroup = false, int backup = 0)
        {
            BuildingFilterGroup = true;
            CloseFilterGroup = true;

            var mereColumn = CurMereTableMin.GetMereColumn(prop);
            if (mereColumn == null)
            {
                ResetCurFilterData();
                return;
            }

            CurFilterMereColumn = mereColumn;
            CurFilterColumnName = mereColumn.ColumnName;
            CurFilterPropertyName = mereColumn.PropertyName;
            CurFilterOr = or;
            CurFilterBackup = backup;
            CurFilterInnerGroup = innerGroup;

            if (!innerGroup)
            {
                CloseFilterGroup = true;
                BuildingFilterGroup = true;
            }

        }

        public void AddFilterGroup(Expression<Func<T, object>> prop, object value, SqlOperator sqlOperator)
        {
            BuildingFilterGroup = true;
            CloseFilterGroup = true;

            if (!CurFilterInnerGroup)
            {
                CloseFilterGroup = true;
                BuildingFilterGroup = true;
            }

            AddFilter(value, sqlOperator);
        }

        public void AddFilterIn<TProp>(IEnumerable<TProp> values)
        {
            InternalAddFilterIn(values);
        }

        public void AddFilterInCaseSensitive<TProp>(IEnumerable<TProp> values)
        {
            InternalAddFilterIn(values, true);
        }

        public void AddFilterNotIn<TProp>(IEnumerable<TProp> values)
        {
            InternalAddFilterIn(values, false, true);
        }

        public void AddFilterNotInCaseSensitive<TProp>(IEnumerable<TProp> values)
        {
            InternalAddFilterIn(values, true, true);
        }

        private void InternalAddFilterIn<TProp>(IEnumerable<TProp> values, bool caseSensitive = false, bool notIn = false)
        {
            var whereChunk = new StringBuilder();

            if (CurFilterMereColumn == null)
                return;


            if (whereChunk.Length > 0)
                whereChunk.Append(" AND ");

            //var cnt = 0;
            string forIn;
            var proptypeFullname = CurFilterMereColumn.PropertyDescriptor.PropertyType.FullName.ToLower();
            if (proptypeFullname.Contains("string")
                     || proptypeFullname.Contains("date")
                     || proptypeFullname.Contains("guid"))
            {
                forIn = "'" + string.Join("', '", values) + "'";
            }
            else
            {
                forIn = string.Join(", ", values);
            }

            var inOp = notIn ? " NOT IN " : " IN ";
            var op = caseSensitive ? " COLLATE Latin1_General_CS_AS " + inOp : inOp;
            whereChunk.AppendFormat(" [{0}] {1} ({2}) ", CurFilterMereColumn.ColumnName, op, forIn);

            if (WhereChunks == null)
                WhereChunks = new List<MereFilterGroup>();

            AddFilter(whereChunk.ToString(), null, CurFilterOr);
        }

        private void InternalAddFilterBetween(object value1, object value2, bool notBetween = false)
        {
            var whereChunk = new StringBuilder();

            var pName1 = MereUtils.NoDupNameCheck("@" + CurFilterColumnName, ParamNames);
            ParamNames.Add(pName1);

            var parm1 = new SqlParameter();
            parm1.ParameterName = pName1;
            parm1.Value = value1;

            var pName2 = MereUtils.NoDupNameCheck("@" + CurFilterColumnName, ParamNames);
            ParamNames.Add(pName2);

            var parm2 = new SqlParameter();
            parm2.ParameterName = pName2;
            parm2.Value = value2;

            var betweenOp = notBetween ? " NOT BETWEEN " : " BETWEEN ";

            Parameters.Add(parm1);
            Parameters.Add(parm2);
            whereChunk.AppendFormat(" [{0}] {1} {2} AND {3}", CurFilterColumnName, betweenOp, pName1, pName2);

            if (WhereChunks == null)
                WhereChunks = new List<MereFilterGroup>();

            AddFilter(whereChunk.ToString(), null, CurFilterOr);

        }

        public void AddFilterBetween(object value1, object value2)
        {
            InternalAddFilterBetween(value1, value2, false);
        }

        public void AddFilterNotBetween(object value1, object value2)
        {
            InternalAddFilterBetween(value1, value2, true);
        }

        //         public void AddFilterBetween(Expression<Func<T, object>> prop, object value1, object value2)
        //         {
        //             AddFilterBetween(value1, value2);
        //         }

        //         public void AddFilterNotBetween(Expression<Func<T, object>> prop, object value1, object value2)
        //         {
        //             AddFilterNotBetween(value1, value2);
        //         }
        #endregion

        #region RunQuery
        public IEnumerable<ExpandoObject> RunQueryExpando(string sql, List<SqlParameter> parameters)
        {
            using (var cmd = parameters != null ? GetCommand(parameters) : GetCommand(false))
            {
                cmd.CommandText = sql ?? Sql;
                cmd.CommandTimeout = Timeout;

                cmd.Connection.Open();
                using (var reader = new MereSqlDataReader<T>(cmd.ExecuteReader()))
                {

                    while (reader.Read())
                    {
                        yield return (ExpandoObject)reader;
                    }
                    cmd.Connection.Close();
                    cmd.Connection.Dispose();
                    cmd.Parameters.Clear();
                }
            }
        }

        public List<ExpandoObject> RunQueryExpandoList(string sql, List<SqlParameter> parameters)
        {
            var toReturn = new List<ExpandoObject>();
            using (var cmd = parameters != null ? GetCommand(parameters) : GetCommand(false))
            {
                cmd.CommandText = sql ?? Sql;
                cmd.CommandTimeout = Timeout;

                cmd.Connection.Open();
                using (var reader = new MereSqlDataReader<T>(cmd.ExecuteReader()))
                {

                    while (reader.Read())
                    {
                        toReturn.Add(reader);
                    }
                    cmd.Connection.Close();
                    cmd.Connection.Dispose();
                    cmd.Parameters.Clear();
                }
            }
            return toReturn;
        }

        public async Task<List<ExpandoObject>> RunQueryExpandoAsync(string sql, List<SqlParameter> parameters)
        {
            var toReturn = new List<ExpandoObject>();
            using (var cmd = parameters != null ? GetCommand(parameters) : GetCommand(false))
            {

                cmd.CommandText = sql ?? Sql;
                cmd.CommandTimeout = Timeout;

                await cmd.Connection.OpenAsync();
                using (var reader = new MereSqlDataReader<T>(await cmd.ExecuteReaderAsync()))
                {

                    while (await reader.ReadAsync())
                    {
                        toReturn.Add(reader);
                    }
                    cmd.Connection.Close();
                    cmd.Connection.Dispose();
                    cmd.Parameters.Clear();
                }
            }
            return toReturn;
        }

        public IEnumerable<dynamic> RunQueryDynamic(string sql, List<SqlParameter> parameters)
        {
            using (var cmd = parameters != null ? GetCommand(parameters) : GetCommand(false))
            {
                cmd.CommandText = sql ?? Sql;
                cmd.CommandTimeout = Timeout;

                cmd.Connection.Open();
                using (var reader = new MereSqlDataReader<T>(cmd.ExecuteReader()))
                {

                    while (reader.Read())
                    {
                        yield return (ExpandoObject)reader;
                    }
                    cmd.Connection.Close();
                    cmd.Connection.Dispose();
                    cmd.Parameters.Clear();
                }
            }
        }

        public List<dynamic> RunQueryDynamicList(string sql, List<SqlParameter> parameters)
        {
            var toReturn = new List<dynamic>();
            using (var cmd = parameters != null ? GetCommand(parameters) : GetCommand(false))
            {
                cmd.CommandText = sql ?? Sql;
                cmd.CommandTimeout = Timeout;

                cmd.Connection.Open();
                using (var reader = new MereSqlDataReader<T>(cmd.ExecuteReader()))
                {

                    while (reader.Read())
                    {
                        toReturn.Add((ExpandoObject)reader);
                    }
                    cmd.Connection.Close();
                    cmd.Connection.Dispose();
                    cmd.Parameters.Clear();
                }
            }
            return toReturn;
        }

        public async Task<List<dynamic>> RunQueryDynamicAsync(string sql, List<SqlParameter> parameters)
        {
            var toReturn = new List<dynamic>();
            using (var cmd = parameters != null ? GetCommand(parameters) : GetCommand(false))
            {

                cmd.CommandText = sql ?? Sql;
                cmd.CommandTimeout = Timeout;

                await cmd.Connection.OpenAsync();
                using (var reader = new MereSqlDataReader<T>(await cmd.ExecuteReaderAsync()))
                {

                    while (await reader.ReadAsync())
                    {
                        toReturn.Add((ExpandoObject)reader);
                    }
                    cmd.Connection.Close();
                    cmd.Connection.Dispose();
                    cmd.Parameters.Clear();
                }
            }
            return toReturn;
        }

        public IEnumerable<T> RunQuery(string sql, List<SqlParameter> parameters)
        {
            using (var cmd = parameters != null ? GetCommand(parameters) : GetCommand(false))
            {
                cmd.CommandText = sql ?? Sql;
                cmd.CommandTimeout = Timeout;

                cmd.Connection.Open();
                using (var reader = new MereSqlDataReader<T>(cmd.ExecuteReader(), SelectMereColumnsList, true))
                {

                    while (reader.Read())
                    {
                        yield return reader;
                    }
                    cmd.Connection.Close();
                    cmd.Connection.Dispose();
                    cmd.Parameters.Clear();
                }
            }
        }
        public List<T> RunQueryList(string sql, List<SqlParameter> parameters)
        {
            var toReturn = new List<T>();
            using (var cmd = parameters != null ? GetCommand(parameters) : GetCommand(false))
            {

                cmd.CommandText = sql ?? Sql;
                cmd.CommandTimeout = Timeout;

                cmd.Connection.Open();
                using (var reader = new MereSqlDataReader<T>(cmd.ExecuteReader(), SelectMereColumnsList, true))
                {

                    while (reader.Read())
                    {
                        toReturn.Add(reader);
                    }
                    cmd.Connection.Close();
                    cmd.Connection.Dispose();
                    cmd.Parameters.Clear();
                }
            }
            return toReturn;
        }

        public async Task<List<T>> RunQueryAsync(string sql, List<SqlParameter> parameters)
        {
            var toReturn = new List<T>();
            using (var cmd = parameters != null ? GetCommand(parameters) : GetCommand(false))
            {

                cmd.CommandText = sql ?? Sql;
                cmd.CommandTimeout = Timeout;

                await cmd.Connection.OpenAsync();
                using (var reader = new MereSqlDataReader<T>(await cmd.ExecuteReaderAsync(), SelectMereColumnsList, true))
                {

                    while (await reader.ReadAsync())
                    {
                        toReturn.Add(reader);
                    }
                    cmd.Connection.Close();
                    cmd.Connection.Dispose();
                    cmd.Parameters.Clear();
                }
            }
            return toReturn;
        }

        public IEnumerable<T> RunQuery()
        {
            return RunQuery(Sql, Parameters);
        }
        public List<T> RunQueryList()
        {
            return RunQueryList(Sql, Parameters);
        }

        public Task<List<T>> RunQueryAsync()
        {
            return RunQueryAsync(Sql, Parameters);
        }
        #endregion

        #region sync query executes

        public IEnumerable<T> ExecuteQuery()
        {
            return RunQuery();
        }

        public List<T> ExecuteQueryToList()
        {
            return RunQueryList();
        }

        public List<IDataRecord> ExecuteQueryToIDataRecordList()
        {
            var toReturn = new List<IDataRecord>();

            using (var cmd = GetCommand())
            {
                cmd.Connection.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        toReturn.Add(reader);
                    }
                    cmd.Connection.Close();
                }
            }
            return toReturn;
        }

        public T ExecuteQueryFirstOrDefault()
        {
            var startingTop = Top;
            Top = 1;
            var toReturn = (RunQuery()).FirstOrDefault();
            Top = startingTop;
            return toReturn;
        }

        #region ExecuteQueryExpando
        public List<dynamic> ExecuteQueryDynamicToList()
        {
            return ExecuteQueryDynamicToList(null);
        }

        public List<dynamic> ExecuteQueryDynamicToList<TFields>(Func<T, TFields> newFields)
        {
            return ExecuteQueryDynamicToList(newFields(new T()));
            //            return ExecuteQueryDynamicToList(newFields(new T()));
        }

        public List<dynamic> ExecuteQueryDynamicToList(object selectFields)
        {
            if (selectFields != null)
            {
                SetFields(x => selectFields);
            }

            return RunQueryDynamicList(null, Parameters);
        }
        public IEnumerable<dynamic> ExecuteQueryDynamic()
        {
            return ExecuteQueryDynamic(null);
        }

        public IEnumerable<dynamic> ExecuteQueryDynamic<TFields>(Func<T, TFields> newFields)
        {
            return ExecuteQueryDynamic(newFields(new T()));
        }

        public IEnumerable<dynamic> ExecuteQueryDynamic(object selectFields)
        {
            if (selectFields != null)
            {
                SetFields(x => selectFields);
            }

            return RunQueryDynamic(null, Parameters);
        }

        public List<ExpandoObject> ExecuteQueryExpandoToList()
        {
            return ExecuteQueryExpandoToList(null);
        }

        public List<ExpandoObject> ExecuteQueryExpandoToList<TFields>(Func<T, TFields> newFields)
        {
            return ExecuteQueryExpandoToList(newFields(new T()));
        }

        public List<ExpandoObject> ExecuteQueryExpandoToList(object selectFields)
        {

            if (selectFields != null)
            {
                SetFields(x => selectFields);
            }

            return RunQueryExpandoList(null, Parameters);
        }

        public IEnumerable<ExpandoObject> ExecuteQueryExpando()
        {
            return ExecuteQueryExpando(null);
        }

        public IEnumerable<ExpandoObject> ExecuteQueryExpando<TFields>(Func<T, TFields> newFields)
        {
            return ExecuteQueryExpando(newFields(new T()));
        }

        public IEnumerable<ExpandoObject> ExecuteQueryExpando(object selectFields)
        {
            if (selectFields != null)
            {
                SetFields(x => selectFields);
            }

            return RunQueryExpando(null, Parameters);
        }
        #endregion

        #region ExecuteQueryCustomQuery
        public List<dynamic> ExecuteQueryCustomQueryDynamicToList(string customQuery)
        {
            return ExecuteQueryCustomQueryDynamicToList(customQuery, null);
        }

        public List<dynamic> ExecuteQueryCustomQueryDynamicToList(string customQuery, object parameters)
        {
            List<SqlParameter> p = null;
            if (parameters != null)
            {
                p = new List<SqlParameter>();
                var self = this;
                customQuery = MereUtils.ParseParmObject(ref self, ref p, parameters, customQuery, false, true);
            }

            return RunQueryDynamicList(customQuery, p);
        }

        public IEnumerable<dynamic> ExecuteQueryCustomQueryDynamic(string customQuery)
        {
            return ExecuteQueryCustomQueryDynamic(customQuery, null);
        }

        public IEnumerable<dynamic> ExecuteQueryCustomQueryDynamic(string customQuery, object parameters)
        {
            List<SqlParameter> p = null;
            if (parameters != null)
            {
                p = new List<SqlParameter>();
                var self = this;
                customQuery = MereUtils.ParseParmObject(ref self, ref p, parameters, customQuery, false, true);
            }

            return RunQueryDynamic(customQuery, p);
        }

        public List<T> ExecuteQueryCustomQueryToList(string customQuery)
        {
            return RunQueryList(customQuery, null);
        }

        public List<T> ExecuteQueryCustomQueryToList(string customQuery, object parameters)
        {
            List<SqlParameter> p = null;
            if (parameters != null)
            {
                p = new List<SqlParameter>();
                var self = this;
                customQuery = MereUtils.ParseParmObject(ref self, ref p, parameters, customQuery, false, true);
            }

            return RunQueryList(customQuery, p);
        }

        public IEnumerable<T> ExecuteQueryCustomQuery(string customQuery)
        {
            return RunQueryList(customQuery, null);
        }

        public IEnumerable<T> ExecuteQueryCustomQuery(string customQuery, object parameters)
        {
            List<SqlParameter> p = null;
            if (parameters != null)
            {
                p = new List<SqlParameter>();
                var self = this;
                customQuery = MereUtils.ParseParmObject(ref self, ref p, parameters, customQuery, false, true);
            }
            return RunQuery(customQuery, p);
        }
        #endregion

        #region ExecuteQueryDistinct
        public IEnumerable<T> ExecuteQueryDistinct()
        {
            var distinctBefore = Distinct;
            var selectFieldsListBefore = SelectMereColumnsList;
            var selectFieldsHaveMutatedBefore = SelectFieldsHaveMutated;

            Distinct = true;

            var toReturn = RunQuery();

            Distinct = distinctBefore;
            SelectMereColumnsList = selectFieldsListBefore;
            SelectFieldsHaveMutated = selectFieldsHaveMutatedBefore;

            return toReturn;
        }

        public IEnumerable<object> ExecuteQueryDistinct<TFields>(Func<T, TFields> fields)
        {
            var distinctBefore = Distinct;
            var selectFieldsListBefore = SelectMereColumnsList;
            var selectFieldsHaveMutatedBefore = SelectFieldsHaveMutated;

            var newFields = fields(new T()) as object;

            if (newFields != null)
            {
                var props = newFields.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();
                //                var props = fields(new T()).GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();

                SelectMereColumnsList.Clear();

                for (var i = 0; i < props.Count; i++)
                {
                    var mereColumn = CurMereTableMin.GetMereColumnByPropertyName(props[i].Name);
                    if (mereColumn == null)
                        continue;

                    SelectMereColumnsList.Add(mereColumn);
                }
            }

            Distinct = true;

            var toReturn = RunQueryExpando(null, Parameters);
            Distinct = distinctBefore;
            SelectMereColumnsList = selectFieldsListBefore;
            SelectFieldsHaveMutated = selectFieldsHaveMutatedBefore;

            return toReturn;
        }

        public int ExecuteDistinctCount()
        {
            return ExecuteDistinctCount<ExpandoObject>(x => null);
        }

        public int ExecuteDistinctCount<TFields>(Func<T, TFields> fields)
        {
            var distinctBefore = Distinct;
            var selectFieldsListBefore = SelectMereColumnsList;
            var selectFieldsHaveMutatedBefore = SelectFieldsHaveMutated;

            var newFields = fields(new T()) as object;

            if (newFields != null)
            {
                var props = fields(new T()).GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();

                SelectMereColumnsList.Clear();

                for (var i = 0; i < props.Count; i++)
                {
                    var mereColumn = CurMereTableMin.GetMereColumnByPropertyName(props[i].Name);
                    if (mereColumn == null)
                        continue;

                    SelectMereColumnsList.Add(mereColumn);
                }
            }

            Distinct = true;

            var toReturn = RunQueryExpando(null, Parameters).Count();
            Distinct = distinctBefore;
            SelectMereColumnsList = selectFieldsListBefore;
            SelectFieldsHaveMutated = selectFieldsHaveMutatedBefore;

            return toReturn;
        }

        public int ExecuteCount()
        {
            var found = RunQueryDynamic(SqlForCount, Parameters).FirstOrDefault();

            return found != null && found.Count != null ? found.Count : 0;
        }
        #endregion

        #endregion

        #region async query executes
        public Task<List<T>> ExecuteQueryAsync()
        {
            return RunQueryAsync();
        }

        public async Task<T> ExecuteQueryFirstOrDefaultAsync()
        {
            var startingTop = Top;
            Top = 1;
            var toReturn = (await RunQueryAsync()).FirstOrDefault();
            Top = startingTop;
            return toReturn;
        }

        #region ExecuteQueryExpando
        public Task<List<dynamic>> ExecuteQueryDynamicAsync()
        {
            return RunQueryDynamicAsync(null, Parameters);
        }

        public Task<List<dynamic>> ExecuteQueryDynamicAsync<TFields>(Func<T, TFields> newFields)
        {
            return ExecuteQueryDynamicAsync(newFields(new T()));
        }

        public Task<List<dynamic>> ExecuteQueryDynamicAsync(object selectFields)
        {
            if (selectFields != null)
            {
                SetFields(x => selectFields);
            }

            return RunQueryDynamicAsync(null, Parameters);
        }

        public Task<List<ExpandoObject>> ExecuteQueryExpandoAsync()
        {
            return RunQueryExpandoAsync(null, Parameters);
        }

        public Task<List<ExpandoObject>> ExecuteQueryExpandoAsync<TFields>(Func<T, TFields> newFields)
        {
            return ExecuteQueryExpandoAsync(newFields(new T()));
        }

        public Task<List<ExpandoObject>> ExecuteQueryExpandoAsync(object selectFields)
        {
            PreExecuteChecks();

            if (selectFields != null)
            {
                SetFields(x => selectFields);
            }

            return RunQueryExpandoAsync(null, Parameters);
        }
        #endregion

        #region ExecuteQueryCustomQuery
        public Task<List<ExpandoObject>> ExecuteQueryCustomQueryExpandoAsync(string customQuery)
        {
            return RunQueryExpandoAsync(customQuery, null);
        }

        public Task<List<ExpandoObject>> ExecuteQueryCustomQueryExpandoAsync(string customQuery, object parameters)
        {
            List<SqlParameter> p = null;
            if (parameters != null)
            {
                p = new List<SqlParameter>();
                var self = this;
                customQuery = MereUtils.ParseParmObject(ref self, ref p, parameters, customQuery, false, true);
            }
            return RunQueryExpandoAsync(customQuery, p);
        }

        public Task<List<dynamic>> ExecuteQueryCustomQueryDynamicAsync(string customQuery)
        {
            return RunQueryDynamicAsync(customQuery, null);
        }

        public Task<List<dynamic>> ExecuteQueryCustomQueryDynamicAsync(string customQuery, object parameters)
        {
            List<SqlParameter> p = null;
            if (parameters != null)
            {
                p = new List<SqlParameter>();
                var self = this;
                customQuery = MereUtils.ParseParmObject(ref self, ref p, parameters, customQuery, false, true);
            }

            return RunQueryDynamicAsync(customQuery, p);
        }

        public Task<List<T>> ExecuteQueryCustomQueryAsync(string customQuery)
        {
            return RunQueryAsync(customQuery, null);
        }

        public Task<List<T>> ExecuteQueryCustomQueryAsync(string customQuery, object parameters)
        {
            List<SqlParameter> p = null;
            if (parameters != null)
            {
                p = new List<SqlParameter>();
                var self = this;
                customQuery = MereUtils.ParseParmObject(ref self, ref p, parameters, customQuery, false, true);
            }

            return RunQueryAsync(customQuery, p);
        }
        #endregion

        #region ExecuteQueryDistinct
        public async Task<List<T>> ExecuteQueryDistinctAsync()
        {
            var distinctBefore = Distinct;

            Distinct = true;
            var toReturn = await RunQueryAsync();

            Distinct = distinctBefore;

            return toReturn;
        }

        public async Task<List<dynamic>> ExecuteQueryDistinctAsync<TFields>(Func<T, TFields> fields)
        {
            var distinctBefore = Distinct;
            var selectFieldsListBefore = SelectMereColumnsList;//SelectFieldsList;
            var selectFieldsHaveMutatedBefore = SelectFieldsHaveMutated;

            var newFields = fields(new T());

            if (newFields != null)
            {
                var props = fields(new T()).GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();
                SelectMereColumnsList.Clear();

                for (var i = 0; i < props.Count; i++)
                {
                    var mereColumn = CurMereTableMin.GetMereColumnByPropertyName(props[i].Name);
                    if (mereColumn == null)
                        continue;

                    SelectMereColumnsList.Add(mereColumn);
                }
            }

            Distinct = true;

            var toReturn = await RunQueryDynamicAsync(null, Parameters);
            Distinct = distinctBefore;
            SelectMereColumnsList = selectFieldsListBefore;
            SelectFieldsHaveMutated = selectFieldsHaveMutatedBefore;

            return toReturn;
        }

        public Task<int> ExecuteDistinctCountAsync()
        {
            return ExecuteDistinctCountAsync<ExpandoObject>(x => null);
        }

        public async Task<int> ExecuteDistinctCountAsync<TFields>(Func<T, TFields> fields)
        {
            var distinctBefore = Distinct;
            var selectFieldsListBefore = SelectMereColumnsList;
            var selectFieldsHaveMutatedBefore = SelectFieldsHaveMutated;

            var newFields = fields(new T()) as object;

            if (newFields != null)
            {
                var props = fields(new T()).GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();

                SelectMereColumnsList.Clear();

                for (var i = 0; i < props.Count; i++)
                {
                    var mereColumn = CurMereTableMin.GetMereColumnByPropertyName(props[i].Name);
                    if (mereColumn == null)
                        continue;

                    SelectMereColumnsList.Add(mereColumn);
                }
            }

            Distinct = true;
            var found = (await RunQueryDynamicAsync(SqlForCount, Parameters)).FirstOrDefault();
            Distinct = distinctBefore;
            SelectMereColumnsList = selectFieldsListBefore;//.SelectFieldsList = selectFieldsListBefore;
            SelectFieldsHaveMutated = selectFieldsHaveMutatedBefore;
            if (found != null && found.Count != null)
                return found.Count;


            return 0;
        }

        public async Task<int> ExecuteCountAsync()
        {
            var found = (await RunQueryDynamicAsync(SqlForCount, Parameters)).FirstOrDefault();

            if (found != null && found.Count != null)
                return found.Count;

            return 0;
        }
        #endregion

        #region ExecuteQuerySimpleQuery
        //public Task<IEnumerable<T>> ExecuteQuerySimpleQueryAsync<T, TFilterObject>(TFilterObject filterObject)
        //{
        //    var q = MereQuery<T>.Create(new List<object> { filterObject });
        //    return q.ExecuteAsync();
        //}

        //public static Task<IEnumerable<T>> ExecuteQuerySimpleQueryAsync<T>(this MereQuery<T> mereQuery, int top)
        //{
        //    var q = MereQuery<T>.Create(top);
        //    return q.ExecuteAsync();
        //}

        //public static Task<IEnumerable<T>> ExecuteQuerySimpleQueryAsync<T>(this MereQuery<T> mereQuery, int top, object filterObject)
        //{
        //    var q = MereQuery<T>.Create(new List<object> { filterObject }, top);
        //    return q.ExecuteAsync();
        //}

        //public static Task<IEnumerable<T>> ExecuteQuerySimpleQueryAsync<T>(this MereQuery<T> mereQuery, string whereSql, object whereObject)
        //{
        //    var q = MereQuery<T>.Create();
        //    q.Where(whereSql, whereObject);
        //    return q.ExecuteAsync();
        //}

        //public static Task<IEnumerable<T>> ExecuteQuerySimpleQueryAsync<T>(this MereQuery<T> mereQuery, int top, string whereSql, object whereObject)
        //{
        //    var q = MereQuery<T>.Create(top);
        //    q.Where(whereSql, whereObject);
        //    return q.ExecuteAsync();
        //}
        #endregion

        #region BulkHelpers
        public async Task<int> BulkCopyToAsync<TDest>(int batchSize = 1000) where TDest : new()
        {
            var mereTableDest = MereUtils.CacheCheck<TDest>();
            var cnt = 0;

            using (var cmd = GetCommand())
            {
                using (var destConn = mereTableDest.GetConnection())
                {
                    await cmd.Connection.OpenAsync();
                    await destConn.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {

                        using (var s = new SqlBulkCopy(destConn))
                        {
                            foreach (var col in SelectMereColumnsList)
                            {
                                s.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                            }

                            s.EnableStreaming = true;
                            s.BatchSize = batchSize;
                            s.BulkCopyTimeout = Timeout;
                            s.DestinationTableName = mereTableDest.TableName;

                            await s.WriteToServerAsync(reader);

                            s.Close();

                        }
                    }

                    Connection.Close();
                    destConn.Close();
                    return cnt;
                }
            }
        }

        public async Task<List<T>> BulkCopyToChunkedParallel<TDest>(int minChunkSize, int maxChunkSize, int threadThrottle = 150, int timeout = 0) where TDest : new()
        {
            var mereTableDest = MereUtils.CacheCheck<TDest>();

            return await ExecuteQueryChunkedParallel(
                (pCnt, chunk) =>
                {
                    var internalChunk = chunk.ToList();
                    var destConn = mereTableDest.GetConnection();
                    destConn.Open();
                    using (var s = new SqlBulkCopy(destConn))
                    {
                        foreach (var col in mereTableDest.SelectMereColumns)
                        {
                            s.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                        }
                        s.EnableStreaming = true;
                        s.BatchSize = internalChunk.Count;
                        s.BulkCopyTimeout = timeout;
                        s.DestinationTableName = mereTableDest.TableName;

                        s.WriteToServer(new MereDataReader<T>(internalChunk));
                        s.Close();

                    }
                    destConn.Close();
                }, minChunkSize, maxChunkSize, threadThrottle);
        }
        #endregion

        #region ExecuteQueryChunkedParallel
        public Task<List<T>> ExecuteQueryChunkedParallel(Action<int, List<T>> perChunk, int minChunkSize,
                                                           int maxChunkSize = 0, int threadThrottle = 150)
        {
            return ExecuteQueryChunkedParallelInternal(perChunk, null, null, null, minChunkSize, maxChunkSize, threadThrottle);
        }

        public Task<List<T>> ExecuteQueryChunkedParallel(Func<int, List<T>, List<T>> perChunk, int minChunkSize,
                                                           int maxChunkSize = 0, int threadThrottle = 150)
        {
            return ExecuteQueryChunkedParallelInternal(null, null, perChunk, null, minChunkSize, maxChunkSize, threadThrottle);
        }

        public Task<List<T>> ExecuteQueryChunkedParallel(Action<List<T>> perChunk, int minChunkSize,
                                                           int maxChunkSize = 0, int threadThrottle = 150)
        {
            return ExecuteQueryChunkedParallelInternal(null, perChunk, null, null, minChunkSize, maxChunkSize, threadThrottle);
        }

        public Task<List<T>> ExecuteQueryChunkedParallel(Func<List<T>, List<T>> perChunk, int minChunkSize,
                                                           int maxChunkSize = 0, int threadThrottle = 150)
        {
            return ExecuteQueryChunkedParallelInternal(null, null, null, perChunk, minChunkSize, maxChunkSize, threadThrottle);
        }

        private async Task<List<T>> ExecuteQueryChunkedParallelInternal(
            Action<int, List<T>> perChunkAct,
            Action<List<T>> perChunkAct2,
            Func<int, List<T>, List<T>> perChunkFunc,
            Func<List<T>, List<T>> perChunkFunc2,
            int minChunkSize, int maxChunkSize = 0, int threadThrottle = 150)
        {
            var taskWatcher = TaskWatcher.StartNew(threadThrottle == 0 ? 1 : threadThrottle);

            var toReturn = new List<T>();
            var found = new List<T>();
            var processedCnt = 0;

            using (var cmd = GetCommand())
            {
                cmd.Connection.Open();
                using (var reader = new MereSqlDataReader<T>(await cmd.ExecuteReaderAsync(),
                                                      SelectMereColumnsList))
                {
                    while (await reader.ReadAsync())
                    {

                        processedCnt++;

                        found.Add(reader);

                        if (((minChunkSize <= 1 || processedCnt != 1) && (processedCnt % minChunkSize == 0 && !taskWatcher.HasPendingTasks) || (found.Count >= maxChunkSize)))
                        {
                            var chunk = new List<T>();
                            chunk.AddRange(found);
                            var cnt = processedCnt;
                            taskWatcher.AddTask(new Task(() => toReturn.AddRange(InternalParallelHelper(perChunkAct, perChunkAct2,
                                                                                                        perChunkFunc, perChunkFunc2,
                                                                                                        cnt, chunk))));

                            found = new List<T>();
                        }

                    }

                    if (found.Count > 0)
                    {
                        taskWatcher.AddTask(new Task(() => toReturn.AddRange(InternalParallelHelper(perChunkAct, perChunkAct2, perChunkFunc, perChunkFunc2,
                                               processedCnt, found))));
                    }

                    //Connection.Close();
                    cmd.Connection.Close();

                    while (taskWatcher.HasTasks)
                    {
                        await Task.Delay(10);
                    }

                    return toReturn;
                }
            }
        }

        private static List<T> InternalParallelHelper(Action<int, List<T>> perChunkAct,
            Action<List<T>> perChunkAct2,
            Func<int, List<T>, List<T>> perChunkFunc,
            Func<List<T>, List<T>> perChunkFunc2,
            int processedCnt, List<T> chunk)
        {
            var toReturn = new List<T>();
            if (perChunkAct != null)
            {
                perChunkAct(processedCnt, chunk);
                toReturn.AddRange(chunk);
            }
            else if (perChunkFunc != null)
            {
                var processed = perChunkFunc(processedCnt, chunk);
                toReturn.AddRange(processed);
            } if (perChunkAct2 != null)
            {
                perChunkAct2(chunk);
                toReturn.AddRange(chunk);
            }
            else if (perChunkFunc2 != null)
            {
                var processed = perChunkFunc2(chunk);
                toReturn.AddRange(processed);
            }
            return toReturn;
        }
        #endregion
        #endregion

        #region sync update executes
        public int ExecuteUpdate()
        {
            //PreExecuteChecks();
            var toReturn = 0;
            using (var cmd = GetCommand())
            {
                foreach (var param in UpdateFieldsDictionary)
                {
                    cmd.Parameters.AddWithValue("@" + param.Key, param.Value ?? DBNull.Value);
                }

                cmd.CommandText = Sql;
                cmd.CommandTimeout = Timeout;

                cmd.Connection.Open();
                toReturn = cmd.ExecuteNonQuery();

                cmd.Connection.Close();
                cmd.Connection.Dispose();
                cmd.Parameters.Clear();
            }

            return toReturn;
        }

        public int ExecuteUpdate(T newValues)
        {
            //PreExecuteChecks();

            //Command.CommandText = Sql;

            var toReturn = 0;
            using (var cmd = GetCommand())
            {

                foreach (var mereColumn in CurMereTableMin.SelectMereColumnsNoIdentity())
                {
                    cmd.Parameters.AddWithValue("@" + mereColumn.ColumnName, mereColumn.GetBase(newValues) ?? DBNull.Value);
                }

                cmd.CommandText = Sql;
                cmd.CommandTimeout = Timeout;

                cmd.Connection.Open();
                toReturn = cmd.ExecuteNonQuery();

                cmd.Connection.Close();
                cmd.Connection.Dispose();
                cmd.Parameters.Clear();
            }

            return toReturn;
        }

        public int ExecuteUpdate(ExpandoObject newValues)
        {
            return ExecuteUpdate((IDictionary<string, object>)newValues);
        }

        /// <summary>
        /// Executes update using supplied fields adding filters for key and/or identity fields
        /// </summary>
        /// <param name="newValues"></param>
        /// <returns></returns>
        public int ExecuteUpdate(IEnumerable<KeyValuePair<string, object>> newValues)
        {
            return ExecuteUpdate(newValues, true);
        }

        /// <summary>
        /// Executes update using supplied fields with option for adding filters for key and/or identity fields
        /// </summary>
        /// <param name="newValues"></param>
        /// <param name="useKeyAndOrIdentity"></param>
        /// <returns></returns>
        public int ExecuteUpdate(IEnumerable<KeyValuePair<string, object>> newValues, bool useKeyAndOrIdentity)
        {
            UpdateFieldsDictionary.Clear();
            AddUpdateField(newValues, useKeyAndOrIdentity);

            return ExecuteUpdate();
        }
        #endregion

        #region async update executes
        public async Task<int> ExecuteUpdateAsync()
        {
            //PreExecuteChecks();

            var toReturn = 0;
            using (var cmd = GetCommand())
            {
                cmd.CommandText = Sql;
                cmd.CommandTimeout = Timeout;

                foreach (var param in UpdateFieldsDictionary)
                {
                    cmd.Parameters.AddWithValue("@" + param.Key, param.Value ?? DBNull.Value);
                }

                await cmd.Connection.OpenAsync();
                toReturn = await cmd.ExecuteNonQueryAsync();

                cmd.Connection.Close();
                cmd.Connection.Dispose();
                cmd.Parameters.Clear();
            }

            return toReturn;
        }

        public async Task<int> ExecuteUpdateAsync(T newValues)
        {
            //PreExecuteChecks();
            var toReturn = 0;
            using (var cmd = GetCommand())
            {
                foreach (var mereColumn in CurMereTableMin.SelectMereColumnsNoIdentity())
                {
                    AddUpdateField(mereColumn.ColumnName, false);
                    cmd.Parameters.AddWithValue("@" + mereColumn.ColumnName, mereColumn.GetBase(newValues) ?? DBNull.Value);
                }

                cmd.CommandText = Sql;
                cmd.CommandTimeout = Timeout;

                await cmd.Connection.OpenAsync();
                toReturn = await cmd.ExecuteNonQueryAsync();

                cmd.Connection.Close();
                cmd.Connection.Dispose();
                cmd.Parameters.Clear();
            }

            return toReturn;
        }

        public Task<int> ExecuteUpdateAsync(ExpandoObject newValues)
        {
            return ExecuteUpdateAsync((IDictionary<string, object>)newValues);
        }

        /// <summary>
        /// ExecuteUpdates update using supplied fields adding filters for key and/or identity fields
        /// </summary>
        /// <param name="newValues"></param>
        /// <returns></returns>
        public Task<int> ExecuteUpdateAsync(IEnumerable<KeyValuePair<string, object>> newValues)
        {
            return ExecuteUpdateAsync(newValues, true);
        }


        /// <summary>
        /// ExecuteUpdates update using supplied fields with option for adding filters for key and/or identity fields
        /// </summary>
        /// <param name="newValues"></param>
        /// <param name="useKeyAndOrIdentity"></param>
        /// <returns></returns>
        public Task<int> ExecuteUpdateAsync(IEnumerable<KeyValuePair<string, object>> newValues, bool useKeyAndOrIdentity)
        {
            UpdateFieldsDictionary.Clear();

            AddUpdateField(newValues, useKeyAndOrIdentity);

            return ExecuteUpdateAsync();
        }
        #endregion

        #region sync delete executes
        public int ExecuteDelete()
        {
            var toReturn = 0;
            using (var cmd = GetCommand())
            {
                cmd.CommandText = Sql;
                cmd.CommandTimeout = Timeout;

                cmd.Connection.Open();
                toReturn = cmd.ExecuteNonQuery();

                cmd.Connection.Close();
                cmd.Connection.Dispose();
                cmd.Parameters.Clear();
            }

            return toReturn;
        }
        #endregion

        #region async delete executes
        public async Task<int> ExecuteDeleteAsync()
        {
            var toReturn = 0;
            using (var cmd = GetCommand())
            {
                cmd.CommandText = Sql;
                cmd.CommandTimeout = Timeout;

                await cmd.Connection.OpenAsync();
                toReturn = await cmd.ExecuteNonQueryAsync();

                cmd.Connection.Close();
                cmd.Connection.Dispose();
                cmd.Parameters.Clear();
            }

            return toReturn;
        }
        #endregion

        public void Dispose()
        {
            if (Connection != null)
                Connection.Dispose();

            if (Command != null)
                Command.Dispose();

            MereUtils.OffMereEnvironmentChanged(MereEnvironmentSubscriptionId);
        }
    }
}
