using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mere.Files.Attributes;

namespace Mere.Files
{
    public class MereFile<T> : MereFile where T : new()
    {
        public MereFile()
            : base(typeof(T)) { }

        private MereTableMin<T> _mereTable;
        public MereTableMin<T> MereTableMin
        {
            get { return _mereTable ?? (_mereTable = MereUtils.CacheCheck<T>()); }
            set { _mereTable = value; }
        }
    }

    public class MereFile
    {
        public MereFile(Type type)
        {
            MereTableMin = MereUtils.CacheCheck(type);
        }

        public static MereFile<T> Create<T>() where T : new()
        {
            return MereFileUtils.CacheCheckFile<T>();
        }

        public MereTableMin MereTableMin { get; set; }
        public MereFileField MereFileFieldForSubRecord { get; set; }

        public virtual bool DelimitedHasHeader { get; set; }
        public virtual bool FixedWidth { get; set; }
        public virtual string Delimiter { get; set; }
        public virtual string LineDelimiter { get; set; }
        public virtual string RacKey { get; set; }
        public virtual int Index { get; set; }

        private List<MereFile> _delimitedsubRecords;
        public List<MereFile> DelimitedSubRecords
        {
            get { return _delimitedsubRecords.OrderBy(x => x.Index).ToList(); }
            set
            {
                _delimitedsubRecords = value;
            }
        }

        private IList<MereFile> _flatFileSubRecords;
        public IList<MereFile> FlatFileSubRecords
        {
            get { return _flatFileSubRecords.OrderBy(x => x.Index).ToList(); }
            set
            {
                _flatFileSubRecords = value;
            }
        }

        public MereFileTableAttribute FlatFileTableAttr { get; set; }
        public MereFileTableAttribute DelimitedFileTableAttr { get; set; }

        public virtual Dictionary<Type, string> DelimitedFileTableTypeFormats { get; set; }
        public virtual Dictionary<Type, string> FlatFileTableTypeFormats { get; set; }

        public virtual Dictionary<Type, List<MereFileParsingOption>> DelimitedFileTableTypeParsingOptions { get; set; }
        public virtual Dictionary<Type, List<MereFileParsingOption>> FlatFileTableTypeParsingOptions { get; set; }
        
        public virtual List<MereFileParsingOption> DelimitedFileTableParsingOptions { get; set; }
        public virtual List<MereFileParsingOption> FlatFileTableParsingOptions { get; set; }

        private List<MereFileField> _delimitedFields;
        public virtual List<MereFileField> DelimitedFields
        {
            get { return _delimitedFields; }
            set
            {
                _delimitedFields = value == null
                    ? null
                    : value.OrderBy(x => x.Index).ToList();
            }
        }

        private List<MereFileField> _flatFileFields;
        public virtual List<MereFileField> FlatFileFields
        {
            get { return _flatFileFields; }
            set
            {
                _flatFileFields = value == null
                    ? null
                    : value.OrderBy(x => x.Index).ToList();
            }
        }

        #region Parsing
        public List<string> ParseForDelimitedWrite<T>(IEnumerable<T> records)
        {
            return ParseMultipleForWrite(records, DelimitedFields, DelimitedFileTableTypeParsingOptions,
                DelimitedFileTableParsingOptions, DelimitedFileTableTypeFormats, FixedWidth);
        }

        public List<string> ParseMultipleForDelimitedWrite<T>(IEnumerable<T> records)
        {
            return ParseMultipleForWrite(records, DelimitedFields, DelimitedFileTableTypeParsingOptions,
                DelimitedFileTableParsingOptions, DelimitedFileTableTypeFormats, FixedWidth);
        }

        public List<string> ParseForDelimitedWrite(object record)
        {
            return ParseForWrite(record, DelimitedFields, DelimitedFileTableTypeParsingOptions,
                DelimitedFileTableParsingOptions, DelimitedFileTableTypeFormats, FixedWidth);
        }

