using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace Mere.Core
{
    public class MereDataReader<T> :
    IDataReader, IDisposable, IDataRecord where T : new()
    {
        protected readonly IDataReader DataReader;
        //private readonly MereTable _mereTable;
        protected readonly bool ByName;
        //private readonly bool _dynamicReturn;

        protected readonly List<MereColumn> SelectMereColumnsList;

        public MereDataReader(IDbCommand cmd)
        {
            DataReader = cmd.ExecuteReader();
            //_mereTable = MereUtils.CacheCheck<T>();
            SelectMereColumnsList = MereUtils.CacheCheck<T>().SelectMereColumns.ToList();
        }

        public MereDataReader(IDbCommand cmd, List<MereColumn> selectMereColumnsList)
        {
            DataReader = cmd.ExecuteReader();
            SelectMereColumnsList = selectMereColumnsList;
        }

        public MereDataReader(IDbCommand cmd, List<MereColumn> selectMereColumnsList, bool byName)
        {
            ByName = byName;
            DataReader = cmd.ExecuteReader();
            SelectMereColumnsList = selectMereColumnsList;
        }

        public MereDataReader(SqlCommand cmd, bool byName)
        {
            DataReader = cmd.ExecuteReader();
            //_mereTable = MereUtils.CacheCheck<T>();
            SelectMereColumnsList = MereUtils.CacheCheck<T>().SelectMereColumns.ToList();
            ByName = byName;
        }

        public MereDataReader(IDataReader reader)
        {
            DataReader = reader;
            //_mereTable = MereUtils.CacheCheck<T>();
            SelectMereColumnsList = MereUtils.CacheCheck<T>().SelectMereColumns.ToList();
        }

        public MereDataReader(IDataReader reader, List<MereColumn> selectMereColumnsList)
        {
            DataReader = reader;
            SelectMereColumnsList = selectMereColumnsList;
        }

        public MereDataReader(IDataReader reader, List<MereColumn> selectMereColumnsList, bool byName)
        {
            ByName = byName;
            DataReader = reader;
            SelectMereColumnsList = selectMereColumnsList;
        }

        public MereDataReader(IDataReader reader, bool byName)
        {
            DataReader = reader;
            //_mereTable = MereUtils.CacheCheck<T>();
            SelectMereColumnsList = MereUtils.CacheCheck<T>().SelectMereColumns.ToList();
            ByName = byName;
        }

        //public MereSqlDataReader(SqlDataReader reader, bool byName, bool dynamicReturn)
        //{
        //    _sqlDataReader = reader;
        //    //_mereTable = MereUtils.CacheCheck<T>();
        //    _selectMereColumnsList = MereUtils.CacheCheck<T>().PropertyHelpersIndexed.Values.ToList();
        //    _byName = byName;
        //    //_dynamicReturn = dynamicReturn;
        //}

        public static implicit operator T(MereDataReader<T> reader)
        {
            var n = new T();
            if (!reader.ByName)
            {
                if (reader.SelectMereColumnsList != null)
                {
                    for (var i = 0; i < reader.FieldCount; i++)
                    {
                        reader.SelectMereColumnsList[i].SetBase(n, reader.DataReader[i] is DBNull ? null : reader.DataReader[i]);
                    }
                }
            }
            else
            {
                for (var i = 0; i < reader.FieldCount; i++)
                {
                    var name = reader.GetName(i);

                    var mereColumn =
                        reader.SelectMereColumnsList.FirstOrDefault(
                            x => string.Compare(x.ColumnName, name, StringComparison.CurrentCultureIgnoreCase) == 0);

                    //var mereColumn =
                    //    reader._mereTable.PropertyHelpersColumnName.FirstOrDefault(
                    //        fd => fd.Key.ToLower() == name.ToLower()).Value ??
                    //    reader._mereTable.PropertyHelpersNamed.FirstOrDefault(
                    //        fd => fd.Key.ToLower() == name.ToLower()).Value;

                    if (mereColumn == null)
                        continue;

                    mereColumn.SetBase(n, reader.DataReader[i] is DBNull ? null : reader.DataReader[i]);
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
        public static implicit operator ExpandoObject(MereDataReader<T> reader)
        {
            var n = new ExpandoObject();
            var d = (IDictionary<string, object>)n;
            for (var i = 0; i < reader.FieldCount; i++)
            {
                var name = reader.GetName(i);
                d.Add(name, reader.DataReader[i] is DBNull ? null : reader.DataReader[i]);
            }
            return n;
        }

        object IDataRecord.this[int i]
        {
            get { return DataReader[i]; }
        }

        object IDataRecord.this[string name]
        {
            get { return DataReader[name]; }
        }

        public bool Read()
        {
            return DataReader.Read();
        }

        public virtual Task<bool> ReadAsync()
        {
            throw new NotImplementedException();
            //return _dataReader.ReadAsync();
        }

        public void Dispose()
        {
            DataReader.Dispose();
        }

        public string GetName(int i)
        {
            return DataReader.GetName(i);
        }

        public string GetDataTypeName(int i)
        {
            return DataReader.GetDataTypeName(i);
        }

        public Type GetFieldType(int i)
        {
            return DataReader.GetFieldType(i);
        }

        public object GetValue(int i)
        {
            return DataReader.GetValue(i);
        }

        public int GetValues(object[] values)
        {
            return DataReader.GetValues(values);
        }

        public int GetOrdinal(string name)
        {
            return DataReader.GetOrdinal(name);
        }

        public bool GetBoolean(int i)
        {
            return DataReader.GetBoolean(i);
        }

        public byte GetByte(int i)
        {
            return DataReader.GetByte(i);
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            return DataReader.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
        }

        public char GetChar(int i)
        {
            return DataReader.GetChar(i);
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            return DataReader.GetChars(i, fieldoffset, buffer, bufferoffset, length);
        }

        public Guid GetGuid(int i)
        {
            return DataReader.GetGuid(i);
        }

        public short GetInt16(int i)
        {
            return DataReader.GetInt16(i);
        }

        public int GetInt32(int i)
        {
            return DataReader.GetInt32(i);
        }

        public long GetInt64(int i)
        {
            return DataReader.GetInt64(i);
        }

        public float GetFloat(int i)
        {
            return DataReader.GetFloat(i);
        }

        public double GetDouble(int i)
        {
            return DataReader.GetDouble(i);
        }

        public string GetString(int i)
        {
            return DataReader.GetString(i);
        }

        public decimal GetDecimal(int i)
        {
            return DataReader.GetDecimal(i);
        }

        public DateTime GetDateTime(int i)
        {
            return DataReader.GetDateTime(i);
        }

        public IDataReader GetData(int i)
        {
            return DataReader.GetData(i);
        }

        public bool IsDBNull(int i)
        {
            return DataReader.IsDBNull(i);
        }

        private int _fieldCount;
        public int FieldCount
        {
            get { return DataReader.FieldCount; }
            private set { _fieldCount = value; }
        }

        public void Close()
        {
            DataReader.Close();
        }

        public DataTable GetSchemaTable()
        {
            return DataReader.GetSchemaTable();
        }

        public bool NextResult()
        {
            return DataReader.NextResult();
        }

        private int _depth;
        public int Depth { get { return DataReader.Depth; } private set { _depth = value; } }
        private bool _isClosed;
        public bool IsClosed { get { return DataReader.IsClosed; } private set { _isClosed = value; } }
        private int _recordsAffected;
        public int RecordsAffected { get { return DataReader.RecordsAffected; } private set { _recordsAffected = value; } }
    }
}