using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using artmdv_webapi.Areas.v2.Command;
using artmdv_webapi.Areas.v2.Models;
using Microsoft.Net.Http.Headers;

namespace artmdv_webapi.Areas.v2.Commands
{
    public class UploadImageRevisionCommand : ICommand
    {
        public Stream File { get; set; }
        public Revision Revision { get; set; }
        public string ImageId { get; set; }

        public UploadImageRevisionCommand(ImageRevisionDto model)
        {
            if (model?.file?.Length > 0)
            {
                var filename = ContentDispositionHeaderValue
                    .Parse(model.file.ContentDisposition)
                    .FileName
                    .Trim('"');

                var fileStream = model.file.OpenReadStream();
                fileStream.CopyTo(File);
                fileStream.Dispose();

                ImageId = model.imageId;

                Revision = new Revision
                {
                    Description = model.description,
                    RevisionDate = DateTime.Now,
                    Thumb = new Thumbnail
                    {
                        Filename = $"thumb_{filename}"
                    },
                    Filename = filename
                };
            }
        }
    }
}