        public List<string> ParseForFlatFileWrite<T>(IEnumerable<T> records)
        {
            return ParseMultipleForWrite(records, FlatFileFields, FlatFileTableTypeParsingOptions,
                FlatFileTableParsingOptions, FlatFileTableTypeFormats, true);
        }

        public List<string> ParseForFlatFileWrite(object record)
        {
            return ParseForWrite(record, FlatFileFields, FlatFileTableTypeParsingOptions,
                FlatFileTableParsingOptions, FlatFileTableTypeFormats, true);
        }

        public List<string> ParseMultipleForWrite<T>(IEnumerable<T> records, List<MereFileField> fileFields,
            Dictionary<Type, List<MereFileParsingOption>> tableTypeParsingOptions,
            List<MereFileParsingOption> tableParsingOptions, Dictionary<Type, string> tableTypeFormats, bool fixedWidth)
        {
            var toReturn = new List<string>();
            foreach (var rec in records)
            {
                toReturn.AddRange(ParseForWrite(rec, fileFields,
            tableTypeParsingOptions,
            tableParsingOptions, tableTypeFormats, fixedWidth));
            }
            return toReturn;
        }

        public List<string> ParseForWrite(object record, List<MereFileField> fileFields,
            Dictionary<Type, List<MereFileParsingOption>> tableTypeParsingOptions,
            List<MereFileParsingOption> tableParsingOptions, Dictionary<Type, string> tableTypeFormats, bool fixedWidth)
        {
            var toReturn = new List<string>();
            MereFileField lastCol = null;
            var fields = new List<string>();

            fileFields.ForEach(field =>
            {
                var toAdd = "";
                if (lastCol != null)
                {
                    var diff = field.Index - (lastCol.Index + 1);
                    if (diff > 0)
                    {
                        fields.AddRange(new String[diff]);
                    }
                }

                var options = new List<MereFileParsingOption>();
                var toStringFormat = field.ToStringFormat;

                if (field.FileFieldParsingOptions != null)
                    options.AddRange(field.FileFieldParsingOptions);

                var fieldType = field.MereColumnForField.PropertyDescriptor.PropertyType;
                if (tableTypeParsingOptions.ContainsKey(fieldType))
                {
                    var toParse = tableTypeParsingOptions[fieldType].Where(x => field.FileFieldParsingOptions == null || field.FileFieldParsingOptions.All(a => a != x));
                    options.AddRange(toParse);
                }

                if (tableParsingOptions != null && tableParsingOptions.Any())
                {
                    var toParse = tableParsingOptions.Where(x => options.All(a => a != x));
                    options.AddRange(toParse);
                }

                if (toStringFormat == null && tableTypeFormats.ContainsKey(fieldType))
                {
                    toStringFormat = tableTypeFormats[fieldType];
                }

                toAdd = MereFileFieldParser.ParseFieldForWrite((options.Any() ? options : null), 
                    toStringFormat, field, field.MereColumnForField.Get(record), fixedWidth);

                fields.Add(toAdd);

                lastCol = field;
            });
            var d = DelimitedFileTableAttr == null ? Delimiter : DelimitedFileTableAttr.Delimiter ?? Delimiter;
            toReturn.Add(string.Join(d, fields));

            foreach (var rec in DelimitedSubRecords)
            {
                if (string.IsNullOrWhiteSpace(rec.Delimiter))
                    rec.Delimiter = d;

                var mereColumn = rec.MereFileFieldForSubRecord.MereColumnForField;
                var val = mereColumn.Get(record);

                if (val == null)
                    continue;

                //check if implements IEnumerable 
                var isEnum = typeof(IEnumerable).IsAssignableFrom(mereColumn.PropertyDescriptor.PropertyType);

                if (isEnum)
                {
                    var enType = mereColumn.PropertyDescriptor.PropertyType.GetGenericArguments()[0];

                    var meth = typeof(MereFile).GetMethod("ParseMultipleForWrite")
                        .MakeGenericMethod(new[] { enType });

                    toReturn.AddRange((List<string>)meth.Invoke(rec, new[] { val, rec.DelimitedFields, 
            tableTypeParsingOptions, 
            tableParsingOptions, tableTypeFormats, fixedWidth }));
                }
                else
                {
                    toReturn.AddRange(rec.ParseForDelimitedWrite(val));
                }
            }

            return toReturn;
        } 
        #endregion

