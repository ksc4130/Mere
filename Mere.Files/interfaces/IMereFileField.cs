using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mere.Files.Attributes;

namespace Mere.Files.interfaces
{
    public interface IMereFileField : IMereFileFieldAttribute
    {
        MereColumn MereColumnForField { get; set; }
    }
}
