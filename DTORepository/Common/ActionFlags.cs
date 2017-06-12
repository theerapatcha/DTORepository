using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTORepository.Common
{
    public enum ActionFlags
    {
        None = 0,
        Create = 1,
        Update = 2,
        Get = 4,
        List = 8,
        All = ActionFlags.Create | ActionFlags.Update | ActionFlags.Get | ActionFlags.List
    }
}
