namespace Mere
{
    public class MereFilter
    {
        public MereFilter(int filterLevel, string key, string value)
        {
            FilterLevel = filterLevel;
            Key = key;
            Value = value;
        }

        public int FilterLevel { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public SqlOperator Operator { get; set; }
    }
}
