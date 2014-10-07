//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Mere.Interfaces;

//namespace Mere.BaseClasses
//{
//    internal class MereFilterableParent<T>
//    {

//        private MereFilterableParent()
//        {
//            _queryContext = new MereContext<T>();
//            _pre = new MereQueryPre<T>(this);
//            _post = new MereQueryPost<T>(this);
//        }

//        public static IMereQueryPre<T> Create()
//        {
//            var parent = new MereFilterableParent<T>();
//            return parent._pre;
//        }
//        public static IMereQueryPre<T> Create(int top)
//        {
//            var parent = new MereFilterableParent<T>();
//            parent._queryContext.Top = top;
//            return parent._pre;
//        }

//        private MereContext<T> _queryContext;
//        private readonly IMereQueryPre<T> _pre;
//        private readonly IMereQueryPost<T> _post;

//        #region methods
//        private void PreExecuteChecks()
//        {
//            UpdateConnection();
//            UpdateCommand();
//        }

//        private void UpdateConnection()
//        {
//            _queryContext.Connection.ConnectionString = _queryContext.ConnectionString;
//        }

//        private void UpdateCommand()
//        {
//            _queryContext.Command.CommandText = _queryContext.Sql;
//            _queryContext.Command.CommandTimeout = _queryContext.Timeout;
//        }
//        #endregion
//    }
//}
