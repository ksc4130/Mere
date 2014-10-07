//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Data.SqlClient;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using System.Threading.Tasks;

//namespace Mere
//{
//    public class MereFilterContext<T>
//    {
//        public MereFilterContext()
//        {
//            CurMereTableMin = MereUtils.CacheCheckMin<T>();
//            TableName = CurMereTableMin.TableName;
//            DatabaseName = CurMereTableMin.DatabaseName;
//            ServerName = CurMereTableMin.ServerName;
//            UserId = CurMereTableMin.UserId;
//            Password = CurMereTableMin.Password;
//            Connection = CurMereTableMin.GetConnection();
//            ParamNames = new List<string>();
//            Connection = new SqlConnection();
//            Command = Connection.CreateCommand();
//            _connectionStringBase = "Data Source={0};Initial Catalog={1};User ID={2};Password={3};Asynchronous Processing=True;MultipleActiveResultSets=True;Timeout={4}";
//        }

//        #region properties
//        public SqlCommand Command { get; set; }
//        public SqlConnection Connection { get; set; }
//        public string TableName { get; set; }
//        public string DatabaseName { get; set; }
//        private readonly string _connectionStringBase;
//        public string ConnectionStringBase { get { return _connectionStringBase; } }
//        public string ConnectionString
//        {
//            get
//            {
//                return ConnectionStringBase == null ? "" : string.Format(ConnectionStringBase, ServerName, DatabaseName,
//                                     UserId, Password, Timeout);
//            }
//        }
//        public string ServerName { get; set; }
//        public int Timeout { get; set; }
//        public string UserId { get; set; }
//        public string Password { get; set; }

//        public MereTableMin<T> CurMereTableMin { get; set; }

//        public bool BuildingFilterGroup { get; set; }
//        public bool CloseFilterGroup { get; set; }
        
//        public MereFilterGroup CurMereFilterGroup { get; set; }

//        public MereColumn CurFilterMereColumn { get; set; }
//        public string CurFilterColumnName { get; set; }
//        public string CurFilterPropertyName { get; set; }
//        public bool CurFilterOr { get; set; }
//        public bool CurFilterGroupOr { get; set; }
//        public bool CurFilterInnerGroup { get; set; }
//        public int CurFilterBackup { get; set; }

//        public List<string> ParamNames { get; set; }

//        public List<MereFilterGroup> WhereChunks { get; set; }

//        public string WhereStr
//        {
//            get
//            {

//                if (CurMereFilterGroup != null && CurMereFilterGroup.HasFilters)
//                {
//                    var whereChunks = new List<MereFilterGroup>();
//                    if (WhereChunks != null && WhereChunks.Any())
//                        whereChunks.AddRange(WhereChunks);

//                    whereChunks.Add(CurMereFilterGroup);

//                    return whereChunks.Count > 0 ? " WHERE " + whereChunks.First().WhereString + string.Join(" ", whereChunks.Skip(1).Select(s => s.AndOr + s.WhereString)) : "";
//                }

//                return WhereChunks != null && WhereChunks.Count > 0 ? " WHERE " + WhereChunks.First().WhereString + string.Join(" ", WhereChunks.Skip(1).Select(s => s.AndOr + s.WhereString)) : "";
//            }
//        }
//        #endregion

//        #region base methods

//        public void Credentials(string serverName, string databaseName, string userId, string password)
//        {
//            ServerName = serverName;
//            DatabaseName = databaseName;
//            UserId = userId;
//            Password = password;
//        }

//        public void Database(string databaseName)
//        {
//            DatabaseName = databaseName;
//        }

//        public void Server(string serverName)
//        {
//            ServerName = serverName;
//        }

//        public void User(string userId)
//        {
//            UserId = userId;
//        }

//        public void PasswordSetter(string password)
//        {
//            Password = password;
//        }

        

//        public virtual void CreateCommand(IList<object> paramObj = null, string whereSqlIn = null, object whereObj = null)
//        {
//            if (Command == null)
//            {
//                Command = Connection.CreateCommand();
//                Command.CommandTimeout = 0;
//            }

//            /* where sql part*/
//            if (whereSqlIn != null)
//            {
//                ParseParmObject(whereObj, whereSqlIn);
//            }

//            if (paramObj != null && paramObj.Count > 0)
//            {
//                for (var iObj = 0; iObj < paramObj.Count; iObj++)
//                {
//                    var c = ParseParmObject(paramObj[iObj]);
//                }
//            }

//            //Command.CommandText = Sql;
//        }
//        #endregion

//        #region Parsing
//        public string ParseParmObject(object obj, string whereSql = null, bool or = false, bool addParamOnly = false, bool innerGroup = false, int backup = 0)
//        {

//            if (CloseFilterGroup)
//            {
//                if (CurMereFilterGroup != null && CurMereFilterGroup.HasFilters && !addParamOnly)
//                {

//                    WhereChunks.Add(CurMereFilterGroup);
//                    CurMereFilterGroup = new MereFilterGroup(or);
//                }

//                CloseFilterGroup = false;
//            }

//            var whereChunk = new StringBuilder();

//            if (whereSql != null && !addParamOnly)
//            {
//                if (whereSql.Trim().ToLower().StartsWith("where"))
//                {
//                    var sp = whereSql.Trim().Skip(5).Select(s => s).ToArray();
//                    whereSql = string.Join("", sp);
//                }

