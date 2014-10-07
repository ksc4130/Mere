using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mere.Files.Attributes
{
    public class MereDelimitedFileTableTypeFormatAttribute : MereFileTableTypeFormatAttribute
    {
        public MereDelimitedFileTableTypeFormatAttribute(Type typeToFormat, string toStringFormat)
            : base(typeToFormat, toStringFormat) { }
    }
}
