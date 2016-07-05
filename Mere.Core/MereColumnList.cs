using System;
using System.Collections.Generic;
using System.Linq;

namespace Mere.Core
{
    public class MereColumnList : List<MereColumn>
    {
        public virtual MereColumn GetMereColumnByColumnName(string columnName)
        {
            return
                this.FirstOrDefault(
                    x => string.Compare(columnName, x.ColumnName, StringComparison.CurrentCultureIgnoreCase) == 0);
        }

        public virtual MereColumn GetMereColumnByPropertyName(string propertyName)
        {
            return
                this.FirstOrDefault(
                    x => propertyName == x.PropertyName);
        }

        public virtual MereColumn GetMereColumn(string name)
        {
            return
                this.FirstOrDefault(
                    x => (string.Compare(name, x.ColumnName, StringComparison.CurrentCultureIgnoreCase) == 0) ||
                        (string.Compare(name, x.PropertyName, StringComparison.CurrentCultureIgnoreCase) == 0));
        }
    }
}