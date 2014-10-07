using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mere.Interfaces
{
    public interface IMereQueryFilter<T, TProp> where T : new()
    {
        #region filter methods
        IMereQueryPost<T> EqualTo(TProp value);

        IMereQueryPost<T> EqualToCaseSensitive(TProp value);

        IMereQueryPost<T> NotEqualTo(TProp value);

        IMereQueryPost<T> NotEqualToCaseSensitive(TProp value);

        IMereQueryPost<T> GreaterThan(TProp value);

        IMereQueryPost<T> GreaterThanOrEqualTo(TProp value);

        IMereQueryPost<T> LessThan(TProp value);

        IMereQueryPost<T> LessThanOrEqualTo(TProp value);

        IMereQueryPost<T> In<TVal>(TVal values) where TVal : IEnumerable<TProp>;
        IMereQueryPost<T> In(params TProp[] values);

        IMereQueryPost<T> InCaseSensitive<TVal>(TVal values) where TVal : IEnumerable<TProp>;
        IMereQueryPost<T> InCaseSensitive(params TProp[] values);

        IMereQueryPost<T> NotIn<TVal>(TVal values) where TVal : IEnumerable<TProp>;
        IMereQueryPost<T> NotIn(params TProp[] values);

        IMereQueryPost<T> NotInCaseSensitive<TVal>(TVal values) where TVal : IEnumerable<TProp>;
        IMereQueryPost<T> NotInCaseSensitive(params TProp[] values);

        IMereQueryPost<T> Between(TProp value1, TProp value2);
        
        IMereQueryPost<T> NotBetween(TProp value1, TProp value2);

        IMereQueryPost<T> Contains(TProp value);
        
        IMereQueryPost<T> NotContains(TProp value);

        IMereQueryPost<T> StartsWith(TProp value);
        
        IMereQueryPost<T> NotStartsWith(TProp value);

        IMereQueryPost<T> EndsWith(TProp value);
        
        IMereQueryPost<T> NotEndsWith(TProp value);

        IMereQueryPost<T> ContainsCaseSensitive(TProp value);

        IMereQueryPost<T> NotContainsCaseSensitive(TProp value);

        IMereQueryPost<T> StartsWithCaseSensitive(TProp value);
        
        IMereQueryPost<T> NotStartsWithCaseSensitive(TProp value);

        IMereQueryPost<T> EndsWithCaseSensitive(TProp value);
        
        IMereQueryPost<T> NotEndsWithCaseSensitive(TProp value);

        #endregion
    }
}
