using Gallery.Domain.ValueObjects.Gear;

namespace Gallery.Domain.ValueObjects
{
    public class AstroImageDescription
    {
        public string Title { get; set; }
        public ImageType Type { get; set; }
        public Location[] Locations { get; set; }
        public string Description { get; set; }
        public IGear[] Gear { get; set; }
        public Frame[] Frames { get; set; }
    }
}
