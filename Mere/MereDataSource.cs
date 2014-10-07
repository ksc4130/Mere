using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mere
{
    public class MereDataSource
    {
        public string ServerName { get; set; }
        public string DatabaseName { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        public string TableName { get; set; }
        public bool TruncateLengths { get; set; }

        public Dictionary<string, MereDataSource> Environments { get; set; }

        public MereDataSource GetEnvironmentDataSource(string environment)
        {
            if (Environments.ContainsKey(environment))
                return Environments[environment];
            else
                return this;
        }

        public MereDataSource this[string environment]
        {
            get
            {
                if (environment == null || !Environments.ContainsKey(environment))
                    return this;

                return Environments[environment];
            }
            set
            {
                if (Environments.ContainsKey(environment))
                    Environments[environment] = value;
                else
                    Environments.Add(environment, value);
            }
        }

        public void AddOrUpdate(string environment, MereDataSource mereDataSource)
        {
            if (Environments.ContainsKey(environment))
                Environments[environment] = mereDataSource;
            else
                Environments.Add(environment, mereDataSource);
        }

        public void AddOrUpdate(string environment, string serverName, string databaseName, string userId, string password)
        {
            AddOrUpdate(environment, Create(serverName, databaseName, userId, password));
        }

        public void AddOrUpdate(string environment, string serverName, string databaseName, string userId, string password, bool truncateLength)
        {
            AddOrUpdate(environment, Create(serverName, databaseName, userId, password, truncateLength));
        }

        protected MereDataSource() { }

        public static MereDataSource Create(string serverName, string databaseName, string userId, string password)
        {
            return new MereDataSource
            {
                Environments = new Dictionary<string, MereDataSource>(),
                ServerName = serverName,
                DatabaseName = databaseName,
                UserId = userId,
                Password = password,
                TableName = null
            };
        }

        public static MereDataSource Create(string serverName, string databaseName, string userId, string password, bool truncateLength)
        {
            return new MereDataSource
            {
                Environments = new Dictionary<string, MereDataSource>(),
                ServerName = serverName,
                DatabaseName = databaseName,
                UserId = userId,
                Password = password,
                TableName = null,
                TruncateLengths = truncateLength
            };
        }

        public static MereDataSource Create(string serverName, string databaseName, string userId, string password, string tableName)
        {
            return new MereDataSource
            {
                Environments = new Dictionary<string, MereDataSource>(),
                ServerName = serverName,
                DatabaseName = databaseName,
                UserId = userId,
                Password = password,
                TableName = tableName
            };
        }

        public static MereDataSource Create(string serverName, string databaseName, string userId, string password, string tableName, bool truncateLength)
        {
            return new MereDataSource
            {
                Environments = new Dictionary<string, MereDataSource>(),
                ServerName = serverName,
                DatabaseName = databaseName,
                UserId = userId,
                Password = password,
                TableName = tableName,
                TruncateLengths = truncateLength
            };
        }

        public static MereDataSource Create<T>() where T : new()
        {
            var mereTable = MereUtils.CacheCheck<T>();
            return new MereDataSource
            {
                Environments = new Dictionary<string, MereDataSource>(),
                ServerName = mereTable.ServerName,
                DatabaseName = mereTable.DatabaseName,
                UserId = mereTable.UserId,
                Password = mereTable.Password,
                TableName = mereTable.TableName
            };
        }
    }
}
