using System;

namespace Gallery.Domain.ValueObjects
{
    public class Thumbnail
    {
        public Guid Id { get; set; }

        public ImageFile Image { get; set; }
    }
}
