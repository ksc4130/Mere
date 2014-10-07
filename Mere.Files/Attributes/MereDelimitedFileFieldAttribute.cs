using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mere.Files.Attributes
{
    public class MereDelimitedFileFieldAttribute : MereFileFieldAttribute
    {
        //for nested sub records
        public MereDelimitedFileFieldAttribute(string recordKey)
            : base(recordKey) { }

        public MereDelimitedFileFieldAttribute(string recordKey, string delimiter)
            : base(recordKey, delimiter) { }

        public MereDelimitedFileFieldAttribute(string recordKey, int index)
            : base(recordKey, index) { }

        public MereDelimitedFileFieldAttribute(string recordKey, string delimiter, int index)
            : base(recordKey, delimiter, index) { }

        public MereDelimitedFileFieldAttribute(string recordKey, params MereFileParsingOption[] fieldOptions)
            : base(recordKey, fieldOptions) { }

        public MereDelimitedFileFieldAttribute(string recordKey, string delimiter, params MereFileParsingOption[] fieldOptions)
            : base(recordKey, delimiter, fieldOptions) { }

        public MereDelimitedFileFieldAttribute(string recordKey, string delimiter, int index, params MereFileParsingOption[] fieldOptions)
            : base(recordKey, delimiter, index, fieldOptions) { }

        public MereDelimitedFileFieldAttribute(string recordKey, int index, params MereFileParsingOption[] fieldOptions)
            : base(recordKey, index, fieldOptions) { }
        //end for nested sub records

       public MereDelimitedFileFieldAttribute(int index) : base (index){ }

        public MereDelimitedFileFieldAttribute(int index,  string toStringFormat)
           : base(index, 0, toStringFormat) { }

        public MereDelimitedFileFieldAttribute(int index, int fieldLength, string toStringFormat)
           : base(index, fieldLength, toStringFormat) { }

        public MereDelimitedFileFieldAttribute(int index, int fieldLength, string toStringFormat, string padChar)
            : base(index, fieldLength, toStringFormat, padChar) { }

        public MereDelimitedFileFieldAttribute(int index, int fieldLength, string toStringFormat, params MereFileParsingOption[] fieldOptions)
            : base(index, fieldLength, toStringFormat, fieldOptions) { }

        public MereDelimitedFileFieldAttribute(int index, int fieldLength, string toStringFormat, string padChar, params MereFileParsingOption[] fieldOptions)
            : base(index, fieldLength, toStringFormat) { }

        public MereDelimitedFileFieldAttribute(int index, int fieldLength, params MereFileParsingOption[] fieldOptions)
            : base(index, fieldLength, fieldOptions) { }

        public MereDelimitedFileFieldAttribute(int index, params MereFileParsingOption[] fieldOptions)
            : base(index, fieldOptions) { }
    }
}
