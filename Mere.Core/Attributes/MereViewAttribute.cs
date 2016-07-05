using System;

namespace Mere.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class MereViewAttribute : Attribute
    {
        public string ViewName { get; set; }
        public string DatabaseName { get; set; }
        public string ServerName { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        public int Timeout { get; set; }
        public MereViewAttribute(string viewName, string databaseName, string serverName, string userId, string password)
        {
            ViewName = viewName;
            DatabaseName = databaseName;
            ServerName = serverName;
            UserId = userId;
            Password = password;
            Password = password;
            Timeout = 0;
        }

        public MereViewAttribute(string viewName, string databaseName, string serverName, string userId, string password, int timeout)
        {
            ViewName = viewName;
            DatabaseName = databaseName;
            ServerName = serverName;
            UserId = userId;
            Password = password;
            Password = password;
            Timeout = timeout;
        }
    }
}
