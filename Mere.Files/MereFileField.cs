using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mere.Files.interfaces;

namespace Mere.Files
{
    public class MereFileField : IMereFileField
    {
        public MereFileField(MereColumn mereColumnForField, string delimiter, int index, string recordKey, string toStringFormat, string padChar, List<MereFileParsingOption> fileFieldParsingOptions, int fieldLength)
        {
            MereColumnForField = mereColumnForField;
            Delimiter = delimiter;
            Index = index;
            RecordKey = recordKey;
            ToStringFormat = toStringFormat;
            PadChar = padChar ?? " ";
            FileFieldParsingOptions = fileFieldParsingOptions;
            FieldLength = fieldLength;
        }

        public MereColumn MereColumnForField { get; set; }
        public string Delimiter { get; set; }
        public int Index { get; set; }
        public string RecordKey { get; set; }
        public string ToStringFormat { get; set; }
        public string PadChar { get; set; }
        public List<MereFileParsingOption> FileFieldParsingOptions { get; set; }
        public int FieldLength { get; set; }
    }
}
