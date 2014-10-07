using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mere.Files.Attributes
{
    public class MereDelimitedFileTableTypeParsingAttribute : MereFileTableTypeParsingAttribute
    {
        public MereDelimitedFileTableTypeParsingAttribute(Type typeToFormat, params MereFileParsingOption[] parsingOptions)
            : base(typeToFormat, parsingOptions) { }
        public MereDelimitedFileTableTypeParsingAttribute(params MereFileParsingOption[] parsingOptions)
            : base(parsingOptions) { }
    }
}
