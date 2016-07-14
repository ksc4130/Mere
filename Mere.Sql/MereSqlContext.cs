using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Mere.Core;

namespace Mere.Sql
{
    public class MereSqlContext<T> : MereContext<T> where T : new()
    {
        public MereSqlContext()
        {
            Connection = CurMereTable.GetConnection();
            Parameters = new List<SqlParameter>();
        }

        public List<SqlParameter> Parameters { get; set; }
        public SqlCommand Command { get; set; }
        public SqlConnection Connection { get; set; }

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

        public string SqlInsert => CurMereTable.SqlInsert();
        

        public string SqlDelete => $"DELETE FROM {CurMereTable.TableName}  {WhereStr}";

        public string UpdateFields
        {
            get
            {
                return (UpdateFieldsDictionary != null && UpdateFieldsDictionary.Any()) ? "UPDATE " + TableName + " SET " + string.Join(", ", UpdateFieldsDictionary.Select(x => x.Key + "=@" + x.Key)) : SqlUpdateNotUsingUpdateFields();
            }
        }

        public string OrderByStr => OrderByList != null && OrderByList.Any() ? " ORDER BY " + string.Join(", ", OrderByList) : "";

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

        public string SqlUpdateUsingUpdateFields  => UpdateFields + WhereStr;

        public string SqlUpdateNotUsingUpdateFields => CurMereTable.SqlUpdateWithoutKey();

        public string SqlUpdate => CurMereTable.SqlUpdateWithoutKey() + WhereStr;

        public string SqlForCount => $"SELECT COUNT(0) AS Count FROM {TableName} {WhereStr}";

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
                        return CurMereTable.GetUpsertSqlWithKey();
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

        public void PreExecuteChecks()
        {
            UpdateConnection();
            UpdateCommand();
        }

        private  void UpdateConnection()
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
    }
}