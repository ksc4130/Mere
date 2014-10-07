using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mere
{
    public static class MereUpdateExtensions
    {
        #region sync
        public static int MereUpdate<T>(this ExpandoObject obj) where T : new()
        {
            var mereUpdate = Mere.MereUpdate.Create<T>();

            return mereUpdate.Execute(obj, true);
        }

        public static int MereUpdate<T>(this ExpandoObject obj, bool useKeyAndOrIdentity) where T : new()
        {
            var mereUpdate = Mere.MereUpdate.Create<T>();

            return mereUpdate.Execute(obj, true);
        }
        #endregion

        #region async
        public static Task<int> MereUpdateAsync<T>(this ExpandoObject obj) where T : new()
        {
            var mereUpdate = Mere.MereUpdate.Create<T>();

            return mereUpdate.ExecuteAsync(obj, true);
        }

        public static Task<int> MereUpdateAsync<T>(this ExpandoObject obj, bool useKeyAndOrIdentity) where T : new()
        {
            var mereUpdate = Mere.MereUpdate.Create<T>();

            return mereUpdate.ExecuteAsync(obj, true);
        }
        #endregion
    }
}
