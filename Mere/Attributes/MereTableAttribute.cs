using System;

namespace Mere.Attributes
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class MereTableAttribute : Attribute
    {
        public string TableName { get; set; }
        public string DatabaseName { get; set; }
        public string ServerName { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        public int Timeout { get; set; }
        public MereTableAttribute(string tableName, string databaseName, string serverName, string userId, string password)
        {
            TableName = tableName;
            DatabaseName = databaseName;
            ServerName = serverName;
            UserId = userId;
            Password = password;
            Timeout = 0;
        }
        public MereTableAttribute(string tableName)
        {
            TableName = tableName;
            Timeout = 0;
        }
        public MereTableAttribute(string tableName, int timeout)
        {
            TableName = tableName;
            Timeout = timeout;
        }

        public MereTableAttribute(string tableName, string databaseName, string serverName, string userId, string password, int timeout)
        {
            TableName = tableName;
            DatabaseName = databaseName;
            ServerName = serverName;
            UserId = userId;
            Password = password;
            Timeout = timeout;
        }
    }
}
