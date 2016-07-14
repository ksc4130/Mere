﻿using System;
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
        private readonly IDataReader _dataReader;
        //private readonly MereTable _mereTable;
        private readonly bool _byName;
        //private readonly bool _dynamicReturn;

        private readonly List<MereColumn> _selectMereColumnsList; 

        public MereSqlDataReader(SqlCommand cmd)
        {
            _dataReader = cmd.ExecuteReader();
            //_mereTable = MereUtils.CacheCheck<T>();
            _selectMereColumnsList = MereUtils.CacheCheck<T>().SelectMereColumns.ToList();
        }

        public MereSqlDataReader(SqlCommand cmd, List<MereColumn> selectMereColumnsList)
        {
            _dataReader = cmd.ExecuteReader();
            _selectMereColumnsList = selectMereColumnsList;
        }

        public MereSqlDataReader(SqlCommand cmd, List<MereColumn> selectMereColumnsList, bool byName)
        {
            _byName = byName;
            _dataReader = cmd.ExecuteReader();
            _selectMereColumnsList = selectMereColumnsList;
        }

        public MereSqlDataReader(SqlCommand cmd, bool byName)
        {
            _dataReader = cmd.ExecuteReader();
            //_mereTable = MereUtils.CacheCheck<T>();
            _selectMereColumnsList = MereUtils.CacheCheck<T>().SelectMereColumns.ToList();
            _byName = byName;
        }

        public MereSqlDataReader(SqlDataReader reader)
        {
            _dataReader = reader;
            //_mereTable = MereUtils.CacheCheck<T>();
            _selectMereColumnsList = MereUtils.CacheCheck<T>().SelectMereColumns.ToList();
        }

        public MereSqlDataReader(SqlDataReader reader, List<MereColumn> selectMereColumnsList)
        {
            _dataReader = reader;
            _selectMereColumnsList = selectMereColumnsList;
        }

        public MereSqlDataReader(SqlDataReader reader, List<MereColumn> selectMereColumnsList, bool byName)
        {
            _byName = byName;
            _dataReader = reader;
            _selectMereColumnsList = selectMereColumnsList;
        }

        public MereSqlDataReader(IDataReader reader, List<MereColumn> selectMereColumnsList, bool byName)
        {
            _byName = byName;
            _dataReader = reader;
            _selectMereColumnsList = selectMereColumnsList;
        }

        public MereSqlDataReader(SqlDataReader reader, bool byName)
        {
            _dataReader = reader;
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
                        reader._selectMereColumnsList[i].SetBase(n, reader._dataReader[i] is DBNull ? null : reader._dataReader[i]);
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

                    mereColumn.SetBase(n, reader._dataReader[i] is DBNull ? null : reader._dataReader[i]);
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
                d.Add(name, reader._dataReader[i] is DBNull ? null : reader._dataReader[i]);
            }
            return n;
        }

        object IDataRecord.this[int i]
        {
            get { return _dataReader[i]; }
        }

        object IDataRecord.this[string name]
        {
            get { return _dataReader[name]; }
        }

        public bool Read()
        {
            return _dataReader.Read();
        }

        public Task<bool> ReadAsync()
        {
            var sqlDataReader = _dataReader as SqlDataReader;
            if(sqlDataReader == null)
                throw new Exception("read async is only available when using sql data reader");
            return sqlDataReader.ReadAsync();
        }

        public void Dispose()
        {
            _dataReader.Dispose();
        }

        public string GetName(int i)
        {
            return _dataReader.GetName(i);
        }

        public string GetDataTypeName(int i)
        {
            return _dataReader.GetDataTypeName(i);
        }

        public Type GetFieldType(int i)
        {
            return _dataReader.GetFieldType(i);
        }

        public object GetValue(int i)
        {
            return _dataReader.GetValue(i);
        }

        public int GetValues(object[] values)
        {
            return _dataReader.GetValues(values);
        }

        public int GetOrdinal(string name)
        {
            return _dataReader.GetOrdinal(name);
        }

        public bool GetBoolean(int i)
        {
            return _dataReader.GetBoolean(i);
        }

        public byte GetByte(int i)
        {
            return _dataReader.GetByte(i);
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            return _dataReader.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
        }

        public char GetChar(int i)
        {
            return _dataReader.GetChar(i);
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            return _dataReader.GetChars(i, fieldoffset, buffer, bufferoffset, length);
        }

        public Guid GetGuid(int i)
        {
            return _dataReader.GetGuid(i);
        }

        public short GetInt16(int i)
        {
            return _dataReader.GetInt16(i);
        }

        public int GetInt32(int i)
        {
            return _dataReader.GetInt32(i);
        }

        public long GetInt64(int i)
        {
            return _dataReader.GetInt64(i);
        }

        public float GetFloat(int i)
        {
            return _dataReader.GetFloat(i);
        }

        public double GetDouble(int i)
        {
            return _dataReader.GetDouble(i);
        }

        public string GetString(int i)
        {
            return _dataReader.GetString(i);
        }

        public decimal GetDecimal(int i)
        {
            return _dataReader.GetDecimal(i);
        }

        public DateTime GetDateTime(int i)
        {
            return _dataReader.GetDateTime(i);
        }

        public IDataReader GetData(int i)
        {
            return _dataReader.GetData(i);
        }

        public bool IsDBNull(int i)
        {
            return _dataReader.IsDBNull(i);
        }

        private int _fieldCount;
        public int FieldCount { get { return _dataReader.FieldCount; }
            private set { _fieldCount = value; }
        }

        public void Close()
        {
            _dataReader.Close();
        }

        public DataTable GetSchemaTable()
        {
            return _dataReader.GetSchemaTable();
        }

        public bool NextResult()
        {
            return _dataReader.NextResult();
        }

        private int _depth;
        public int Depth { get { return _dataReader.Depth; } private set { _depth = value; } }
        private bool _isClosed;
        public bool IsClosed { get { return _dataReader.IsClosed; } private set { _isClosed = value; } }
        private int _recordsAffected;
        public int RecordsAffected { get { return _dataReader.RecordsAffected; } private set { _recordsAffected = value; } }
    }

}
