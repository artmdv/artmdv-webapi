using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gallery.Domain.ValueObjects.Gear
{
    public interface IGear
    {
        string Name { get; set; }
        string Manufacturer { get; set; }
        GearType Type { get; set; }
    }
}
