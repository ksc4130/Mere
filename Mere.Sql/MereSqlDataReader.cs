using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Mere.Core;

namespace Mere.Sql
{
    public class MereSqlDataReader<T> : MereDataReader<T> where T : new()
    {

        public MereSqlDataReader(IDbCommand cmd) : base(cmd)
        {
        }

        public MereSqlDataReader(IDbCommand cmd, List<MereColumn> selectMereColumnsList) : base(cmd, selectMereColumnsList)
        {
        }

        public MereSqlDataReader(IDbCommand cmd, List<MereColumn> selectMereColumnsList, bool byName) : base(cmd, selectMereColumnsList, byName)
        {
        }

        public MereSqlDataReader(SqlCommand cmd, bool byName) : base(cmd, byName)
        {
        }

        public MereSqlDataReader(IDataReader reader) : base(reader)
        {
        }

        public MereSqlDataReader(IDataReader reader, List<MereColumn> selectMereColumnsList) : base(reader, selectMereColumnsList)
        {
        }

        public MereSqlDataReader(IDataReader reader, List<MereColumn> selectMereColumnsList, bool byName) : base(reader, selectMereColumnsList, byName)
        {
        }

        public MereSqlDataReader(IDataReader reader, bool byName) : base(reader, byName)
        {
        }

        public override Task<bool> ReadAsync()
        {
            var sdr = DataReader as SqlDataReader;
            if (sdr == null)
                throw new Exception("IDataReader is not a SqlDataReader");
            return sdr.ReadAsync();
        }

    }
}