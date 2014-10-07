using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace Mere
{
    public class MereSqlDataReader<T> :
    IDataReader, IDisposable, IDataRecord where T : new()
    {
        private readonly SqlDataReader _sqlDataReader;
        //private readonly MereTable _mereTable;
        private readonly bool _byName;
        //private readonly bool _dynamicReturn;

        private readonly List<MereColumn> _selectMereColumnsList; 

        public MereSqlDataReader(SqlCommand cmd)
        {
            _sqlDataReader = cmd.ExecuteReader();
            //_mereTable = MereUtils.CacheCheck<T>();
            _selectMereColumnsList = MereUtils.CacheCheck<T>().SelectMereColumns.ToList();
        }

        public MereSqlDataReader(SqlCommand cmd, List<MereColumn> selectMereColumnsList)
        {
            _sqlDataReader = cmd.ExecuteReader();
            _selectMereColumnsList = selectMereColumnsList;
        }

        public MereSqlDataReader(SqlCommand cmd, List<MereColumn> selectMereColumnsList, bool byName)
        {
            _byName = byName;
            _sqlDataReader = cmd.ExecuteReader();
            _selectMereColumnsList = selectMereColumnsList;
        }

        public MereSqlDataReader(SqlCommand cmd, bool byName)
        {
            _sqlDataReader = cmd.ExecuteReader();
            //_mereTable = MereUtils.CacheCheck<T>();
            _selectMereColumnsList = MereUtils.CacheCheck<T>().SelectMereColumns.ToList();
            _byName = byName;
        }

        public MereSqlDataReader(SqlDataReader reader)
        {
            _sqlDataReader = reader;
            //_mereTable = MereUtils.CacheCheck<T>();
            _selectMereColumnsList = MereUtils.CacheCheck<T>().SelectMereColumns.ToList();
        }

        public MereSqlDataReader(SqlDataReader reader, List<MereColumn> selectMereColumnsList)
        {
            _sqlDataReader = reader;
            _selectMereColumnsList = selectMereColumnsList;
        }

        public MereSqlDataReader(SqlDataReader reader, List<MereColumn> selectMereColumnsList, bool byName)
        {
            _byName = byName;
            _sqlDataReader = reader;
            _selectMereColumnsList = selectMereColumnsList;
        }

        public MereSqlDataReader(SqlDataReader reader, bool byName)
        {
            _sqlDataReader = reader;
            //_mereTable = MereUtils.CacheCheck<T>();
            _selectMereColumnsList = MereUtils.CacheCheck<T>().SelectMereColumns.ToList();
            _byName = byName;
        }

        //public MereSqlDataReader(SqlDataReader reader, bool byName, bool dynamicReturn)
        //{
        //    _sqlDataReader = reader;
        //    //_mereTable = MereUtils.CacheCheck<T>();
        //    _selectMereColumnsList = MereUtils.CacheCheck<T>().PropertyHelpersIndexed.Values.ToList();
        //    _byName = byName;
        //    //_dynamicReturn = dynamicReturn;
        //}

        public static implicit operator T(MereSqlDataReader<T> reader)
        {
            var n = new T();
            if (!reader._byName)
            {
                if (reader._selectMereColumnsList != null)
                {
                    for (var i = 0; i < reader.FieldCount; i++)
                    {
                        reader._selectMereColumnsList[i].SetBase(n, reader._sqlDataReader[i] is DBNull ? null : reader._sqlDataReader[i]);
                    }
                }
            }
            else
            {
                for (var i = 0; i < reader.FieldCount; i++)
                {
                    var name = reader.GetName(i);

                    var mereColumn =
                        reader._selectMereColumnsList.FirstOrDefault(
                            x => string.Compare(x.ColumnName, name, StringComparison.CurrentCultureIgnoreCase) == 0);

                    //var mereColumn =
                    //    reader._mereTable.PropertyHelpersColumnName.FirstOrDefault(
                    //        fd => fd.Key.ToLower() == name.ToLower()).Value ??
                    //    reader._mereTable.PropertyHelpersNamed.FirstOrDefault(
                    //        fd => fd.Key.ToLower() == name.ToLower()).Value;

                    if (mereColumn == null)
                        continue;

                    mereColumn.SetBase(n, reader._sqlDataReader[i] is DBNull ? null : reader._sqlDataReader[i]);
                }
            }
            return n;
        }

        //public void BinaryStream(BinaryWriter writer)
        //{
        //    for (var i = 0; i < FieldCount; i++)
        //    {
        //        var val = _sqlDataReader[i] is DBNull ? null : _sqlDataReader[i].ToString();
        //        writer.Write(val);
        //    }
        //}
        public static implicit operator ExpandoObject(MereSqlDataReader<T> reader)
        {
            var n = new ExpandoObject();
            var d = (IDictionary<string, object>)n;
            for (var i = 0; i < reader.FieldCount; i++)
            {
                var name = reader.GetName(i);
                d.Add(name, reader._sqlDataReader[i] is DBNull ? null : reader._sqlDataReader[i]);
            }
            return n;
        }

        object IDataRecord.this[int i]
        {
            get { return _sqlDataReader[i]; }
        }

        object IDataRecord.this[string name]
        {
            get { return _sqlDataReader[name]; }
        }

        public bool Read()
        {
            return _sqlDataReader.Read();
        }

        public Task<bool> ReadAsync()
        {
            return _sqlDataReader.ReadAsync();
        }

        public void Dispose()
        {
            _sqlDataReader.Dispose();
        }

        public string GetName(int i)
        {
            return _sqlDataReader.GetName(i);
        }

        public string GetDataTypeName(int i)
        {
            return _sqlDataReader.GetDataTypeName(i);
        }

        public Type GetFieldType(int i)
        {
            return _sqlDataReader.GetFieldType(i);
        }

        public object GetValue(int i)
        {
            return _sqlDataReader.GetValue(i);
        }

        public int GetValues(object[] values)
        {
            return _sqlDataReader.GetValues(values);
        }

        public int GetOrdinal(string name)
        {
            return _sqlDataReader.GetOrdinal(name);
        }

        public bool GetBoolean(int i)
        {
            return _sqlDataReader.GetBoolean(i);
        }

        public byte GetByte(int i)
        {
            return _sqlDataReader.GetByte(i);
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            return _sqlDataReader.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
        }

        public char GetChar(int i)
        {
            return _sqlDataReader.GetChar(i);
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            return _sqlDataReader.GetChars(i, fieldoffset, buffer, bufferoffset, length);
        }

        public Guid GetGuid(int i)
        {
            return _sqlDataReader.GetGuid(i);
        }

        public short GetInt16(int i)
        {
            return _sqlDataReader.GetInt16(i);
        }

        public int GetInt32(int i)
        {
            return _sqlDataReader.GetInt32(i);
        }

        public long GetInt64(int i)
        {
            return _sqlDataReader.GetInt64(i);
        }

        public float GetFloat(int i)
        {
            return _sqlDataReader.GetFloat(i);
        }

        public double GetDouble(int i)
        {
            return _sqlDataReader.GetDouble(i);
        }

        public string GetString(int i)
        {
            return _sqlDataReader.GetString(i);
        }

        public decimal GetDecimal(int i)
        {
            return _sqlDataReader.GetDecimal(i);
        }

        public DateTime GetDateTime(int i)
        {
            return _sqlDataReader.GetDateTime(i);
        }

        public IDataReader GetData(int i)
        {
            return _sqlDataReader.GetData(i);
        }

        public bool IsDBNull(int i)
        {
            return _sqlDataReader.IsDBNull(i);
        }

        private int _fieldCount;
        public int FieldCount { get { return _sqlDataReader.FieldCount; }
            private set { _fieldCount = value; }
        }

        public void Close()
        {
            _sqlDataReader.Close();
        }

        public DataTable GetSchemaTable()
        {
            return _sqlDataReader.GetSchemaTable();
        }

        public bool NextResult()
        {
            return _sqlDataReader.NextResult();
        }

        private int _depth;
        public int Depth { get { return _sqlDataReader.Depth; } private set { _depth = value; } }
        private bool _isClosed;
        public bool IsClosed { get { return _sqlDataReader.IsClosed; } private set { _isClosed = value; } }
        private int _recordsAffected;
        public int RecordsAffected { get { return _sqlDataReader.RecordsAffected; } private set { _recordsAffected = value; } }
    }

}
