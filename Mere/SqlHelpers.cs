using System.Collections.Generic;

namespace Mere
{
    public class SqllErrorNumbers
    {
        public const int BadObject = 208;
        public const int DupKey = 2627;
    }

    public class SqlDataTypeOperator
    {
        public List<string> DataTypes { get; set; }
        public List<string> OperatorOptions { get; set; }
    }

    public class SqlOperatorConverter
    {
        public List<string> English { get; set; }
        public List<string> Operators { get; set; }
        public SqlOperator SqlOperator { get; set; }

        public static implicit operator string(SqlOperatorConverter src)
        {
            return SqlOperatorHelper.GetString(src.SqlOperator);
        }

        public static implicit operator SqlOperator(SqlOperatorConverter src)
        {
            return src.SqlOperator;
        }
    }

    public struct SqlOperatorStruct
    {
        #region merejs
        //    var sqlOperators = [
        //    {
        //        english: [
        //            'Equal To',
        //            'eq',
        //            'Equals'
        //        ],
        //        operators: [
        //            '=',
        //            '==',
        //            '==='
        //        ],
        //        sqlOperator: '='
        //    },
        //    {
        //        english: [
        //            'Not Equal To'
        //        ],
        //        operators: [
        //            '!=',
        //            '!==',
        //            '<>'
        //        ],
        //        sqlOperator: '<>'
        //    },
        //    {
        //        english: [
        //            'Greater Than',
        //            'gt'
        //        ],
        //        operators: [
        //            '>'
        //        ],
        //        sqlOperator: '>'
        //    },
        //    {
        //        english: [
        //            'Greater Than Or Equal To'
        //        ],
        //        operators: [
        //            '>='
        //        ],
        //        sqlOperator: '>='
        //    },
        //    {
        //        english: [
        //            'Less Than',
        //            'lt'
        //        ],
        //        operators: [
        //            '<'
        //        ],
        //        sqlOperator: '<'
        //    },
        //    {
        //        english: [
        //            'Less Than Or Equal To'
        //        ],
        //        operators: [
        //            '<='
        //        ],
        //        sqlOperator: '<='
        //    },
        //    {
        //        english: [
        //            'Between'
        //        ],
        //        operators: [
        //            'BETWEEN'
        //        ],
        //        sqlOperator: 'BETWEEN'
        //    },
        //    {
        //        english: [
        //            'In'
        //        ],
        //        operators: [
        //            'IN'
        //        ],
        //        sqlOperator: 'IN'
        //    },
        //    {
        //        english: [
        //            'Not In'
        //        ],
        //        operators: [
        //            'NOT IN'
        //        ],
        //        sqlOperator: 'NOT IN'
        //    },
        //    {
        //        english: [
        //            'Starts With'
        //        ],
        //        operators: [
        //            'startsWith'
        //        ],
        //        sqlOperator: 'startsWith'
        //    },
        //    {
        //        english: [
        //            'End With'
        //        ],
        //        operators: [
        //            'endsWith'
        //        ],
        //        sqlOperator: 'endsWith'
        //    },
        //    {
        //        english: [
        //            'Contains'
        //        ],
        //        operators: [
        //            'contains'
        //        ],
        //        sqlOperator: 'contains'
        //    }
        //];

        //var sqlDataTypeOperatorsEnglish = [
        //{
        //    dataTypes: [
        //        'money',
        //        'smallmoney',//***??
        //        'decimal',
        //        'int',
        //        'tinyint',
        //        'smallint',
        //        'bigint',//***??
        //        'numeric',
        //        'float',
        //        'datetime',
        //        'smalldatetime',
        //        'date',
        //        'datetime2',//***??
        //        'time',//***??
        //        'timestamp'//***??
        //    ],
        //    operatorOptions: [
        //        'Equal To',
        //        'Not Equal To',
        //        'Greater Than',
        //        'Greater Than Or Equal To',
        //        'Less Than',
        //        'Less Than Or Equal To',
        //        'Between',
        //        'In',
        //        'Not In'
        //    ]
        //},
        //{
        //    dataTypes: [
        //        'varbinary',
        //        'varchar',
        //        'nchar',
        //        'char',
        //        'nvarchar',
        //        'text',//***??
        //        'uniqueidentifier'//***??
        //    ],
        //    operatorOptions: [
        //        'Equal To',
        //        'Not Equal To',
        //        'Starts With',
        //        'Ends With',
        //        'Contains',
        //        'In',
        //        'Not In'
        //    ]
        //},
        //{
        //    dataTypes: [
        //        'bit',
        //        'bool'
        //    ],
        //    operatorOptions: [
        //        'Equal To',
        //        ' Not Equal To '
        //    ]
        //}

        //];

