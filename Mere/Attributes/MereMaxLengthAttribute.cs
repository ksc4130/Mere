﻿using System;

namespace Mere.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class MereMaxLengthAttribute : Attribute
    {
        public int MaxLength { get; set; }
        public MereMaxLengthAttribute(int maxLength)
        {
            MaxLength = maxLength;
        }
    }
}
