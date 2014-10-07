using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mere.Files.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public abstract class MereFileTableTypeFormatAttribute : Attribute
    {
        public Type TypeToFormat { get; set; }
        public string ToStringFormat { get; set; }

        protected MereFileTableTypeFormatAttribute(Type typeToFormat, string toStringFormat)
        {
            TypeToFormat = typeToFormat;
            ToStringFormat = toStringFormat;
        }
    }
}
