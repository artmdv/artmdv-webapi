using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gallery.Domain.ValueObjects
{
    public class Annotation
    {
        public Guid ImageId { get; set; }

        public ImageFile Image { get; set; }
    }
}
