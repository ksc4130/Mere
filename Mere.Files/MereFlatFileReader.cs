//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Globalization;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Mere.Attributes;
//
//namespace Mere.Files
//{
//    public class MereFlatFileReader<T> :
//        IDataReader, IDisposable, IDataRecord where T : new()
//    {
//        private FileStream _fileStream;
//        private StreamReader _streamReader;
//        private readonly bool _byName;
//        private string _curLine;
//        private const Int32 BufferSize = 128;
//
//        private readonly MereTable<T> _mereTable;
//        private readonly List<MereColumn> _flatFileFields;
//
//        public MereFlatFileReader(string filePath)
//        {
//            _fileStream = File.OpenRead(filePath);
//            _streamReader = new StreamReader(_fileStream, Encoding.UTF8, true, BufferSize);
//            _mereTable = MereUtils.CacheCheckFile<T>();
//            _flatFileFields = _mereTable.FlatFileFields.ToList();
//        }
//
//        //public MereFlatFileReader(string filePath, List<MereColumn> selectMereColumnsList)
//        //{
//        //    _fileStream = File.OpenRead(filePath);
//        //    _selectMereColumnsList = selectMereColumnsList;
//        //}
//
//        //public MereFlatFileReader(string filePath, List<MereColumn> selectMereColumnsList, bool byName)
//        //{
//        //    _byName = byName;
//        //    _fileStream = File.OpenRead(filePath);
//        //    _selectMereColumnsList = selectMereColumnsList;
//        //}
//
//        public static implicit operator T(MereFlatFileReader<T> reader)
//        {
//            var n = Activator.CreateInstance<T>();
//            //if (!reader._byName)
//            //{
//            if (reader._flatFileFields != null)
//            {
//                for (var i = 0; i < reader._flatFileFields.Count; i++)
//                {
//                    var field = reader._flatFileFields[i];
//                    var attr = field.FlatFileFieldAttr;
//                    //var val = reader.IsDBNull(i) ? null : reader._curLine.Substring(attr.Index, attr.Length);
//                    var toReturn = reader._curLine == null
//                                       ? null
//                                       : reader._curLine.Substring(attr.Index, attr.Length);
//
//                    if (toReturn != null)
//                    {
//                        var options = attr.FieldOptions;
//
//                        if (options != null)
//                        {
//                            var l = string.IsNullOrEmpty(toReturn) ? ' ' : toReturn.Last();
//
//                            if (options.Contains(MereFileParsingOption.Ebcidic))
//                            {
//                                var pos = new[] { '{', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', ' ' };
//                                var neg = new[] { '}', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', ' ' };
//                                if (neg.Contains(l) || pos.Contains(l))
//                                {
//                                    var isNeg = neg.Contains(l);
//                                    var index = isNeg
//                                                    ? Array.IndexOf(neg, l)
//                                                    : Array.IndexOf(pos, l);
//
//                                    if (isNeg)
//                                        toReturn = "-" + toReturn;
//
//                                    toReturn =
//                                        string.Join("", toReturn.Take(toReturn.Length - 1)) + index;
//                                }
//                            }
//
//                            foreach (var option in options)
//                            {
//                                int precision;
//                                switch (option)
//                                {
//                                    case MereFileParsingOption.TrailingNegative:
//
//                                        if (l == '-')
//                                        {
//                                            toReturn = "-" + toReturn.TrimEnd('-');
//                                        }
//                                        break;
//                                    case MereFileParsingOption.RemoveAllSpaces:
//                                        toReturn = toReturn.Replace(" ", "");
//                                        break;
//                                    //case MereFileParsingOption.TrimStartSpaces:
//
//                                    //case MereFileParsingOption.TrimEndSpaces:
//
//                                    //case MereFileParsingOption.Trim:
//
//                                    //case MereFileParsingOption.RemoveDoubleSpaces:
//
//                                    case MereFileParsingOption.RemovedDecimal1:
//                                        precision = 1;
//                                        toReturn =
//                                            string.Join("",
//                                                        toReturn.Take(toReturn.Length - precision)) +
//                                            "." +
//                                            string.Join("",
//                                                        toReturn.Skip(toReturn.Length - precision));
//                                        break;
//                                    case MereFileParsingOption.RemovedDecimal2:
//                                        precision = 2;
//                                        toReturn =
//                                            string.Join("",
//                                                        toReturn.Take(toReturn.Length - precision)) +
//                                            "." +
//                                            string.Join("",
//                                                        toReturn.Skip(toReturn.Length - precision));
//                                        break;
//                                    case MereFileParsingOption.RemovedDecimal3:
//                                        precision = 3;
//                                        toReturn =
//                                            string.Join("",
//                                                        toReturn.Take(toReturn.Length - precision)) +
//                                            "." +
//                                            string.Join("",
//                                                        toReturn.Skip(toReturn.Length - precision));
//                                        break;
//                                    case MereFileParsingOption.RemovedDecimal4:
//                                        precision = 4;
//                                        toReturn =
//                                            string.Join("",
//                                                        toReturn.Take(toReturn.Length - precision)) +
//                                            "." +
//                                            string.Join("",
//                                                        toReturn.Skip(toReturn.Length - precision));
//                                        break;
//                                    case MereFileParsingOption.RemovedDecimal5:
//                                        precision = 5;
//                                        toReturn =
//                                            string.Join("",
//                                                        toReturn.Take(toReturn.Length - precision)) +
//                                            "." +
//                                            string.Join("",
//                                                        toReturn.Skip(toReturn.Length - precision));
//                                        break;
//                                    case MereFileParsingOption.RemovedDecimal6:
//                                        precision = 6;
//                                        toReturn =
//                                            string.Join("",
//                                                        toReturn.Take(toReturn.Length - precision)) +
//                                            "." +
//                                            string.Join("",
//                                                        toReturn.Skip(toReturn.Length - precision));
//                                        break;
//
//                                }
//                            } //end foreach option
//                        }
//
//
//
//                        var toStringFormat = attr.ToStringFormat;
//                        if (!string.IsNullOrEmpty(toReturn) && toStringFormat != null)
//                        {
//
//                            if (field.PropertyDescriptor.PropertyType == typeof(DateTime) ||
//                                field.PropertyDescriptor.PropertyType == typeof(DateTime?))
//                            {
//                                DateTime d;
//                                if (DateTime.TryParseExact(toReturn, toStringFormat,
//                                                           new CultureInfo("en-US"),
//                                                           DateTimeStyles.None, out d))
//                                    // field.Get(value);
//                                    toReturn = d.ToString();
//                            }
//                        }
//                    }
//
//                    reader._flatFileFields[i].Set(n, toReturn);
//                }
//                return n;
//            }
//
//            //}
//            //else
//            //{
//            //    for (var i = 0; i < reader.FieldCount; i++)
//            //    {
//            //        var name = reader.GetName(i);
//
//            //        var mereColumn =
//            //            reader._flatFileFields.FirstOrDefault(
//            //                x => string.Compare(x.ColumnName, name, StringComparison.CurrentCultureIgnoreCase) == 0);
//
//            //        //var mereColumn =
//            //        //    reader._mereTable.PropertyHelpersColumnName.FirstOrDefault(
//            //        //        fd => fd.Key.ToLower() == name.ToLower()).Value ??
//            //        //    reader._mereTable.PropertyHelpersNamed.FirstOrDefault(
//            //        //        fd => fd.Key.ToLower() == name.ToLower()).Value;
//
//            //        if (mereColumn == null)
//            //            continue;
//
//            //        mereColumn.Set(n, reader._fileStream[i] is DBNull ? null : reader._fileStream[i]);
//            //    }
//            //}
//            return n;
//        }
//
//        //public void BinaryStream(BinaryWriter writer)
//        //{
//        //    for (var i = 0; i < FieldCount; i++)
//        //    {
//        //        var val = _fileStream[i] is DBNull ? null : _fileStream[i].ToString();
//        //        writer.Write(val);
//        //    }
//        //}
//
//        //was used
//        //public static implicit operator ExpandoObject(MereSqlDataReader<T> reader)
//        //{
//        //    var n = new ExpandoObject();
//        //    var d = (IDictionary<string, object>)n;
//        //    for (var i = 0; i < reader.FieldCount; i++)
//        //    {
//        //        var name = reader.GetName(i);
//        //        d.Add(name, reader._fileStream[i] is DBNull ? null : reader._fileStream[i]);
//        //    }
//        //    return n;
//        //}
//
//        object IDataRecord.this[int i]
//        {
//            get
//            {
//                var attr = _flatFileFields[i].FlatFileFieldAttr;
//                return attr != null ? _curLine.Substring(attr.Index, attr.Length) : null;
//            }
//        }
//
//        object IDataRecord.this[string name]
//        {
//            get
//            {
//                var field = _flatFileFields.FirstOrDefault(x => string.Compare(x.ColumnName, name, StringComparison.CurrentCultureIgnoreCase) == 0);
//                if (field == null)
//                    return null;
//                var attr = field.FlatFileFieldAttr;
//                return attr != null ? _curLine.Substring(attr.Index, attr.Length) : null;
//            }
//        }
//
//        public bool Read()
//        {
//            _curLine = _streamReader.ReadLine();
//            return _curLine != null;
//        }
//
//        public async Task<bool> ReadAsync()
//        {
//            _curLine = await _streamReader.ReadLineAsync();
//            return _curLine != null;
//        }
//
//        public void Dispose()
//        {
//            _streamReader.Dispose();
//            _fileStream.Dispose();
//        }
//
//        public void Close()
//        {
//            _streamReader.Close();
//            _fileStream.Close();
//        }
//
//        // ReSharper disable InconsistentNaming
//        public bool IsDBNull(int i)
//        // ReSharper restore InconsistentNaming
//        {
//            var attr = _flatFileFields[i].FlatFileFieldAttr;
//            return attr == null;
//        }
//
//        public int FieldCount { get; private set; }
//
//        #region NotImplemented
//
//        public string GetName(int i)
//        {
//            throw new NotImplementedException();
//        }
//
//        public string GetDataTypeName(int i)
//        {
//            throw new NotImplementedException();
//        }
//
//        public Type GetFieldType(int i)
//        {
//            throw new NotImplementedException();
//        }
//
//        public object GetValue(int i)
//        {
//            throw new NotImplementedException();
//        }
//
//        public int GetValues(object[] values)
//        {
//            throw new NotImplementedException();
//        }
//
//        public int GetOrdinal(string name)
//        {
//            throw new NotImplementedException();
//        }
//
//        public bool GetBoolean(int i)
//        {
//            throw new NotImplementedException();
//        }
//
//        public byte GetByte(int i)
//        {
//            throw new NotImplementedException();
//        }
//
//        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
//        {
//            throw new NotImplementedException();
//        }
//
//        public char GetChar(int i)
//        {
//            throw new NotImplementedException();
//        }
//
//        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
//        {
//            throw new NotImplementedException();
//        }
//
//        public Guid GetGuid(int i)
//        {
//            throw new NotImplementedException();
//        }
//
//        public short GetInt16(int i)
//        {
//            throw new NotImplementedException();
//        }
//
//        public int GetInt32(int i)
//        {
//            throw new NotImplementedException();
//        }
//
//        public long GetInt64(int i)
//        {
//            throw new NotImplementedException();
//        }
//
//        public float GetFloat(int i)
//        {
//            throw new NotImplementedException();
//        }
//
//        public double GetDouble(int i)
//        {
//            throw new NotImplementedException();
//        }
//
//        public string GetString(int i)
//        {
//            throw new NotImplementedException();
//        }
//
//        public decimal GetDecimal(int i)
//        {
//            throw new NotImplementedException();
//        }
//
//        public DateTime GetDateTime(int i)
//        {
//            throw new NotImplementedException();
//        }
//
//        public IDataReader GetData(int i)
//        {
//            throw new NotImplementedException();
//        }
//
//        public DataTable GetSchemaTable()
//        {
//            throw new NotImplementedException();
//        }
//
//        public bool NextResult()
//        {
//            throw new NotImplementedException();
//        }
//        #endregion
//
//        public int Depth { get; private set; }
//        public bool IsClosed { get; private set; }
//        public int RecordsAffected { get; private set; }
//    }
//}
