using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;

namespace Mere.Core
{
    public class MereTable<T> : MereTable where T : new()
    {
        public virtual MereColumn GetMereColumn<TField>(Expression<Func<T, TField>> field)
        {
            var fName = field.GetPropName();
            return GetMereColumnByPropertyName(fName);
        }
        public virtual T CreateNewInstance<TField>(Expression<Func<T, TField>> field)
        {
            return new T();
        }
    }

    public class MereTable
    {
        public MereTable()
        {
            ConnectionStringBase = "Server={0};Database={1};User Id={2};Password={3};";
        }

        public virtual int Timeout { get; set; }
        public virtual string TableName { get; set; }
        public virtual string DatabaseName { get; set; }
        public virtual string ServerName { get; set; }
        public virtual string UserId { get; set; }
        public virtual string Password { get; set; }
        public virtual string KeyColumnName { get; set; }
        public virtual string KeyPropertyName { get; set; }
        public virtual bool IsView { get; set; }

        //moved to ext
//        public virtual SqlConnection GetConnection()
//        {
//            return new SqlConnection(ConnectionString);
//        }

        public string ConnectionStringBase { get; set; }
        public string ConnectionString
        {
            get
            {
                return string.Format(ConnectionStringBase, ServerName, DatabaseName,
                                     UserId, Password, Timeout);
            }
        }

        public virtual MereColumn GetMereColumnByColumnName(string columnName)
        {
            return SelectMereColumns.GetMereColumnByColumnName(columnName);
        }

        public virtual MereColumn GetMereColumnByPropertyName(string propertyName)
        {
            return
                SelectMereColumns.GetMereColumnByPropertyName(propertyName);
        }

        public virtual MereColumn GetMereColumn(string name)
        {
            return SelectMereColumns.GetMereColumn(name);
        }

        #region SelectMereColumns

        public virtual MereColumnList AllMereColumns { get; set; }

        private MereColumnList _selectMereColumns;
        public virtual MereColumnList SelectMereColumns
        {
            get { return _selectMereColumns; }
            set
            {
                _selectMereColumnsNoIdentity = null;
                _selectMereColumnsNoKey = null;
                _selectMereColumns = value;
            }
        }

        #region caching
        private IList<MereColumn> _selectMereColumnsNoIdentity;
        private IList<MereColumn> _selectMereColumnsNoKey;
        private IList<MereColumn> _selectMereColumnsNoIdentityOrKey;
        #endregion

        #region no identity
        public virtual IEnumerable<MereColumn> SelectMereColumnsNoIdentity()
        {
            return _selectMereColumnsNoIdentity ??
                   (_selectMereColumnsNoIdentity = SelectMereColumns.Where(x => !x.IsIdentity).ToList());
        }

        public virtual IEnumerable<MereColumn> SelectMereColumnsNoIdentity(IEnumerable<MereColumn> selectMereColumns)
        {
            return selectMereColumns.Where(x => !x.IsIdentity);
        }
        public virtual IEnumerable<MereColumn> SelectMereColumnsNoIdentity(IList<MereColumn> selectMereColumns)
        {
            return selectMereColumns.Where(x => !x.IsIdentity);
        }
        #endregion

        #region no key
        public virtual IEnumerable<MereColumn> SelectMereColumnsNoKey()
        {
            return _selectMereColumnsNoKey ??
                   (_selectMereColumnsNoKey = SelectMereColumns.Where(x => !x.IsKey).ToList());
        }
        public virtual IEnumerable<MereColumn> SelectMereColumnsNoKey(IEnumerable<MereColumn> selectMereColumns)
        {
            return selectMereColumns.Where(x => !x.IsKey);
        }
        public virtual IEnumerable<MereColumn> SelectMereColumnsNoKey(IList<MereColumn> selectMereColumns)
        {
            return selectMereColumns.Where(x => !x.IsKey);
        }
        #endregion

