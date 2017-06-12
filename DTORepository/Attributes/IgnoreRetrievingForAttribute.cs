using DTORepository.Common;
using System;

namespace DTORepository.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class IgnoreRetrievingForAttribute : Attribute
    {

        public ActionFlags actions = ActionFlags.None;

        public IgnoreRetrievingForAttribute(ActionFlags actions)
        {
            this.actions = actions;
        }
    }
}
