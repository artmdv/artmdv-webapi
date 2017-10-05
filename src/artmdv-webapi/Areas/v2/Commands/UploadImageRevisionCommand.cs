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

        public UploadImageRevisionCommand(ImageRevisionDto model, Stream file, string filename)
        {
            if (file?.Length > 0)
            {
                var datenow = DateTime.Now;

                File = new MemoryStream();
                
                file.CopyTo(File);
                File.Position = 0;
                file.Dispose();

                ImageId = model.imageId;

                Revision = new Revision
                {
                    Description = model.description,
                    RevisionDate = new DateTime(datenow.Year, datenow.Month, datenow.Day, datenow.Hour, datenow.Minute, datenow.Second),
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
