using DTORepository.Common;
using System;

namespace DTORepository.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class IgnoreWritingForAttribute : Attribute
    {

        public ActionFlags actions = ActionFlags.None;

        public IgnoreWritingForAttribute(ActionFlags actions)
        {
            this.actions = actions;
        }
    }
}