        //var sqlDataTypeOperators = [
        //{
        //    dataTypes: [
        //        'money',
        //        'smallmoney',//***??
        //        'decimal',
        //        'int',
        //        'tinyint',
        //        'smallint',
        //        'bigint',//***??
        //        'numeric',
        //        'float',
        //        'datetime',
        //        'smalldatetime',
        //        'date',
        //        'datetime2',//***??
        //        'time',//***??
        //        'timestamp'//***??
        //    ],
        //    operatorOptions: [
        //        ' = ',
        //        ' Not Equal To ',
        //        ' > ',
        //        ' >= ',
        //        ' < ',
        //        ' <= ',
        //        'Between',
        //        'In',
        //        'Not In'
        //    ]
        //},
        //{
        //    dataTypes: [
        //        'varbinary',
        //        'varchar',
        //        'nchar',
        //        'char',
        //        'nvarchar',
        //        'text',//***??
        //        'uniqueidentifier'//***??
        //    ],
        //    operatorOptions: [
        //        ' = ',
        //        ' Not Equal To ',
        //        'Starts With',
        //        'Ends With',
        //        'Contains',
        //        'In',
        //        'Not In'
        //    ]
        //},
        //{
        //    dataTypes: [
        //        'bit',
        //        'bool'
        //    ],
        //    operatorOptions: [
        //        ' = ',
        //        ' Not Equal To '
        //    ]
        //}

        //];
        #endregion
        private string InternalValue { get; set; }

        public static readonly string EqualTo = "=";
        public static readonly string NotEqualTo = "<>";
        public static readonly string GreaterThan = ">";
        public static readonly string GreaterThanOrEqualTo = ">=";
        public static readonly string LessThan = "<";
        public static readonly string LessThanOrEqualTo = "<=";
        public static readonly string Like = "LIKE";
        public static readonly string NotLike = "LIKE";
        //public static readonly string Between = "BETWEEN";
        
        public static string StartWith(string value)
        {
            return string.Format("{0} '%{1}'", Like, value);
        }
        public static string NotStartWith(string value)
        {
            return string.Format("{0} '%{1}'", NotLike, value);
        }
        
        public static string Between(string val1, string val2)
        {
            return string.Format("BETWEEN {0} AND {1}", val1, val2);
        }

        public static string NotBetween(string val1, string val2)
        {
            return string.Format("NOT BETWEEN {0} AND {1}", val1, val2);
        }

        public bool Equals(SqlOperatorStruct other)
        {
            return string.Equals(InternalValue, other.InternalValue);
        }

        public override int GetHashCode()
        {
            return (InternalValue != null ? InternalValue.GetHashCode() : 0);
        }

        public override bool Equals(object obj)
        {
            var otherObj = (SqlOperatorStruct)obj;
            return otherObj.InternalValue.Equals(this.InternalValue);
        }

        public static bool operator ==(SqlOperatorStruct left, SqlOperatorStruct right)
        {
            return (left.InternalValue == right.InternalValue);
        }

        public static bool operator !=(SqlOperatorStruct left, SqlOperatorStruct right)
        {
            return (left.InternalValue != right.InternalValue);
        }

        public static implicit operator SqlOperatorStruct(string otherType)
        {
            return new SqlOperatorStruct
            {
                InternalValue = otherType
            };
        }
    }
    public enum SqlOperator
    {
        EqualTo,
        GreaterThan,
        GreaterThanOrEqualTo,
        LessThan,
        LessThanOrEqualTo,
        Between,
        NotBetween,
        NotEqualTo,
        Like,
        NotLike,
        EqualToCaseSensitive,
        NotEqualToCaseSensitive,
        LikeCaseSensitive,
        NotLikeCaseSensitive
    }

    public static class SqlOperatorHelper
    {
        public static string GetString(SqlOperator sqlOperator)
        {
            switch (sqlOperator)
            {
                case SqlOperator.EqualTo:
                    return "=";
                case SqlOperator.NotEqualTo:
                    return "<>";
                case SqlOperator.GreaterThan:
                    return ">";
                case SqlOperator.GreaterThanOrEqualTo:
                    return ">=";
                case SqlOperator.LessThan:
                    return "<";
                case SqlOperator.LessThanOrEqualTo:
                    return "<=";
                case SqlOperator.Like:
                    return " LIKE ";
                case SqlOperator.NotLike:
                    return " NOT LIKE ";
                case SqlOperator.EqualToCaseSensitive:
                    return " COLLATE Latin1_General_CS_AS = ";
                case SqlOperator.NotEqualToCaseSensitive:
                    return " COLLATE Latin1_General_CS_AS <> ";
                case SqlOperator.LikeCaseSensitive:
                    return " COLLATE Latin1_General_CS_AS LIKE ";
                case SqlOperator.NotLikeCaseSensitive:
                    return " COLLATE Latin1_General_CS_AS NOT LIKE ";
                default:
                    return " COLLATE Latin1_General_CS_AS = ";
            }
        }
    }
}
