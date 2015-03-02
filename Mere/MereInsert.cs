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
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Mere.Interfaces;

namespace Mere
{
    public class MereInsert
    {
        protected MereInsert() { }

        public static MereInsert<T> Create<T>() where T : new()
        {
            return MereInsert<T>.Create();
        }

        public static MereInsert<T> Create<T>(MereDataSource mds) where T : new()
        {
            return MereInsert<T>.Create(mds);
        }
    }
    public class MereInsert<T> where T : new()
    {
        public MereInsert()
        {
            _context = new MereContext<T>(MereContextType.Insert);
        }

        public MereInsert(bool truncateLength)
        {
            TruncateLength = truncateLength;
            _context = new MereContext<T>(MereContextType.Insert);
        }

        public MereInsert(MereDataSource ds, bool truncateLength)
        {
            TruncateLength = truncateLength;
            _context = new MereContext<T>(MereContextType.Insert, ds);
        }

        public static MereInsert<T> Create()
        {
            return new MereInsert<T>();
        }

        public static MereInsert<T> Create(bool truncateLength)
        {
            return new MereInsert<T>(truncateLength);
        }

        public static MereInsert<T> Create(MereDataSource ds)
        {
            return new MereInsert<T>(ds, false);
        }

        public static MereInsert<T> Create(MereDataSource ds, bool truncateLength)
        {
            return new MereInsert<T>(ds, truncateLength);
        }

        private MereContext<T> _context;
        public MereContext<T> Context { get { return _context; } }

        #region properties
        public SqlConnection Connection { get; set; }
        public SqlCommand Command { get; set; }
        public string ConnectionString { get; set; }
        public string Sql { get; set; }
        public int Timeout { get; set; }
        public bool TruncateLength { get; set; }
        #endregion

        #region methods
        private void PreExecuteChecks()
        {
            Command = _context.GetCommand();
        }

        public MereInsert<T> SetDatabase(string databaseName)
        {
            Context.DatabaseName = databaseName;
            return this;
        }

        public MereInsert<T> SetServer(string serverName)
        {
            Context.ServerName = serverName;
            return this;
        }

        public MereInsert<T> SetTable(string tableName)
        {
            Context.TableName = tableName;
            return this;
        }

        public MereInsert<T> SetUserId(string userId)
        {
            Context.UserId = userId;
            return this;
        }

        public MereInsert<T> SetPassword(string password)
        {
            Context.Password = password;
            return this;
        }

        public void FillParams(T toInsert)
        {
            Command.Parameters.Clear();

            foreach (var mereColumn in _context.CurMereTableMin.SelectMereColumnsNoIdentity())
            {
                Command.Parameters.AddWithValue("@" + mereColumn.ColumnName, mereColumn.Get(toInsert, TruncateLength));
            } 
        }
        #endregion

        #region sync executes
        public int Execute(T toInsert)
        {
            PreExecuteChecks();

            FillParams(toInsert);

            Command.Connection.Open();

            var toReturn = Command.ExecuteNonQuery();

            Command.Connection.Close();
            return toReturn;
        }
        #endregion

        #region async executes
        public async Task<int> ExecuteAsync(T toInsert)
        {
            PreExecuteChecks();

            FillParams(toInsert);

            await Command.Connection.OpenAsync();

            var toReturn = await Command.ExecuteNonQueryAsync();

            Command.Connection.Close();
            return toReturn;
        }
        #endregion
    }
}
