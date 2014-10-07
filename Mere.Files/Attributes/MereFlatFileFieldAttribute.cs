using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mere.Files.Attributes
{

    public class MereFlatFileFieldAttribute : MereFileFieldAttribute
    {
        //for nested sub records
        public MereFlatFileFieldAttribute(string recordKey)
            : base(recordKey) { }

        public MereFlatFileFieldAttribute(string recordKey, string delimiter)
            : base(recordKey, delimiter) { }

        public MereFlatFileFieldAttribute(string recordKey, int index)
            : base(recordKey, index) { }

        public MereFlatFileFieldAttribute(string recordKey, string delimiter, int index)
            : base(recordKey, delimiter, index) { }

        public MereFlatFileFieldAttribute(string recordKey, params MereFileParsingOption[] fieldOptions)
            : base(recordKey, fieldOptions) { }

        public MereFlatFileFieldAttribute(string recordKey, string delimiter, params MereFileParsingOption[] fieldOptions)
            : base(recordKey, delimiter, fieldOptions) { }

        public MereFlatFileFieldAttribute(string recordKey, string delimiter, int index, params MereFileParsingOption[] fieldOptions)
            : base(recordKey, delimiter, index, fieldOptions) { }

        public MereFlatFileFieldAttribute(string recordKey, int index, params MereFileParsingOption[] fieldOptions)
            : base(recordKey, index, fieldOptions) { }
        //end for nested sub records

       public MereFlatFileFieldAttribute(int index, int fieldLength) : base (index, fieldLength){ }

        public MereFlatFileFieldAttribute(int index, int fieldLength, string toStringFormat)
           : base(index, fieldLength, toStringFormat) { }

        public MereFlatFileFieldAttribute(int index, int fieldLength, string toStringFormat, string padChar)
            : base(index, fieldLength, toStringFormat, padChar) { }

        public MereFlatFileFieldAttribute(int index, int fieldLength, string toStringFormat, params MereFileParsingOption[] fieldOptions)
            : base(index, fieldLength, toStringFormat, fieldOptions) { }

        public MereFlatFileFieldAttribute(int index, int fieldLength, string toStringFormat, string padChar, params MereFileParsingOption[] fieldOptions)
            : base(index, fieldLength, toStringFormat, padChar, fieldOptions) { }

        public MereFlatFileFieldAttribute(int index, int fieldLength, params MereFileParsingOption[] fieldOptions)
            : base(index, fieldLength, fieldOptions) { }
    }
}
