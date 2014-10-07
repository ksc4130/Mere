using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mere.Files.Attributes
{
    public interface IMereFileFieldAttribute
    {
        int Index { get; set; }
        string ToStringFormat { get; set; }
        string PadChar { get; set; }
        List<MereFileParsingOption> FileFieldParsingOptions { get; set; }
        int FieldLength { get; set; }
        string RecordKey { get; set; }
        string Delimiter { get; set; }
    }
}
