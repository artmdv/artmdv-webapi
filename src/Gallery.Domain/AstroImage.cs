using System;
using System.Collections.Generic;
using Gallery.Domain.ValueObjects;

namespace Gallery.Domain
{
    public class AstroImage
    {
        public Guid Id { get; private set; }
        
        public AstroImageDescription Description { get; private set; }

        public IList<Tag> Tags { get; private set; }

        public Thumbnail Thumbnail { get; private set; }

        public ImageFile Image { get; private set; }

        public Annotation Annotation { get; private set; }

        public void Create(ImageFile image, AstroImageDescription description, Annotation annotation, Tag[] tags)
        {
            Id = Guid.NewGuid();
            Image = image;
            Description = description;
            Annotation = annotation;
            Tags = tags;

            Thumbnail = GenerateThumbnail();
        }

        private Thumbnail GenerateThumbnail()
        {
            throw new NotImplementedException();
        }

        public void AddTag(params Tag[] tag)
        {
            
        }
    }
}
