using System;

namespace Mere.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class MereIdentityAttribute : Attribute
    {
        public int Seed { get; set; }
        public int Increment { get; set; }
        public MereIdentityAttribute()
        {

        }
        
        public MereIdentityAttribute(int seed, int increment)
        {
            Seed = seed;
            Increment = increment;
        }
    }
}
