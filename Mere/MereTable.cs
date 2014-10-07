//using System;
//using System.Collections.Generic;
//using System.Data.SqlClient;
//using System.Linq;
//using System.Linq.Expressions;
//using Mere.Attributes;
//
//namespace Mere
//{
//    public class MereTable<T> : MereTable where T : new()
//    {
//        public MereTable(Type tableClassType) : base(tableClassType)
//        {
//            
//        }
//        public virtual MereColumn GetMereColumn<TField>(Expression<Func<T, TField>> field)
//        {
//            var fName = field.GetPropName();
//            return PropertyHelpersNamed.FirstOrDefault(x => x.Key.ToLower() == fName.ToLower()).Value;
//        }
//        public virtual T CreateNewInstance<TField>(Expression<Func<T, TField>> field)
//        {
//            return new T();
//        }
//    }
//
//
//    public class MereTable
//    {
//        public MereTable(Type tableClassType)
//        {
//            TableClassType = tableClassType;
//            ConnectionStringBase = "Server={0};Database={1};User Id={2};Password={3};";
//            //"Data Source={0};Initial Catalog={1};User ID={2};Password={3};Asynchronous Processing=True;MultipleActiveResultSets=True;Timeout={4}";
//        }
//
//        //public virtual ITable TableInfo { get; set; }
//        public virtual bool IsView { get; set; }
//        public virtual SqlConnection GetConnection()
//        {
//            return new SqlConnection(ConnectionString);
//        }
//        public Type TableClassType { get; set; }
//        public virtual string TableName { get; set; }
//        public virtual string DatabaseName { get; set; }
//        public virtual string ServerName { get; set; }
//        public virtual string UserId { get; set; }
//        public virtual string Password { get; set; }
//        public virtual string KeyColumnName { get; set; }
//        public virtual string KeyPropertyName { get; set; }
//        public virtual int Timeout { get; set; }
//        public virtual bool DelimitedHasHeader { get; set; }
//        public virtual string Delimiter { get; set; }
//
//        public MereFlatFileTableAttribute FlatFileTableAttr { get; set; }
//        //public MereDelimitedFileTableAttribute DelimitedTableAttr { get; set; }
//
//        public string ConnectionStringBase { get; set; }
//        public string ConnectionString
//        {
//            get
//            {
//                return string.Format(ConnectionStringBase, ServerName, DatabaseName,
//                                     UserId, Password, Timeout);
//            }
//        }
//
//        public virtual Dictionary<int, MereColumn> PropertyHelpersIndexed { get; set; }
//        public virtual Dictionary<string, MereColumn> PropertyHelpersNamed { get; set; }
//        public virtual Dictionary<string, MereColumn> PropertyHelpersColumnName { get; set; }
//
//        public virtual Dictionary<int, MereColumn> PropertyHelpersFlatFileFieldsIndexed { get; set; }
//        public virtual Dictionary<int, MereColumn> PropertyHelpersDelimitedFieldsIndexed { get; set; }
//
//        private IEnumerable<MereColumn> _delimitedFileFields;
//        public virtual IEnumerable<MereColumn> DelimitedFields
//        {
//            get
//            {
//                if (_delimitedFileFields == null)
//                {
//                    var found = PropertyHelpersIndexed.Values.Where(
//                        x => x.DelimitedFieldAttrs != null && x.DelimitedFieldAttrs.Any())
//                        .SelectMany(x => x.DelimitedFieldAttrs.Select(d =>
//                        {
//                            var tr = x.CloneToMereColumn();
//                            tr.DelimitedFieldAttr = d;
//                            return tr;
//                        })).OrderBy(x => x.DelimitedFieldAttr.Index);
//                    _delimitedFileFields = found;
//                }
//
//                return _delimitedFileFields;
//            }
//            set { _delimitedFileFields = value; }
//        }
//
//        private IEnumerable<MereColumn> _flatFileFields;
//        public virtual IEnumerable<MereColumn> FlatFileFields
//        {
//            get
//            {
//                return _flatFileFields ?? PropertyHelpersIndexed.Values.Where(x => x.FlatFileFieldAttr != null);
//            }
//            set { _flatFileFields = value; }
//        }
//        
////        public virtual IEnumerable<MereColumn> DelimitedFields { get; set; }
////        public virtual IEnumerable<MereColumn> FlatFileFields { get; set; }
//
//        public virtual MereColumn GetMereColumnByColumnName(string columnName)
//        {
//            return
//                PropertyHelpersIndexed.Values.FirstOrDefault(
//                    x => string.Compare(columnName, x.ColumnName, StringComparison.CurrentCultureIgnoreCase) == 0);
//        }
//
//        public virtual MereColumn GetMereColumnByPropertyName(string propertyName)
//        {
//            return
//                PropertyHelpersIndexed.Values.FirstOrDefault(
//                    x => propertyName == x.PropertyName);
//        }
//
//        public virtual MereColumn GetMereColumn(string name)
//        {
//            return
//                PropertyHelpersIndexed.Values.FirstOrDefault(
//                    x => (string.Compare(name, x.ColumnName, StringComparison.CurrentCultureIgnoreCase) == 0) ||
//                        (string.Compare(name, x.PropertyName, StringComparison.CurrentCultureIgnoreCase) == 0));
//        }
//
//        public virtual string SelectPropertiesAll
//        {
//            get { return SelectPropertiesAllList != null ? string.Join(",", SelectPropertiesAllList) : null; }
//            set
//            {
//                if(value != null)
//                    SelectPropertiesAllList = value.Split(',').ToList();
//                else
//                {
//                    SelectPropertiesAllList = null;
//                }
//            } }
//        public virtual string SelectPropertiesNoIdentity
//        {
//            get { return SelectPropertiesNoIdentityList != null ? string.Join(",", SelectPropertiesNoIdentityList) : null; }
//            set
//            {
//                if(value != null)
//                    SelectPropertiesNoIdentityList = value.Split(',').ToList();
//                else
//                {
//                    SelectPropertiesNoIdentityList = null;
//                }
//            } }
//        public virtual string SelectPropertiesNoKey
//        {
//            get { return SelectPropertiesNoKeyList != null ? string.Join(",", SelectPropertiesNoKeyList) : null; }
//            set
//            {
//                if(value != null)
//                    SelectPropertiesNoKeyList = value.Split(',').ToList();
//                else
//                {
//                    SelectPropertiesNoKeyList = null;
//                }
//            } }
//        public virtual string SelectPropertiesNoIdentityOrKey
//        {
//            get { return SelectPropertiesNoIdentityOrKeyList != null ? string.Join(",", SelectPropertiesNoIdentityOrKeyList) : null; }
//            set
//            {
//                if(value != null)
//                    SelectPropertiesNoIdentityOrKeyList = value.Split(',').ToList();
//                else
//                {
//                    SelectPropertiesNoIdentityOrKeyList = null;
//                }
//            } }
//
//        public virtual string AtPropNamesNoIdentity
//        {
//            get { return PropertyNamesNoIdentityList != null ? "@" + string.Join(",@", PropertyNamesNoIdentityList) : null; }
//            set
//            {
//                if (value != null)
//                    PropertyNamesNoIdentityList = value.Split(",@".ToCharArray()).ToList();
//                else
//                {
//                    PropertyNamesNoIdentityList = null;
//                }
//            }
//        }
//
//        public string UpdateStr 
//        { 
//            get
//            {
//                var toReturn = "";
//                var i = 0;
//                foreach (var str in PropertyNamesNoIdentityList)
//                {
//                    toReturn += i > 0 ? ",[" + str + "]=@" + str : "[" + str + "]=@" + str;
//                    i++;
//                }
//                return toReturn;
//            }
//        }
//
//        public virtual IList<string> SelectPropertiesAllList { get; set; }
//        public virtual IList<string> SelectPropertiesNoIdentityList { get; set; }
//        public virtual IList<string> SelectPropertiesNoKeyList { get; set; }
//        public virtual IList<string> SelectPropertiesNoIdentityOrKeyList { get; set; }
//
//        public virtual IList<string> PropertyNamesAllList { get; set; }
//        public virtual IList<string> PropertyNamesNoIdentityList { get; set; }
//        public virtual IList<string> PropertyNamesNoKeyList { get; set; }
//        public virtual IList<string> PropertyNamesNoIdentityOrKeyList { get; set; }
//
//        public virtual string GetUpsertSqlWithKey()
//        {
//            return @"IF (SELECT COUNT(0) FROM " + TableName + @" WHERE " + KeyColumnName + @"=@key) > 0
//                     BEGIN
//                        UPDATE " + TableName + " SET " + UpdateStr + @" WHERE " + KeyColumnName + @"=@key
//                     END
//                 ELSE
//                     BEGIN
//                        INSERT INTO " + TableName + " (" + SelectPropertiesNoIdentity + @") VALUES(" + AtPropNamesNoIdentity + @")
//                     END";
//        }
//
//        public virtual string GetUpsertSqlWithCustomKey(string columnName)
//        {
//            return @"IF (SELECT COUNT(0) FROM " + TableName + @" WHERE " + columnName + @"=@key) > 0
//                     BEGIN
//                        UPDATE " + TableName + " SET " + UpdateStr + @" WHERE " + columnName + @"=@key
//                     END
//                 ELSE
//                     BEGIN
//                        INSERT INTO " + TableName + " (" + SelectPropertiesNoIdentity + @") VALUES(" + AtPropNamesNoIdentity + @")
//                     END";
//        }
//    }
//}
