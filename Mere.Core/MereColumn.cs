using System;
using System.ComponentModel;

namespace Mere.Core
{
    public class MereColumn : ICloneable
    {
        public virtual PropertyDescriptor PropertyDescriptor { get; set; }
        public virtual string PropertyName { get; set; }
        public virtual string ColumnName { get; set; }
        public virtual string DisplayName { get; set; }
        public virtual bool IsKey { get; set; }
        public virtual bool IsNullable { get; set; }
        public virtual bool IsIdentity { get; set; }
        public virtual bool IsColumn { get; set; }
        public virtual int? MaxLength { get; set; }

        /// <summary>
        /// action for settings value in object
        /// first param is parent object 
        /// second param is value to set field to 
        /// </summary>
        public virtual Action<object, object> SetBase { get; set; }
        /// <summary>
        /// func for getting value from object
        /// param is parent object holding field in which value you desire
        /// returns object to be casted
        /// </summary>
        public virtual Func<object, object> GetBase { get; set; }

        public virtual object Get(object parent)
        {
            return Get(parent, false);
        }

        public virtual object Get(object parent, bool cleanUp)
        {
            var val = GetBase(parent);
            if (cleanUp)
            {
                if (val != null && val is string && MaxLength != null &&
                    val.ToString().Length > MaxLength)
                { return val.ToString().Substring(0, (int)MaxLength); }
            }
            return val;
        }

        public virtual void Set(object parent, object value)
        {
            Set(parent, value, false);
        }

        public virtual void Set(object parent, object value, bool cleanUp)
        {
            var val = value;
            if (cleanUp)
            {
                if (val != null && val is string && MaxLength != null &&
                    value.ToString().Length > MaxLength)
                { val = value.ToString().Substring(0, (int)MaxLength); }
            }
            SetBase(parent, val);
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public MereColumn CloneToMereColumn()
        {
            return (MereColumn)Clone();
        }
    }
}