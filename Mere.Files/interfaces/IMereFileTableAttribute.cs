using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Mere.Files.Attributes
{
    public interface IMereFileTableAttribute
    {
        string Delimiter { get; set; }
        string RecKey { get; set; }
    }
}
