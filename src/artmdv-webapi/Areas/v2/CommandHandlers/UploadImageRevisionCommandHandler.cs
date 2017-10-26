using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using artmdv_webapi.Areas.v2.Commands;
using artmdv_webapi.Areas.v2.Models;
using artmdv_webapi.Areas.v2.Repository;

namespace artmdv_webapi.Areas.v2.CommandHandlers
{
    public class UploadImageRevisionCommandHandler: ImageHandler<UploadImageRevisionCommand, object>
    {
        private IImageRepository DataAccess { get; set; }

        public UploadImageRevisionCommandHandler(IImageRepository dataAccess)
        {
            DataAccess = dataAccess;
        }

        public override Task<object> HandleAsync(UploadImageRevisionCommand cmd)
        {
            var thumbStream = new MemoryStream();
            cmd.File.CopyTo(thumbStream);
            var thumb = base.GenerateThumbnail(thumbStream);
            var thumbId = DataAccess.CreateThumb(cmd.Revision.Thumb.Filename, thumb);

            cmd.Revision.Thumb.ContentId = thumbId;
            cmd.Revision.Filename = DataAccess.CreateImageFile(cmd.File, cmd.Revision.Filename);
            cmd.Revision.RevisionId = Guid.NewGuid().ToString();

            var image = DataAccess.Get(cmd.ImageId);
            if (image.Revisions == null)
            {
                image.Revisions = new List<Revision>();
            }
            image.Revisions.Add(cmd.Revision);

            DataAccess.Update(image);

            return null;
        }
    }
}
