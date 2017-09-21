using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ImageProcessorCore;

namespace artmdv_webapi.Areas.v2.CommandHandlers
{
    public abstract class ImageHandler<TCommand, TResponse>: IHandler<TCommand,TResponse>
    {
        public abstract Task<TResponse> HandleAsync(TCommand model);

        protected Stream GenerateThumbnail(MemoryStream image)
        {
            image.Position = 0;
            var resizedStream = new MemoryStream();
            var thumb = new ImageProcessorCore.Image(image);
            thumb.Resize(400, thumb.Height * 400 / thumb.Width).Save(resizedStream);
            resizedStream.Position = 0;
            return resizedStream;
        }
    }
}
