using System.IO;
using artmdv_webapi.Areas.v2.Models;

namespace artmdv_webapi.Areas.v2.Command
{
    public class UploadImageCommand : ICommand
    {
        public Stream File{ get; set; }
        public Image Image { get; set; }
        

        public UploadImageCommand(ImageUploadDto model, Stream file, string fileName)
        {
            if (file?.Length > 0)
            {
                File = new MemoryStream();
                
                file.CopyTo(File);
                File.Position = 0;
                file.Dispose();

                Image = new Image
                {
                    Filename = fileName,
                    Description = model.description,
                    Title = model.title,
                    Tags = model.tags?.Split(',') ?? new []{"default"},
                    Date = model.date,
                    Thumb = new Thumbnail
                    {
                        Filename = $"thumb_{fileName}"
                    },
                    Annotation = model.annotation,
                    Inverted = model.inverted
                };
            }
        }
    }
}
