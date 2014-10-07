using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mere.Files.Attributes
{
    public class MereFlatFileTableTypeFormatAttribute : MereFileTableTypeFormatAttribute
    {
        public MereFlatFileTableTypeFormatAttribute(Type typeToFormat, string toStringFormat)
            : base(typeToFormat, toStringFormat) { }
    }
}
