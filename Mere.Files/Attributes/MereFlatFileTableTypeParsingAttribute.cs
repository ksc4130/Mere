using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mere.Files.Attributes
{
    public class MereFlatFileTableTypeParsingAttribute : MereFileTableTypeParsingAttribute
    {
        public MereFlatFileTableTypeParsingAttribute(Type typeToFormat, params MereFileParsingOption[] parsingOptions)
            : base(typeToFormat, parsingOptions) { }
        public MereFlatFileTableTypeParsingAttribute(params MereFileParsingOption[] parsingOptions)
            : base(parsingOptions) { }
    }
}
