using System;
using System.Collections.Generic;
using System.ComponentModel;
using Mere.Attributes;

namespace Mere
{
    #region MereColumn<T> idea
    //public class MereColumn<T>
    //{
    //    public bool CleanupConversion { get; set; }

    //    public MereColumn(PropertyDescriptor propertyDescriptor)
    //    {

    //        _propertyDescriptor = propertyDescriptor;
    //        //add set methods to property helper
    //        Action<object, object> setAct;

    //        //TODO: Clean up set
    //        if (propertyDescriptor.PropertyType == typeof(DateTime))
    //        {
    //            //TODO: default to 1/1/1900
    //            setAct = (parent, val) =>
    //            {
    //                DateTime d;
    //                if (!CleanupConversion)
    //                    d = DateTime.Parse(val.ToString());
    //                else
    //                {
    //                    if (val == null || !DateTime.TryParse(val.ToString(), out d))
    //                        d = DateTime.Parse("01/01/1900");

    //                    if (d.Year < 1900)
    //                        d = DateTime.Parse("01/01/1900");
    //                }

    //                propertyDescriptor.SetValue(parent, d);
    //            };
    //        }
    //        else if (propertyDescriptor.PropertyType == typeof(DateTime?))
    //        {
    //            setAct = (parent, val) =>
    //            {
    //                DateTime? d = null;
    //                if (!CleanupConversion)
    //                    d = (DateTime?)val;
    //                else
    //                {
    //                    DateTime test;
    //                    if (DateTime.TryParse(val == null ? "0" : val.ToString(), out test))
    //                    {
    //                        if (test.Year < 1900)
    //                            test = DateTime.Parse("01/01/1900");

    //                        d = test;
    //                    }
    //                    else
    //                    {
    //                        d = null;
    //                    }
    //                }

    //                // ReSharper disable AssignNullToNotNullAttribute
    //                propertyDescriptor.SetValue(parent, d);
    //                // ReSharper restore AssignNullToNotNullAttribute
    //            };
    //        }
    //        else if (propertyDescriptor.PropertyType == typeof(int))
    //        {
    //            setAct = (parent, val) =>
    //            {
    //                int d;
    //                if (!CleanupConversion)
    //                    d = int.Parse(val.ToString());
    //                else
    //                    int.TryParse(val == null ? "0" : val.ToString(), out d);

    //                propertyDescriptor.SetValue(parent, d);
    //            };
    //        }
    //        else if (propertyDescriptor.PropertyType == typeof(int?))
    //        {
    //            setAct = (parent, val) =>
    //            {
    //                int? d;
    //                if (!CleanupConversion)
    //                    d = (int?)val;
    //                else
    //                {
    //                    int test;
    //                    if (int.TryParse(val == null ? "0" : val.ToString(), out test))
    //                    {
    //                        d = test;
    //                    }
    //                    else
    //                    {
    //                        d = null;
    //                    }
    //                }

    //                // ReSharper disable AssignNullToNotNullAttribute
    //                propertyDescriptor.SetValue(parent, d);
    //                // ReSharper restore AssignNullToNotNullAttribute
    //            };
    //        }
    //        else if (propertyDescriptor.PropertyType == typeof(long))
    //        {
    //            setAct = (parent, val) =>
    //            {
    //                long d;
    //                if (!CleanupConversion)
    //                    d = long.Parse(val.ToString());
    //                else
    //                    long.TryParse(val == null ? "0" : val.ToString(), out d);

    //                propertyDescriptor.SetValue(parent, d);
    //            };
    //        }
    //        else if (propertyDescriptor.PropertyType == typeof(long?))
    //        {
    //            setAct = (parent, val) =>
    //            {
    //                long? d;
    //                if (!CleanupConversion)
    //                    d = (long?)val;
    //                else
    //                {
    //                    long test;
    //                    if (long.TryParse(val == null ? "0" : val.ToString(), out test))
    //                    {
    //                        d = test;
    //                    }
    //                    else
    //                    {
    //                        d = null;
    //                    }
    //                }

    //                propertyDescriptor.SetValue(parent,
    //                                                                  d == null ? "" : d.ToString());
    //            };
    //        }
    //        else if (propertyDescriptor.PropertyType == typeof(decimal))
    //        {
    //            setAct = (parent, val) =>
    //            {
    //                decimal d;
    //                if (!CleanupConversion)
    //                    d = decimal.Parse(val.ToString());
    //                else
    //                    decimal.TryParse(val == null ? "0" : val.ToString(), out d);


    //                propertyDescriptor.SetValue(parent, d);
    //            };
    //        }
    //        else if (propertyDescriptor.PropertyType == typeof(decimal?))
    //        {
    //            setAct = (parent, val) =>
    //            {
    //                decimal? d;
    //                if (!CleanupConversion)
    //                    d = (decimal?)val;
    //                else
    //                {
    //                    decimal test;
    //                    if (decimal.TryParse(val == null ? "0" : val.ToString(), out test))
    //                    {
    //                        d = test;
    //                    }
    //                    else
    //                    {
    //                        d = null;
    //                    }
    //                }

    //                // ReSharper disable AssignNullToNotNullAttribute
    //                propertyDescriptor.SetValue(parent, d);
    //                // ReSharper restore AssignNullToNotNullAttribute
    //            };
    //        }
    //        else
    //        {
    //            setAct = propertyDescriptor.SetValue;
    //        }

    //        _setOld = setAct;
    //    }

    //    private PropertyDescriptor _propertyDescriptor;
    //    public virtual PropertyDescriptor PropertyDescriptor { get { return _propertyDescriptor; } set { _propertyDescriptor = value; } }
    //    public virtual string PropertyName { get; set; }
    //    public virtual string ColumnName { get; set; }
    //    public virtual string DisplayName { get; set; }
    //    public virtual bool IsKey { get; set; }
    //    public virtual bool IsNullable { get; set; }
    //    public virtual bool IsIdentity { get; set; }
    //    public virtual int? MaxLength { get; set; }

    //    /// <summary>
    //    /// action for settings value in object
    //    /// first param is parent object 
    //    /// second param is value to set field to 
    //    /// </summary>
    //    public virtual void Set(object instance, T value)
    //    {
    //        PropertyDescriptor.SetValue(instance, value);
    //    }

    //    private Action<object, object> _setOld;

    //    /// <summary>
    //    /// action for settings value in object
    //    /// first param is parent object 
    //    /// second param is value to set field to 
    //    /// </summary>
    //    public virtual Action<object, object> SetOld { get { return _setOld; } set { _setOld = value; } }

    //    /// <summary>
    //    /// func for getting value from object
    //    /// param is parent object holding field in which value you desire
    //    /// returns object to be casted
    //    /// </summary>
    //    public virtual Func<object, object> Get { get; set; }
    //}
    #endregion

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
                {return val.ToString().Substring(0, (int)MaxLength);}
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
                {val = value.ToString().Substring(0, (int)MaxLength);}
            }
            SetBase(parent, val);
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public MereColumn CloneToMereColumn()
        {
            return (MereColumn) Clone();
        }
    }
}
