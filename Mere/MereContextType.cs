using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mere
{
    public enum MereContextType
    {
        Query,
        Insert,
        Update,
        UpdateWithUpdateFields,
        Save,
        Delete,
        NonQuery,
        Custom
    }
}
