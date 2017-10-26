using System.IO;
using System.Threading.Tasks;
using artmdv_webapi.Areas.v2.Command;
using artmdv_webapi.Areas.v2.Repository;

namespace artmdv_webapi.Areas.v2.CommandHandlers
{
    public class UploadImageCommandHandler: ImageHandler<UploadImageCommand, string>
    {
        private IImageRepository DataAccess { get; set; }

        public UploadImageCommandHandler(IImageRepository dataAccess)
        {
            DataAccess = dataAccess;
        }

        public override Task<string> HandleAsync(UploadImageCommand cmd)
        {
            var thumbStream = new MemoryStream();
            cmd.File.CopyTo(thumbStream);
            var thumb = base.GenerateThumbnail(thumbStream);
            
            var imageId = DataAccess.Create(cmd.Image, cmd.File, thumb);
            return Task.FromResult(imageId);
        }
    }
}
