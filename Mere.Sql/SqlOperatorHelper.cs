using Mere.Core;

namespace Mere.Sql
{
    public static class SqlOperatorHelper
    {
        public static string GetString(MereOperator mereOperator)
        {
            switch (mereOperator)
            {
                case MereOperator.EqualTo:
                    return "=";
                case MereOperator.NotEqualTo:
                    return "<>";
                case MereOperator.GreaterThan:
                    return ">";
                case MereOperator.GreaterThanOrEqualTo:
                    return ">=";
                case MereOperator.LessThan:
                    return "<";
                case MereOperator.LessThanOrEqualTo:
                    return "<=";
                case MereOperator.Like:
                    return " LIKE ";
                case MereOperator.NotLike:
                    return " NOT LIKE ";
                case MereOperator.EqualToCaseSensitive:
                    return " COLLATE Latin1_General_CS_AS = ";
                case MereOperator.NotEqualToCaseSensitive:
                    return " COLLATE Latin1_General_CS_AS <> ";
                case MereOperator.LikeCaseSensitive:
                    return " COLLATE Latin1_General_CS_AS LIKE ";
                case MereOperator.NotLikeCaseSensitive:
                    return " COLLATE Latin1_General_CS_AS NOT LIKE ";
                default:
                    return " COLLATE Latin1_General_CS_AS = ";
            }
        }
    }
}