using System;
using System.Linq.Expressions;

namespace Mere.Core
{
    public static class ExpressionExt
    {
        /// <summary>
        /// Converts linq expression to the string version of selected property's name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TProp"></typeparam>
        /// <param name="src"></param>
        /// <returns>Returns property's name as string</returns>
        public static string GetPropName<T, TProp>(this Expression<Func<T, TProp>> src)
        {
            var pinfo = MereUtils.GetProperty(src);
            return pinfo.Name;
        }
    }
}