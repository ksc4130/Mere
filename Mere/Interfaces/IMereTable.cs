using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mere.Interfaces
{
    interface IMereTable
    {
        MereColumn this[string columnName] { get; set; }

        int Timeout { get; set; }
        string TableName { get; set; }
        string DatabaseName { get; set; }
        string ServerName { get; set; }
        string UserId { get; set; }
        string Password { get; set; }
        string KeyColumnName { get; set; }
        string KeyPropertyName { get; set; }
        bool IsView { get; set; }
        IDbConnection GetConnection();

        string ConnectionStringBase { get; set; }
        string ConnectionString { get; }

        MereColumn GetMereColumnByColumnName(string columnName);
        MereColumn GetMereColumnByPropertyName(string propertyName);

        MereColumn GetMereColumn(string name);

        #region SelectMereColumns
        MereColumnList AllMereColumns { get; set; }

        MereColumnList SelectMereColumns { get; set; }

        #region no identity

        IEnumerable<MereColumn> SelectMereColumnsNoIdentity();

        IEnumerable<MereColumn> SelectMereColumnsNoIdentity(IEnumerable<MereColumn> selectMereColumns);

        IEnumerable<MereColumn> SelectMereColumnsNoIdentity(IList<MereColumn> selectMereColumns);
        #endregion

        #region no key

        IEnumerable<MereColumn> SelectMereColumnsNoKey();

        IEnumerable<MereColumn> SelectMereColumnsNoKey(IEnumerable<MereColumn> selectMereColumns);

        IEnumerable<MereColumn> SelectMereColumnsNoKey(IList<MereColumn> selectMereColumns);
        #endregion

        #region no identity or key

        IEnumerable<MereColumn> SelectMereColumnsNoIdentityOrKey();

        IEnumerable<MereColumn> SelectMereColumnsNoIdentityOrKey(IEnumerable<MereColumn> selectMereColumns);

        IEnumerable<MereColumn> SelectMereColumnsNoIdentityOrKey(IList<MereColumn> selectMereColumns);
        #endregion


        #endregion

        #region sql starters
        string SqlColumnNamesAll { get; }

        string SqlColumnNamesNoIdentity { get; }

        string SqlColumnNamesNoIdentityForValues { get; }

        string SqlColumnNamesNoIdentityForUpdate { get; }

        string SqlUpdateWithoutKey { get; }

        string SqlInsert { get; }

        string SqlInsertValueSetPrefixedWithComma();

        string SqlInsertValueSetPrefixedWithComma(string uniqueParamSuffix);

        string SqlInsertValueSetPrefixedWithComma(int uniqueParamSuffix);

        string GetUpsertSqlWithKey();

        string GetUpsertSqlWithCustomKey(string columnName);

        #endregion
    }
}
