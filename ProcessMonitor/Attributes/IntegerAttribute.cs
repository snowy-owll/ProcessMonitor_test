using System;
using System.ComponentModel.DataAnnotations;

namespace ProcessMonitor
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class IntegerAttribute : DataTypeAttribute
    {
        public IntegerAttribute()
            :base("integer") { }

        public override bool IsValid(object value)
        {
            if (value == null) return false;
            return int.TryParse(Convert.ToString(value), out int _);
        }
    }
}
