using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTORepository.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class IgnoreMappingIfValueAttribute : Attribute
    {
        public object IgnoredValue { get; }
        public IgnoreMappingIfValueAttribute(object IgnoredValue)
        {
            this.IgnoredValue = IgnoredValue;
        }
    }
}
