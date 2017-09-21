using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using artmdv_webapi.Areas.v2.Command;
using artmdv_webapi.Areas.v2.CommandHandlers;
using artmdv_webapi.Areas.v2.Commands;
using artmdv_webapi.Areas.v2.Core;
using artmdv_webapi.Areas.v2.Models;
using artmdv_webapi.Areas.v2.Query;
using artmdv_webapi.Areas.v2.Repository;
using ImageProcessorCore;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Image = artmdv_webapi.Areas.v2.Models.Image;

namespace artmdv_webapi.Areas.v2.Controllers
{
    [Area("v2")]
    [Route("[area]/Images")]
    [EnableCors("default")]
    public class ImagesController : Controller
    {
        private IImageRepository DataAccess { get; set; }
        public IQuery<FeaturedImageViewModel> FeaturedImageQuery { get; }
        public IHandler<SetFeaturedImageCommand, object> SetFeaturedImageCommandHandler { get; }
        public IHandler<UploadImageCommand, string> UploadImageCommandHandler { get; }
        public IHandler<UploadImageRevisionCommand, string> UploadImageRevisionCommandHandler { get; }
        public ISecurityHandler SecurityHandler { get; }

        public ImagesController(
            IImageRepository dataAccess, 
            IQuery<FeaturedImageViewModel> featuredImageQuery, 
            IHandler<SetFeaturedImageCommand, object> setFeaturedImageCommandHandler,
            IHandler<UploadImageCommand, string> uploadImageCommandHandler,
            IHandler<UploadImageRevisionCommand, string> uploadImageRevisionCommandHandler,
            ISecurityHandler securityHandler)
        {
            DataAccess = dataAccess;
            FeaturedImageQuery = featuredImageQuery;
            SetFeaturedImageCommandHandler = setFeaturedImageCommandHandler;
            UploadImageCommandHandler = uploadImageCommandHandler;
            UploadImageRevisionCommandHandler = uploadImageRevisionCommandHandler;
            SecurityHandler = securityHandler;
        }

