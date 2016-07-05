using System;

namespace Mere.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class MereColumnAttribute : Attribute
    {
        public string ColumnName { get; private set; }
        //TODO: fix this not a huge deal for now
        //removed table name until below is fixed
        //public string TableName { get; private set; }
        public string CompiledName
        {
            get
            {
                //TODO: fix this not a huge deal for now
                //removed table name column name concat per query parsing incorrectly 
                //return string.IsNullOrEmpty(TableName) ? string.IsNullOrEmpty(ColumnName) ? null : ColumnName : + TableName + "." + ColumnName;

                return ColumnName;
            }
        }

        public MereColumnAttribute()
        {

        }

        public MereColumnAttribute(string colName)
        {
            ColumnName = colName;
        }

        //TODO: fix this not a huge deal for now
        //removed constructor accepting table name until above is fixed
        //public MereColumnAttribute(string colName, string tableName)
        //{
        //    TableName = tableName;
        //    ColumnName = colName;
        //}
    }
}
