using System;
using Gallery.Contracts.Commands;

namespace Gallery.Domain
{
    public class Image
    {
        public Image()
        {
        }

        public Guid Id { get; private set; }

        public string Title { get; private set; }

        public string FileName { get; private set; }

        public string Description { get; private set; }

        public string[] Tags { get; private set; }

        public Guid Thumbnail { get; private set; }

        public Guid ImageFileId { get; private set; }

        public void Create(CreateImageCommand cmd)
        {
            Id = Guid.NewGuid();
            Title = cmd.Title;
            FileName = cmd.FileName;

        }
    }
}
