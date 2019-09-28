using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHH.UI.Elements.Buttons
{
    [Flags]
    public enum Prerequisites
    {
        None = 1,
        Tier1 = 2,
        Tier2 = 4,
        Tier3 = 8,
        Tier4 = 16
    }
}
