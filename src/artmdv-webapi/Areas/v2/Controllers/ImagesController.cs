﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using artmdv_webapi.Areas.v2.DataAccess;
using artmdv_webapi.Areas.v2.Models;
using ImageProcessorCore;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Image = artmdv_webapi.Areas.v2.Models.Image;

namespace artmdv_webapi.Areas.v2.Controllers
{
    [Area("v2")]
    [Route("[area]/Images")]
    [EnableCors("default")]
    public class ImagesController : Controller
    {
        public ImagesController()
        {
        }

        [HttpPost]
        public dynamic UploadImage(ImageUploadDto model)
        {
            try
            {
                CheckPassword(model?.password);

                if (model?.file?.Length > 0)
                {
                    var fileName = ContentDispositionHeaderValue
                        .Parse(model.file.ContentDisposition)
                        .FileName
                        .Trim('"'); 

                    var dataAcces = new ImageDataAccess();

                    var fileStream = model.file.OpenReadStream();

                    var image = new Image
                    {
                        Filename = fileName,
                        Description = model.description,
                        Title = model.title,
                        Tags = model.tags.Split(','),
                        Date = model.date,
                        Thumb = new Thumbnail
                        {
                            Filename = $"thumb_{fileName}"
                        },
                        Annotation = model.annotation,
                        Inverted = model.inverted
                    };
                    var thumbStream = new MemoryStream();
                    fileStream.CopyTo(thumbStream);
                    var thumb = GenerateThumbnail(thumbStream);

                    var imageId = dataAcces.Create(image, fileStream, thumb);

                    return GetImage(imageId.ToString());
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return null;
        }

        [HttpPost]
        [Route("Revision")]
        public dynamic ImageRevision(ImageRevisionDto model)
        {
            try
            {
                CheckPassword(model?.password);
                if (model?.file?.Length > 0 && !string.IsNullOrWhiteSpace(model.imageId))
                {
                    var fileName = ContentDispositionHeaderValue
                       .Parse(model.file.ContentDisposition)
                       .FileName
                       .Trim('"');

                    var dataAcces = new ImageDataAccess();

                    var fileStream = model.file.OpenReadStream();

                    var revision = new Revision
                    {
                        Description = model.description,
                        RevisionDate = DateTime.Now,
                        Thumb = new Thumbnail
                        {
                            Filename = $"thumb_{fileName}"
                        },
                        Filename = fileName
                    };
                    var thumbStream = new MemoryStream();
                    fileStream.CopyTo(thumbStream);
                    var thumb = GenerateThumbnail(thumbStream);
                    var thumbId = dataAcces.CreateThumb(revision.Thumb.Filename, thumb);
                    revision.Thumb.ContentId = thumbId;
                    revision.ContentId = dataAcces.CreateImage(fileStream, fileName);
                    revision.RevisionId = Guid.NewGuid().ToString();
                    
                    var image = dataAcces.Get(model.imageId);
                    if (image.Revisions == null)
                    {
                        image.Revisions = new List<Revision>();
                    }
                    image.Revisions.Add(revision);

                    return dataAcces.Update(image);
                }
            }

            catch (Exception ex)
            {
                return ex.Message;
            }
            return null;
        }

        [HttpPut]
        public dynamic UpdateImage([FromBody]ImageUpdateDto imageVm)
        {
            CheckPassword(imageVm?.password);
            var dataAcces = new ImageDataAccess();
            if (imageVm?.image != null)
            {
                var image = imageVm.image.ToImage();
                var originalImage = dataAcces.Get(image.Id.ToString());
                image.Revisions = originalImage.Revisions;
                return dataAcces.Update(image);
            }
            return null;
        }

        [HttpDelete]
        [Route("{id}/{password}")]
        public dynamic DeleteImage(string password, string id)
        {
            CheckPassword(password);
            var dataAcces = new ImageDataAccess();
            dataAcces.Delete(id);
            return null;
        }

        [HttpGet]
        [Route("{id}")]
        public dynamic GetImage(string id)
        {
            var dataAcces = new ImageDataAccess();
            var image = dataAcces.Get(id);

            return DecorateImage(image);
        }

        private dynamic DecorateImage(Image image)
        {
            var uri = new Uri(Request.GetDisplayUrl());
            var host = uri.AbsoluteUri.Replace(uri.LocalPath, "");
            var dataAcces = new ImageDataAccess();
            var imageRelativePath = dataAcces.GetPath(image);
            var imagePath = imageRelativePath != null ? $"{host}/{imageRelativePath}" : Url.Link("ImageContentRoute", new {image.Id});
            
            var thumbPath = Url.Link("ThumbContentRoute", new { image.Id });
            var annotationPath = "";
            var invertedPath = "";
            if (!string.IsNullOrEmpty(image.Annotation))
            {
                var annotationRelativePath = dataAcces.GetAnnotationPath(image);
                annotationPath = annotationRelativePath != null ? $"{host}/{annotationRelativePath}" : Url.Link("AnnotationContentRoute", new {image.Id});
            }
            if (!string.IsNullOrEmpty(image.Inverted))
            {
                var invertedRelativePath = dataAcces.GetInvertedPath(image);
                invertedPath = invertedRelativePath != null ? $"{host}/{invertedRelativePath}" : Url.Link("InvertedContentRoute", new { image.Id });
            }

            
            var revisions = new List<RevisionPaths>();
            if (image.Revisions != null)
            {
                image.Revisions = image.Revisions.OrderBy(x => x.RevisionDate).ToList();
                foreach (var revision in image?.Revisions)
                {
                    var revisionRelativePath = dataAcces.GetRevisionPath(revision);
                    var revisionImagePath = revisionRelativePath != null ? $"{host}/{revisionRelativePath}" : Url.Link("ContentIdRoute", new { id = revision.ContentId });
                    var revisionThumbPath = Url.Link("ContentIdRoute", new {id = revision.Thumb.ContentId});
                    revisions.Add(new RevisionPaths {Thumb = revisionThumbPath, Image = revisionImagePath, date = revision.RevisionDate.ToString("yyyy-MM-dd HH:mm:ss"), description = revision.Description});
                }
            }

            var result = new
            {
                Image = ImageViewModel.ToViewModel(image),
                links = new
                {
                    ImageContent = imagePath,
                    ThumbnailContent = thumbPath,
                    AnnotationContent = annotationPath,
                    InvertedContent = invertedPath,
                    Revisions = revisions
                },
                ForumPost = $"[url={Url.Link("ImageContentRoute", new { image.Id })}][img]{Url.Link("ThumbContentRoute", new { image.Id })}[/img][/url]"
            };
            return result;
        }

        [HttpGet]
        public async Task<dynamic> Getall(string tag=null)
        {
                if (tag == "all")
                {
                    tag = string.Empty;
                }
                tag = tag ?? string.Empty;
                var dataAcces = new ImageDataAccess();
                var images = await dataAcces.GetAllByTag(tag);

                images = images.OrderByDescending(x => x.Date).ToList();

                return images.Select(image => DecorateImage(image)).ToList();
        }

        [HttpGet]
        [Route("{id}/Content", Name = "ImageContentRoute")]
        public ActionResult GetImageContent(string id)
        {
            var dataAcces = new ImageDataAccess();
            var image = dataAcces.GetImageContent(id);
            return new FileStreamResult(image, "image/jpeg");
        }

        [HttpGet]
        [Route("Content/{id}", Name = "ContentIdRoute")]
        public ActionResult GetContentById(string id)
        {
            var dataAcces = new ImageDataAccess();
            var image = dataAcces.GetByContentId(id);
            return new FileStreamResult(image, "image/jpeg");
        }

        [Route("{id}/Thumbnail", Name = "ThumbContentRoute")]
        public ActionResult GetThumb(string id)
        {
                var dataAcces = new ImageDataAccess();
                var image = dataAcces.GetThumbContent(id);
                return new FileStreamResult(image, "image/jpeg");
        }

        [Route("{id}/Annotation", Name = "AnnotationContentRoute")]
        public ActionResult GetAnnotation(string id)
        {
                var dataAcces = new ImageDataAccess();
                var image = dataAcces.GetAnnotationContent(id);
                if (image == null)
                    return null;
                return new FileStreamResult(image, "image/jpeg");
        }

        [Route("{id}/Inverted", Name = "InvertedContentRoute")]
        public ActionResult GetInverted(string id)
        {
                var dataAcces = new ImageDataAccess();
                var image = dataAcces.GetInvertedContent(id);
                if (image == null)
                    return null;
                return new FileStreamResult(image, "image/jpeg");
        }

        private Stream GenerateThumbnail(MemoryStream image)
        {
            image.Position = 0;
            var resizedStream = new MemoryStream();
            var thumb = new ImageProcessorCore.Image(image);
            thumb.Resize(400, thumb.Height*400/thumb.Width).Save(resizedStream);
            resizedStream.Position = 0;
            return resizedStream;
        }

        private static void CheckPassword(string password)
        {
            var fs = new FileStream("config.json", FileMode.Open, FileAccess.Read);
            JObject config = null;
            using (StreamReader streamReader = new StreamReader(fs))
            using (JsonTextReader reader = new JsonTextReader(streamReader))
            {
                config = (JObject) JToken.ReadFrom(reader);
            }
            if (config?.GetValue("password").ToString() != password)
            {
                throw new HttpRequestException();
            }

        }
    }

    public class RevisionPaths
    {
        public string Thumb { get; set; }
        public string Image { get; set; }
        public string date { get; set; }
        public string description { get; set; }
    }

    public class ImageUpdateDto
    {
        public ImageViewModel image { get; set; }
        public string password { get; set; }
    }

    public class ImageRevisionDto
    {
        public string imageId { get; set; }
        public IFormFile file { get; set; }
        public string description { get; set; }
        public string password { get; set; }
    }

    public class ImageUploadDto
    {
        public IFormFile file { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string tags { get; set; }
        public string date { get; set; }
        public string annotation { get; set; }
        public string inverted { get; set; }
        public string password { get; set; }
    }
}
