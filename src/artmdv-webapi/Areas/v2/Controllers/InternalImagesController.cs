using System;
using System.Linq;
using System.Threading.Tasks;
using artmdv_webapi.Areas.v2.Command;
using artmdv_webapi.Areas.v2.CommandHandlers;
using artmdv_webapi.Areas.v2.Commands;
using artmdv_webapi.Areas.v2.Core;
using artmdv_webapi.Areas.v2.Models;
using artmdv_webapi.Areas.v2.Repository;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace artmdv_webapi.Areas.v2.Controllers
{
    [Area("v2")]
    [Route("[area]/Images")]
    [EnableCors("default")]
    public class InternalImagesController: BaseImagesController
    {
        public IHandler<SetFeaturedImageCommand, object> SetFeaturedImageCommandHandler { get; }
        public IHandler<UploadImageCommand, string> UploadImageCommandHandler { get; }
        public IHandler<UploadImageRevisionCommand, object> UploadImageRevisionCommandHandler { get; }
        public ISecurityHandler SecurityHandler { get; }

        public InternalImagesController(
            IImageRepository dataAccess,
            IHandler<SetFeaturedImageCommand, object> setFeaturedImageCommandHandler,
            IHandler<UploadImageCommand, string> uploadImageCommandHandler,
            IHandler<UploadImageRevisionCommand, object> uploadImageRevisionCommandHandler,
            ISecurityHandler securityHandler): base(dataAccess)
        {
            SetFeaturedImageCommandHandler = setFeaturedImageCommandHandler;
            UploadImageCommandHandler = uploadImageCommandHandler;
            UploadImageRevisionCommandHandler = uploadImageRevisionCommandHandler;
            SecurityHandler = securityHandler;
        }

        //internal
        [HttpPost]
        public async Task<ImageResponse> UploadImage(ImageUploadDto model)
        {
            try
            {
                if (!SecurityHandler.IsValidPassword(model?.password))
                {
                    throw new UnauthorizedAccessException();
                }

                var filename = ContentDispositionHeaderValue
                    .Parse(model.file.ContentDisposition)
                    .FileName
                    .ToString().Trim('"');

                var fileStream = model.file.OpenReadStream();

                var cmd = new UploadImageCommand(model, fileStream, filename);

                var imageId = await UploadImageCommandHandler.HandleAsync(cmd).ConfigureAwait(false);

                return await Task.FromResult(GetImageById(imageId.ToString())).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //internal
        [HttpPost]
        [Route("Revision")]
        public ImageResponse ImageRevision(ImageRevisionDto model)
        {
            try
            {
                if (!SecurityHandler.IsValidPassword(model?.password))
                {
                    throw new UnauthorizedAccessException();
                }

                var filename = ContentDispositionHeaderValue
                    .Parse(model.file.ContentDisposition)
                    .FileName
                    .ToString()
                    .Trim('"');

                var fileStream = model.file.OpenReadStream();

                var cmd = new UploadImageRevisionCommand(model, fileStream, filename);

                UploadImageRevisionCommandHandler.HandleAsync(cmd);

                return GetImageById(model?.imageId);
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        //internal
        [HttpPut]
        public ActionResult UpdateImage([FromBody] ImageUpdateDto imageVm)
        {
            try
            {
                if (!SecurityHandler.IsValidPassword(imageVm?.password))
                {
                    throw new UnauthorizedAccessException();
                }

                if (imageVm?.image != null)
                {
                    var image = imageVm.image.ToImage();
                    var originalImage = DataAccess.Get(image.Id.ToString());
                    image.Revisions = originalImage.Revisions;
                    DataAccess.Update(image);
                    return new OkObjectResult(GetImageById(imageVm.image.Id));
                }
                return new BadRequestObjectResult("invalid image for update");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //internal
        [HttpDelete]
        [Route("{id}/{password}")]
        public dynamic DeleteImage(string password, string id)
        {
            if (!SecurityHandler.IsValidPassword(password))
            {
                throw new UnauthorizedAccessException();
            }

            DataAccess.Delete(id);
            return null;
        }

        //internal
        [HttpDelete]
        [Route("{id}/revision/{revisionId}/{password}")]
        public dynamic DeleteRevision(string password, string id, string revisionId)
        {
            try
            {
                if (!SecurityHandler.IsValidPassword(password))
                {
                    throw new UnauthorizedAccessException();
                }

                var image = DataAccess.Get(id);
                image.Revisions.Remove(image.Revisions.Single(x => x.RevisionId == revisionId));
                DataAccess.Update(image);
                return null;
            }

            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        //internal
        [Route("Featured/{id}/{password}")]
        [HttpPost]
        public ActionResult SetFeaturedImage(string id, string password)
        {
            if (!SecurityHandler.IsValidPassword(password))
            {
                throw new UnauthorizedAccessException();
            }

            var command = new SetFeaturedImageCommand(id);
            SetFeaturedImageCommandHandler.HandleAsync(command);
            return new OkResult();
        }
    }
}
