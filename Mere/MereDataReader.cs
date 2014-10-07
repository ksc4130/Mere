using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Mere
{
    public class MereDataReader<T> : IDataReader where T : new()
    {
        private readonly IList<T> _list;
        private int _curIndex = -1;
        private readonly MereTableMin _mereTable;
        public int FieldCount { get; private set; }
        public bool TruncateLength { get; set; }

        public MereDataReader(IEnumerable<T> list)
        {
            if (list == null)
                throw new NullReferenceException("list was null or empty");

            _mereTable = MereUtils.CacheCheck<T>();
            FieldCount = _mereTable.SelectMereColumns.Count;
            _list = list.ToList();
        }

        public MereDataReader(IEnumerable<T> list, bool truncateLength)
        {
            TruncateLength = truncateLength;
            if (list == null)
                throw new NullReferenceException("list was null or empty");

            _mereTable = MereUtils.CacheCheck<T>();
            FieldCount = _mereTable.SelectMereColumns.Count;
            _list = list.ToList();
        }

        public string GetName(int i)
        {
            if (_mereTable.SelectMereColumns.Count - 1 < i)
                return string.Empty;

            return _mereTable.SelectMereColumns[i].ColumnName;
        }

        public object GetValue(int i)
        {
            if (_mereTable.SelectMereColumns.Count - 1 < i)
                return null;
            var toReturn = _mereTable.SelectMereColumns[i].Get(_list[_curIndex], TruncateLength);
            return toReturn;
        }

        public int GetOrdinal(string name)
        {
            var r = _mereTable.GetMereColumn(name);
            if (r == null)
                return -1;

            return _mereTable.SelectMereColumns.IndexOf(r);
        }

        public bool Read()
        {
            if ((_curIndex + 1) < _list.Count)
            {
                _curIndex++;
                return true;
            }
            else
            {
                return false;
            }

        }

        // ReSharper disable InconsistentNaming
        public bool IsDBNull(int i)
        // ReSharper restore InconsistentNaming
        {
            if (_mereTable.SelectMereColumns.Count - 1 < i)
            { return true; }
            var val = _mereTable.SelectMereColumns[i].Get(_list[_curIndex], TruncateLength);
            return val == null || val == DBNull.Value;
        }

        #region NotImplemented
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public string GetDataTypeName(int i)
        {
            throw new NotImplementedException();
        }

        public Type GetFieldType(int i)
        {
            throw new NotImplementedException();
        }

        public int GetValues(object[] values)
        {
            throw new NotImplementedException();
        }

        public bool GetBoolean(int i)
        {
            throw new NotImplementedException();
        }

        public byte GetByte(int i)
        {
            throw new NotImplementedException();
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public char GetChar(int i)
        {
            throw new NotImplementedException();
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public Guid GetGuid(int i)
        {
            throw new NotImplementedException();
        }

        public short GetInt16(int i)
        {
            throw new NotImplementedException();
        }

        public int GetInt32(int i)
        {
            throw new NotImplementedException();
        }

        public long GetInt64(int i)
        {
            throw new NotImplementedException();
        }

        public float GetFloat(int i)
        {
            throw new NotImplementedException();
        }

        public double GetDouble(int i)
        {
            throw new NotImplementedException();
        }

        public string GetString(int i)
        {
            throw new NotImplementedException();
        }

        public decimal GetDecimal(int i)
        {
            throw new NotImplementedException();
        }

        public DateTime GetDateTime(int i)
        {
            throw new NotImplementedException();
        }

        public IDataReader GetData(int i)
        {
            throw new NotImplementedException();
        }

        object IDataRecord.this[int i]
        {
            get { throw new NotImplementedException(); }
        }

        object IDataRecord.this[string name]
        {
            get { throw new NotImplementedException(); }
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public DataTable GetSchemaTable()
        {
            throw new NotImplementedException();
        }

        public bool NextResult()
        {
            throw new NotImplementedException();
        }
        #endregion

        public int Depth { get; private set; }
        public bool IsClosed { get; private set; }
        public int RecordsAffected { get; private set; }
    }
}