        #region no identity or key
        public virtual IEnumerable<MereColumn> SelectMereColumnsNoIdentityOrKey()
        {
            return _selectMereColumnsNoIdentityOrKey ??
                   (_selectMereColumnsNoIdentityOrKey = SelectMereColumns.Where(x => !x.IsKey && !x.IsIdentity).ToList());
        }
        public virtual IEnumerable<MereColumn> SelectMereColumnsNoIdentityOrKey(IEnumerable<MereColumn> selectMereColumns)
        {
            return selectMereColumns.Where(x => !x.IsKey && !x.IsIdentity);
        }
        public virtual IEnumerable<MereColumn> SelectMereColumnsNoIdentityOrKey(IList<MereColumn> selectMereColumns)
        {
            return selectMereColumns.Where(x => !x.IsKey && !x.IsIdentity);
        }
        #endregion


        #endregion

        //moving to ext
        #region sql starters
//        public string SqlColumnNamesAll
//        {
//            get
//            {
//                return string.Join(", ", SelectMereColumns.Select(x => "[" + x.ColumnName + "]"));
//            }
//        }

//        public string SqlColumnNamesNoIdentity
//        {
//            get
//            {
//                return string.Join(", ", SelectMereColumnsNoIdentity().Select(x => "[" + x.ColumnName + "]"));
//            }
//        }

//        public string SqlColumnNamesNoIdentityForValues
//        {
//            get
//            {
//                return " @" + string.Join(", @", SelectMereColumnsNoIdentity().Select(x => x.ColumnName));
//            }
//        }

//        public string SqlColumnNamesNoIdentityForUpdate
//        {
//            get
//            {
//                return string.Join(", ",
//                                   SelectMereColumnsNoIdentity().Select(x => "[" + x.ColumnName + "]=@" + x.ColumnName));
//            }
//        }

//        public string SqlUpdateWithoutKey
//        {
//            get { return "UPDATE " + TableName + " SET " + SqlColumnNamesNoIdentityForUpdate + " "; }
//        }

//        public string SqlInsert
//        {
//            get { return "INSERT INTO " + TableName + " (" + SqlColumnNamesNoIdentity + ") VALUES(" + SqlColumnNamesNoIdentityForValues + ")"; }
//        }

//        public string SqlInsertValueSetPrefixedWithComma()
//        {
//            return ", (" + SqlColumnNamesNoIdentityForValues + ")";
//        }

//        public string SqlInsertValueSetPrefixedWithComma(string uniqueParamSuffix)
//        {
//            return ", (@" + string.Join(", @", SelectMereColumnsNoIdentity().Select(x => x.ColumnName + uniqueParamSuffix)) + ")";
//        }

//        public string SqlInsertValueSetPrefixedWithComma(int uniqueParamSuffix)
//        {
//            return ", (@" + string.Join(", @", SelectMereColumnsNoIdentity().Select(x => x.ColumnName + uniqueParamSuffix)) + ")";
//        }

//        public virtual string GetUpsertSqlWithKey()
//        {
//            return @"IF (SELECT COUNT(0) FROM " + TableName + @" WHERE " + KeyColumnName + @"=@key) > 0
//                     BEGIN "
//                        + SqlUpdateWithoutKey + @" WHERE " + KeyColumnName + @"=@key
//                     END
//                 ELSE
//                     BEGIN "
//                       + SqlInsert +
//                     "END";
//        }

//        public virtual string GetUpsertSqlWithCustomKey(string columnName)
//        {
//            return @"IF (SELECT COUNT(0) FROM " + TableName + @" WHERE " + columnName + @"=@key) > 0
//                     BEGIN "
//                        + SqlUpdateWithoutKey + @" WHERE " + columnName + @"=@key
//                     END
//                 ELSE
//                     BEGIN "
//                       + SqlInsert +
//                     "END";
//        }
        #endregion
    }
}