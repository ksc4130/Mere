using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mere.Files.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class MereFileTableTypeParsingAttribute : Attribute
    {
        public Type TypeToFormat { get; set; }
        public List<MereFileParsingOption> FileFieldParsingOptions { get; set; }

        protected MereFileTableTypeParsingAttribute(params MereFileParsingOption[] parsingOptions)
        {
            TypeToFormat = null;
            FileFieldParsingOptions = parsingOptions.ToList();
        }

        protected MereFileTableTypeParsingAttribute(Type typeToFormat, params MereFileParsingOption[] parsingOptions)
        {
            TypeToFormat = typeToFormat;
            FileFieldParsingOptions = parsingOptions.ToList();
        }
    }
}