        [HttpPost]
        public ImageResponse UploadImage(ImageUploadDto model)
        {
            try
            {
                if (!SecurityHandler.IsValidPassword(model?.password))
                {
                    throw new UnauthorizedAccessException();
                }

                var cmd = new UploadImageCommand(model);

                var imageId = UploadImageCommandHandler.HandleAsync(cmd);

                return GetImage(imageId.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        [Route("Revision")]
        public dynamic ImageRevision(ImageRevisionDto model)
        {
            try
            {
                if (!SecurityHandler.IsValidPassword(model?.password))
                {
                    throw new UnauthorizedAccessException();
                }

                var cmd = new UploadImageRevisionCommand(model);

                UploadImageRevisionCommandHandler.HandleAsync(cmd);
                if (model == null)
                {
                    return null;
                }
                return GetImage(model.imageId);
            }

            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [HttpPut]
        public dynamic UpdateImage([FromBody] ImageUpdateDto imageVm)
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
                    return DataAccess.Update(image);
                }
                return null;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

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


        [HttpGet]
        [Route("{id}")]
        public ImageResponse GetImage(string id)
        {
            var image = DataAccess.Get(id);
            if (image == null)
            {
                return null;
            }
            return DecorateImage(image);
        }

        private ImageResponse DecorateImage(Image image)
        {
            var uri = new Uri(Request.GetDisplayUrl());
            var host = uri.AbsoluteUri.Replace(uri.LocalPath, "");
            var imageRelativePath = DataAccess.GetPath(image);
            var imagePath = imageRelativePath != null
                ? $"{host}/{imageRelativePath}"
                : Url.Link("ImageContentRoute", new {image.Id});

            var thumbPath = Url.Link("ThumbContentRoute", new {image.Id});
            var annotationPath = "";
            var invertedPath = "";
            if (!string.IsNullOrEmpty(image.Annotation))
            {
                var annotationRelativePath = DataAccess.GetAnnotationPath(image);
                annotationPath = annotationRelativePath != null
                    ? $"{host}/{annotationRelativePath}"
                    : Url.Link("AnnotationContentRoute", new {image.Id});
            }
            if (!string.IsNullOrEmpty(image.Inverted))
            {
                var invertedRelativePath = DataAccess.GetInvertedPath(image);
                invertedPath = invertedRelativePath != null
                    ? $"{host}/{invertedRelativePath}"
                    : Url.Link("InvertedContentRoute", new {image.Id});
            }


            var revisions = new List<RevisionPaths>();
            if (image.Revisions != null)
            {
                image.Revisions = image.Revisions.OrderBy(x => x.RevisionDate).ToList();
                foreach (var revision in image?.Revisions)
                {
                    var revisionRelativePath = DataAccess.GetRevisionPath(revision);
                    var revisionImagePath = revisionRelativePath != null
                        ? $"{host}/{revisionRelativePath}"
                        : Url.Link("ContentIdRoute", new {id = revision.ContentId});
                    var revisionThumbPath = Url.Link("ContentIdRoute", new {id = revision.Thumb.ContentId});
                    revisions.Add(new RevisionPaths
                    {
                        id = revision.RevisionId,
                        Thumb = revisionThumbPath,
                        Image = revisionImagePath,
                        date = revision.RevisionDate,
                        description = revision.Description
                    });
                }
            }

            var result = new ImageResponse
            {
                Image = ImageViewModel.ToViewModel(image),
                links = new Links
                {
                    ImageContent = imagePath,
                    ThumbnailContent = thumbPath,
                    AnnotationContent = annotationPath,
                    InvertedContent = invertedPath,
                    Revisions = revisions
                }
            };
            return result;
        }

        [HttpGet]
        public async Task<dynamic> Getall(string tag = null)
        {
            if (tag == "all")
            {
                tag = string.Empty;
            }
            tag = tag ?? string.Empty;
            var images = await DataAccess.GetAllByTag(tag).ConfigureAwait(false);

            images = images.OrderByDescending(x => x.Date).ToList();

            return images.Select(image => DecorateImage(image)).ToList();
        }

        [HttpGet]
        [Route("{id}/Content", Name = "ImageContentRoute")]
        public ActionResult GetImageContent(string id)
        {
            var image = DataAccess.GetImageContent(id);
            if (image != null)
            {
                return new FileStreamResult(image, "image/jpeg");
            }
            return null;
        }

        [HttpGet]
        [Route("Content/{id}", Name = "ContentIdRoute")]
        public ActionResult GetContentById(string id)
        {
            var image = DataAccess.GetByContentId(id);
            if (image != null)
            {
                return new FileStreamResult(image, "image/jpeg");
            }
            return null;
        }

        [Route("{id}/Thumbnail", Name = "ThumbContentRoute")]
        public ActionResult GetThumb(string id)
        {
            var image = DataAccess.GetThumbContent(id);
            if (image != null)
            {
                return new FileStreamResult(image, "image/jpeg");
            }
            return null;
        }

        [Route("{id}/Annotation", Name = "AnnotationContentRoute")]
        public ActionResult GetAnnotation(string id)
        {
            var image = DataAccess.GetAnnotationContent(id);
            if (image != null)
            {
                return new FileStreamResult(image, "image/jpeg");
            }
            return null;
        }

        [Route("{id}/Inverted", Name = "InvertedContentRoute")]
        public ActionResult GetInverted(string id)
        {
            var image = DataAccess.GetInvertedContent(id);
            if (image != null)
            {
                return new FileStreamResult(image, "image/jpeg");
            }
            return null;
        }

        

        [Route("Featured")]
        [HttpGet]
        public ActionResult GetFeaturedImage()
        {
            var featuredImage = FeaturedImageQuery.Get();
            return new OkObjectResult(featuredImage);
        }

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
