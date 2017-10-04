using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using artmdv_webapi.Areas.v2.Models;
using Microsoft.Net.Http.Headers;

namespace artmdv_webapi.Areas.v2.Command
{
    public class UploadImageCommand : ICommand
    {
        public Stream File{ get; set; }
        public Image Image { get; set; }
        

        public UploadImageCommand(ImageUploadDto model)
        {
            if (model?.file?.Length > 0)
            {
                var filename = ContentDispositionHeaderValue
                    .Parse(model.file.ContentDisposition)
                    .FileName
                    .ToString().Trim('"');

                File = new MemoryStream();

                var fileStream = model.file.OpenReadStream();
                fileStream.CopyTo(File);
                File.Position = 0;
                fileStream.Dispose();

                Image = new Image
                {
                    Filename = filename,
                    Description = model.description,
                    Title = model.title,
                    Tags = model.tags.Split(','),
                    Date = model.date,
                    Thumb = new Thumbnail
                    {
                        Filename = $"thumb_{filename}"
                    },
                    Annotation = model.annotation,
                    Inverted = model.inverted
                };
            }
        }
    }
}
