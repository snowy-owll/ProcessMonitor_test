using System;
using System.ComponentModel.DataAnnotations;

namespace ProcessMonitor
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class MinAttribute : DataTypeAttribute
    {   
        public MinAttribute(int min) : base("min")
        {
            _min = min;
        }

        public MinAttribute(double min) : base("min")
        {
            _min = min;
        }

        private readonly double _min;
        public object Min => _min;

        public override bool IsValid(object value)
        {
            if (value == null) return true;
            var isDouble = double.TryParse(Convert.ToString(value), out double valueAsDouble);
            return isDouble && valueAsDouble >= _min;
        }
    }
}