//            }

//            if (obj != null)
//            {
//                var props = obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();

//                for (var i = 0; i < props.Count; i++)
//                {
//                    var toAdd = ParseParamValue(props[i].Name, props[i].GetValue(obj), whereSql);

//                    if (toAdd == null)
//                        continue;

//                    if (whereChunk.Length > 0 && whereSql == null)
//                    {
//                        whereChunk.Append(" AND ");
//                    }

//                    if (whereSql != null)
//                        whereSql = toAdd;
//                    else
//                        whereChunk.Append(toAdd);
//                }

//            }
//            if (WhereChunks == null && !addParamOnly)
//                WhereChunks = new List<MereFilterGroup>();


//            if (!addParamOnly)
//            {
//                if (CurMereFilterGroup == null)
//                    CurMereFilterGroup = new MereFilterGroup(or);

//                CurMereFilterGroup.AddFilter(or ? " OR " : " AND ", whereSql ?? whereChunk.ToString(), innerGroup, backup);

//            }

//            return whereSql ?? whereChunk.ToString();
//        }

//        public string ParseParamValue(string propOrColName, object value, string whereSql = null, string sqlOperator = null)
//        {
//            string toReturn;

//            var mereColumn = CurMereTableMin.GetMereColumn(propOrColName);

//            //if (CurMereTableMin.PropertyHelpersNamed.Keys.Any(a => a.ToLower() == propOrColName.ToLower()))
//            //    mereColumn = CurMereTableMin.PropertyHelpersNamed.First(f => f.Key.ToLower() == propOrColName.ToLower()).Value;
//            //else if (CurMereTableMin.PropertyHelpersColumnName.Keys.Any(a => a.ToLower() == propOrColName.ToLower()))
//            //    mereColumn = CurMereTableMin.PropertyHelpersColumnName.First(f => f.Key.ToLower() == propOrColName.ToLower()).Value;
//            //else if (whereSql == null)
//            //    return null;

//            //could throw error here
//            if (whereSql == null && mereColumn == null)
//                return null;

//            if (value == null)
//            {

//                if (whereSql != null)
//                {
//                    toReturn = whereSql.Replace("@" + propOrColName, " NULL ");
//                }
//                else
//                {
//                    var trimmedSql = sqlOperator != null ? sqlOperator.Trim() : "=";
//                    if (trimmedSql == "=")
//                        toReturn = string.Format(" [{0}] IS NULL ", mereColumn.ColumnName);
//                    else if (trimmedSql == "!=" || trimmedSql == "<>")
//                        toReturn = string.Format(" [{0}] IS NOT NULL ", mereColumn.ColumnName);
//                    else
//                        throw new ArgumentNullException("value", "value can only be with =, !=, or <>");
//                }
//                //return toReturn;
//            }

//            var isListOrArray = false;
//            var valPropertTypeFullName = "";
//            if (value != null)
//            {
//                var baseType = value.GetType().BaseType;
//                valPropertTypeFullName = value.GetType().FullName;
//                if (baseType != null)
//                {
//                    isListOrArray = baseType.FullName.ToLower().Contains("enumerable") ||
//                                    baseType.FullName.ToLower().Contains("array") ||
//                                    baseType.FullName.ToLower().Contains("list");
//                }

//                isListOrArray = isListOrArray || value.GetType().FullName.ToLower().Contains("enumerable") ||
//                                value.GetType().FullName.ToLower().Contains("array") ||
//                                value.GetType().FullName.ToLower().Contains("list");
//            }


//            if (isListOrArray)
//            {
//                var forIn = new StringBuilder();
//                var cnt = 0;
//                foreach (var p in (IEnumerable)value)
//                {
//                    if (cnt > 0)
//                        forIn.Append(", ");

//                    if ((mereColumn != null && (mereColumn.PropertyDescriptor.PropertyType.FullName.ToLower().Contains("string")
//                        || mereColumn.PropertyDescriptor.PropertyType.FullName.ToLower().Contains("date")))
//                        || (valPropertTypeFullName.ToLower().Contains("string")
//                        || valPropertTypeFullName.ToLower().Contains("date")))
//                    {
//                        forIn.AppendFormat("'{0}'", p);
//                    }
//                    else
//                    {
//                        forIn.AppendFormat("{0}", p);
//                    }
//                    cnt++;
//                }

//                if (whereSql != null)
//                {
//                    toReturn = whereSql.Replace("@" + propOrColName, forIn.ToString());
//                }
//                else
//                {
//                    toReturn = string.Format(" {0} IN ({1}) ", mereColumn.ColumnName, forIn);
//                }
//            }
//            else
//            {
//                var pName = MereUtils.NoDupNameCheck("@" + propOrColName, ParamNames);

//                ParamNames.Add(pName);

//                var parm = Command.CreateParameter();
//                parm.ParameterName = pName;
//                parm.Value = value;
//                Command.Parameters.Add(parm);

//                if (whereSql != null)
//                {
//                    toReturn = whereSql.Replace("@" + propOrColName, pName);
//                }
//                else
//                {
//                    toReturn = string.Format(" [{0}] {1} {2} ", mereColumn.ColumnName, sqlOperator ?? " = ", pName);
//                }
//            }

//            return toReturn;
//        }
//        #endregion
//    }
//}
