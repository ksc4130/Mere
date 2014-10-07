using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mere.Files.Attributes
{
    public class MereFlatFileTableAttribute : MereFileTableAttribute
    {
        public MereFlatFileTableAttribute(string recKey, string lineDelimiter = "\r\n")
            : base(null, lineDelimiter, recKey) { }
    }
}
