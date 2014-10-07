using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mere.Files.Attributes;

namespace Mere.Files.interfaces
{
     public interface IMereFile
    {
        MereTableMin MereTableMin { get; set; }
        MereFileField MereFileFieldForSubRecord { get; set; }

        bool DelimitedHasHeader { get; set; }
        bool FixedWidth { get; set; }
        string Delimiter { get; set; }
        string RacKey { get; set; }
        int Index { get; set; }

        List<MereFile> SubRecords { get; set; }

        MereFileTableAttribute FileTableAttr { get; set; }
         
        Dictionary<Type, string> FileTableTypeFormats { get; set; }

        Dictionary<Type, List<MereFileParsingOption>> FileTableTypeParsingOptions { get; set; }

        List<MereFileParsingOption> FileTableParsingOptions { get; set; }

        List<MereFileField> FileFields { get; set; }
    }
}
