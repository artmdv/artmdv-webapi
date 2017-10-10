using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using artmdv_webapi.Areas.v2.Command;
using artmdv_webapi.Areas.v2.CommandHandlers;
using artmdv_webapi.Areas.v2.Commands;
using artmdv_webapi.Areas.v2.Core;
using artmdv_webapi.Areas.v2.Models;
using artmdv_webapi.Areas.v2.Query;
using artmdv_webapi.Areas.v2.Repository;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Image = artmdv_webapi.Areas.v2.Models.Image;

namespace artmdv_webapi.Areas.v2.Controllers
{
    [Area("v2")]
    [Route("[area]/Images")]
    [EnableCors("default")]
    public class ImagesController : BaseImagesController
    {
        public IQuery<FeaturedImageViewModel, QueryFilter> FeaturedImageQuery { get; }
        public IQuery<List<Image>, TagFilter> ImagesQuery { get; }

        public ImagesController(
            IImageRepository dataAccess, 
            IQuery<FeaturedImageViewModel, QueryFilter> featuredImageQuery,
            IQuery<List<Image>, TagFilter> imagesQuery): base(dataAccess)
        {
            FeaturedImageQuery = featuredImageQuery;
            ImagesQuery = imagesQuery;
        }

        //heavy usage
        [HttpGet]
        [Route("{id}")]
        public ImageResponse GetImage(string id)
        {
            return GetImageById(id);
        }
        
        //heavy usage
        [HttpGet]
        public async Task<dynamic> Getall(string tag = null)
        {
            var images = await ImagesQuery.Get(new TagFilter(tag)).ConfigureAwait(false);

            return images.Select(image => DecorateImage(image)).ToList();
        }

        //heavy usage
        [HttpGet]
        [Route("{id}/Content", Name = "ImageContentRoute")]
        public ActionResult GetImageContent(string id)
        {
            var image = DataAccess.GetImageContent(id);
            if (image != null)
            {
                return new FileStreamResult(image, "image/jpeg");
            }
            return new NotFoundResult();
        }

        //heavy usage
        [HttpGet]
        [Route("Content/{id}", Name = "ContentIdRoute")]
        public ActionResult GetContentById(string id)
        {
            var image = DataAccess.GetByContentId(id);
            if (image != null)
            {
                return new FileStreamResult(image, "image/jpeg");
            }
            return new NotFoundResult();
        }

        //heavy usage
        [Route("{id}/Thumbnail", Name = "ThumbContentRoute")]
        public ActionResult GetThumb(string id)
        {
            var image = DataAccess.GetThumbContent(id);
            if (image != null)
            {
                return new FileStreamResult(image, "image/jpeg");
            }
            return new NotFoundResult();
        }

        //most usages from UI
        [Route("{id}/Annotation", Name = "AnnotationContentRoute")]
        public ActionResult GetAnnotation(string id)
        {
            var image = DataAccess.GetAnnotationContent(id);
            if (image != null)
            {
                return new FileStreamResult(image, "image/jpeg");
            }
            return new NotFoundResult();
        }

        //most usages from UI
        [Route("{id}/Inverted", Name = "InvertedContentRoute")]
        public ActionResult GetInverted(string id)
        {
            var image = DataAccess.GetInvertedContent(id);
            if (image != null)
            {
                return new FileStreamResult(image, "image/jpeg");
            }
            return new NotFoundResult();
        }

        //only UI
        [Route("Featured")]
        [HttpGet]
        public async Task<ActionResult> GetFeaturedImage()
        {
            var featuredImage = await FeaturedImageQuery.Get(null).ConfigureAwait(false);
            return new OkObjectResult(featuredImage);
        }
    }
}
