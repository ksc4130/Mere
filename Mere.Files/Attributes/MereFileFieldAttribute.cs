using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mere.Files.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public abstract class MereFileFieldAttribute : Attribute, IMereFileFieldAttribute
    {
        public int Index { get; set; }
        public int FieldLength { get; set; }
        public List<MereFileParsingOption> FileFieldParsingOptions { get; set; }
        public string PadChar { get; set; }
        public string RecordKey { get; set; }
        public string Delimiter { get; set; }
        public string ToStringFormat { get; set; }
        
        //for nested sub records
        protected MereFileFieldAttribute(string recordKey)
        {
            RecordKey = recordKey;
        }

        protected MereFileFieldAttribute(string recordKey, string delimiter)
        {
            RecordKey = recordKey;
            Delimiter = delimiter;
        }

        protected MereFileFieldAttribute(string recordKey, string delimiter, int index)
        {
            Delimiter = delimiter;
            RecordKey = recordKey;
            Index = index;
        }

        protected MereFileFieldAttribute(string recordKey, int index)
        {
            RecordKey = recordKey;
            Index = index;
        }

        protected MereFileFieldAttribute(string recordKey, params MereFileParsingOption[] fieldOptions)
        {
            FileFieldParsingOptions = fieldOptions.ToList();
            RecordKey = recordKey;
        }

        protected MereFileFieldAttribute(string recordKey, string delimiter, params MereFileParsingOption[] fieldOptions)
        {
            Delimiter = delimiter;
            FileFieldParsingOptions = fieldOptions.ToList();
            RecordKey = recordKey;
        }

        protected MereFileFieldAttribute(string recordKey, string delimiter, int index, params MereFileParsingOption[] fieldOptions)
        {
            Delimiter = delimiter;
            FileFieldParsingOptions = fieldOptions.ToList();
            RecordKey = recordKey;
            Index = index;
        }

        protected MereFileFieldAttribute(string recordKey, int index, params MereFileParsingOption[] fieldOptions)
        {
            FileFieldParsingOptions = fieldOptions.ToList();
            RecordKey = recordKey;
            Index = index;
        }
        //end for nested sub records
        
        protected MereFileFieldAttribute(int index)
        {
            PadChar = " ";
            Index = index;
        }

        protected MereFileFieldAttribute(int index, int fieldLength)
        {
            PadChar = " ";
            Index = index;
            FieldLength = fieldLength;
        }

        protected MereFileFieldAttribute(int index, int fieldLength, string toStringFormat)
        {
            Index = index;
            FieldLength = fieldLength;
            ToStringFormat = toStringFormat;
        }

        protected MereFileFieldAttribute(int index, int fieldLength, string toStringFormat, string padChar)
        {
            Index = index;
            FieldLength = fieldLength;
            PadChar = padChar;
            ToStringFormat = toStringFormat;
        }
        protected MereFileFieldAttribute(int index, int fieldLength, string toStringFormat, params MereFileParsingOption[] fieldOptions)
        {
            Index = index;
            FieldLength = fieldLength;
            ToStringFormat = toStringFormat;
            FileFieldParsingOptions = fieldOptions.ToList();
        }

        protected MereFileFieldAttribute(int index, int fieldLength, string toStringFormat, string padChar, params MereFileParsingOption[] fieldOptions)
        {
            Index = index;
            FieldLength = fieldLength;
            PadChar = padChar;
            FileFieldParsingOptions = fieldOptions.ToList();
            ToStringFormat = toStringFormat;
        }

        protected MereFileFieldAttribute(int index, params MereFileParsingOption[] fieldOptions)
        {
            PadChar = " ";
            Index = index;
            FileFieldParsingOptions = fieldOptions.ToList();
        }

        protected MereFileFieldAttribute(int index, int fieldLength, params MereFileParsingOption[] fieldOptions)
        {
            PadChar = " ";
            Index = index;
            FieldLength = fieldLength;
            FileFieldParsingOptions = fieldOptions.ToList();
        }
    }
}