        #region Delimited file Writing
        public static void WriteToDelimitedFile<T>(IEnumerable<T> toWrite, string filePath, bool append) where T : new()
        {
            var mereFile = Create<T>();
            mereFile.WriteDelimitedFile(toWrite, filePath, append);
        }

        public static void WriteSingleToDelimitedFile<T>(T toWrite, string filePath, bool append) where T : new()
        {
            var mereFile = Create<T>();
            mereFile.WriteSingleRecordToDelimitedFile(toWrite, filePath, append);
        }

        public void WriteDelimitedFile<T>(IEnumerable<T> toWrite, string filePath, bool append) where T : new()
        {
            LineDelimiter = LineDelimiter ?? "\r\n";
            Delimiter = Delimiter ?? ",";

            var ld = DelimitedFileTableAttr == null ? LineDelimiter : DelimitedFileTableAttr.LineDelimiter ?? LineDelimiter;
            var lines = ParseForDelimitedWrite(toWrite);

            lines.WriteLinesToFile(filePath, append, ld);
        }

        public void WriteSingleRecordToDelimitedFile<T>(T toWrite, string filePath, bool append) where T : new()
        {
            LineDelimiter = LineDelimiter ?? "\r\n";
            Delimiter = Delimiter ?? ",";

            var ld = DelimitedFileTableAttr == null ? LineDelimiter : DelimitedFileTableAttr.LineDelimiter ?? LineDelimiter;
            var lines = ParseForDelimitedWrite(toWrite);

            lines.WriteLinesToFile(filePath, append, ld);
        }
        #endregion

        #region Flat file writing
        public static void WriteToFlatFile<T>(IEnumerable<T> toWrite, string filePath, bool append) where T : new()
        {
            var mereFile = Create<T>();
            mereFile.WriteFlatFile(toWrite, filePath, append);
        }
        public static void WriteToFlatFile<T>(IEnumerable<T> toWrite, string filePath) where T : new()
        {
            var mereFile = Create<T>();
            mereFile.WriteFlatFile(toWrite, filePath);
        }

        public static void WriteSingleToFlatFile<T>(T toWrite, string filePath, bool append) where T : new()
        {
            var mereFile = Create<T>();
            mereFile.WriteSingleRecordToFlatFile(toWrite, filePath, append);
        }

        public static void WriteSingleToFlatFile<T>(T toWrite, string filePath) where T : new()
        {
            var mereFile = Create<T>();
            mereFile.WriteSingleRecordToFlatFile(toWrite, filePath);
        }

        public void WriteFlatFile<T>(IEnumerable<T> toWrite, string filePath) where T : new()
        {
            WriteFlatFile(toWrite, filePath, false);
        }

        public void WriteFlatFile<T>(IEnumerable<T> toWrite, string filePath, bool append) where T : new()
        {
            LineDelimiter = LineDelimiter ?? "\r\n";

            var ld = FlatFileTableAttr == null ? LineDelimiter : FlatFileTableAttr.LineDelimiter ?? LineDelimiter;
            var lines = ParseForFlatFileWrite(toWrite);

            lines.WriteLinesToFile(filePath, append, ld);
        }

        public void WriteSingleRecordToFlatFile<T>(T toWrite, string filePath) where T : new()
        {
            WriteSingleRecordToFlatFile(toWrite, filePath, false);
        }

        public void WriteSingleRecordToFlatFile<T>(T toWrite, string filePath, bool append) where T : new()
        {
            LineDelimiter = LineDelimiter ?? "\r\n";

            var ld = FlatFileTableAttr == null ? LineDelimiter : FlatFileTableAttr.LineDelimiter ?? LineDelimiter;
            var lines = ParseForFlatFileWrite(toWrite);

            lines.WriteLinesToFile(filePath, append, ld);
        }
        #endregion
    }

}
