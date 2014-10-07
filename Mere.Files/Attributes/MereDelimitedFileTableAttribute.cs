using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mere.Files.Attributes
{
    public class MereDelimitedFileTableAttribute : MereFileTableAttribute
    {
        public MereDelimitedFileTableAttribute(string delimiter = ",", string lineDelimiter = "\r\n", string recKey = null)
            : base(delimiter, lineDelimiter, recKey) { }
    }
}
