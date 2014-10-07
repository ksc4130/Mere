using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mere.Files.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public abstract class MereFileTableAttribute : Attribute, IMereFileTableAttribute
    {
        public string Delimiter { get; set; }
        public string LineDelimiter { get; set; }
        public string RecKey { get; set; }

        protected MereFileTableAttribute(string delimiter = ",", string lineDelimiter = "\r\n", string recKey = null)
        {
            Delimiter = delimiter;
            LineDelimiter = lineDelimiter;
            RecKey = recKey;
        }
    }
}
