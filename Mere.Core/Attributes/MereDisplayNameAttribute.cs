using System;

namespace Mere.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class MereDisplayNameAttribute : Attribute
    {
        public string DisplayName { get; set; }
        public MereDisplayNameAttribute(string displayName)
        {
            DisplayName = displayName;
        }
    }
}
