using System.Data.SqlClient;
using System.Linq;
using Mere.Core;

namespace Mere.Sql
{
    public static class MereTableExt
    {
        public static SqlConnection GetConnection(this MereTable src)
        {
            return new SqlConnection(src.ConnectionString);
        }

        public static string SqlColumnNamesAll(this MereTable src)
        {
                return string.Join(", ", src.SelectMereColumns.Select(x => "[" + x.ColumnName + "]"));
        }

        public static string SqlColumnNamesNoIdentity(this MereTable src)
        {
            return string.Join(", ", src.SelectMereColumnsNoIdentity().Select(x => "[" + x.ColumnName + "]"));
        }
        public static string SqlColumnNamesNoIdentityForValues(this MereTable src)
        {
                return " @" + string.Join(", @", src.SelectMereColumnsNoIdentity().Select(x => x.ColumnName));
        }

        public static string SqlColumnNamesNoIdentityForUpdate(this MereTable src)
        {
                return string.Join(", ", src.SelectMereColumnsNoIdentity().Select(x => "[" + x.ColumnName + "]=@" + x.ColumnName));
        }

        public static string SqlUpdateWithoutKey(this MereTable src)
        {
            return "UPDATE " + src.TableName + " SET " + src.SqlColumnNamesNoIdentityForUpdate() + " "; 
        }

        public static string SqlInsert(this MereTable src)
        {
            return "INSERT INTO " + src.TableName + " (" + src.SqlColumnNamesNoIdentity() + ") VALUES(" + src.SqlColumnNamesNoIdentityForValues() + ")"; 
        }

        public static string SqlInsertValueSetPrefixedWithComma(this MereTable src)
        {
            return ", (" + src.SqlColumnNamesNoIdentityForValues() + ")";
        }

        public static string SqlInsertValueSetPrefixedWithComma(this MereTable src, string uniqueParamSuffix)
        {
            return ", (@" + string.Join(", @", src.SelectMereColumnsNoIdentity().Select(x => x.ColumnName + uniqueParamSuffix)) + ")";
        }

        public static string SqlInsertValueSetPrefixedWithComma(this MereTable src, int uniqueParamSuffix)
        {
            return ", (@" + string.Join(", @", src.SelectMereColumnsNoIdentity().Select(x => x.ColumnName + uniqueParamSuffix)) + ")";
        }
        public static string GetUpsertSqlWithKey(this MereTable src)
        {
            return @"IF (SELECT COUNT(0) FROM " + src.TableName + @" WHERE " + src.KeyColumnName + @"=@key) > 0
                     BEGIN "
                        + src.SqlUpdateWithoutKey() + @" WHERE " + src.KeyColumnName + @"=@key
                     END
                 ELSE
                     BEGIN "
                       + src.SqlInsert() +
                     "END";
        }

        public static string GetUpsertSqlWithCustomKey(this MereTable src, string columnName)
        {
            return @"IF (SELECT COUNT(0) FROM " + src.TableName + @" WHERE " + columnName + @"=@key) > 0
                     BEGIN "
                        + src.SqlUpdateWithoutKey() + @" WHERE " + columnName + @"=@key
                     END
                 ELSE
                     BEGIN "
                       + src.SqlInsert() +
                     "END";
        }
    }
}