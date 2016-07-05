using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Mere.Core
{
    public class MereContext<T> : IDisposable where T : new()
    {
        public MereContext() :
            this(MereContextType.Query, MereUtils.GetDataSource<T>())
        { }

        public MereContext(MereContextType contextType) :
            this(contextType, MereUtils.GetDataSource<T>())
        { }

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
            this(MereContextType.Query, dataSource)
        { }

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
    }
}