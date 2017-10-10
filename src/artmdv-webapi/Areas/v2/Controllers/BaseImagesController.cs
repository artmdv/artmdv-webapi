using System;
using System.Collections.Generic;
using System.Linq;
using artmdv_webapi.Areas.v2.Models;
using artmdv_webapi.Areas.v2.Repository;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace artmdv_webapi.Areas.v2.Controllers
{
    public class BaseImagesController: Controller
    {
        public IImageRepository DataAccess { get; }

        public BaseImagesController(IImageRepository dataAccess)
        {
            DataAccess = dataAccess;
        }

        protected ImageResponse GetImageById(string id)
        {
            var image = DataAccess.Get(id);
            if (image == null)
            {
                return null;
            }
            return DecorateImage(image);
        }

        protected ImageResponse DecorateImage(Image image)
        {
            var uri = new Uri(Request.GetDisplayUrl());

            var host = uri.AbsoluteUri.Replace(uri.LocalPath, "");

            if (!string.IsNullOrEmpty(uri.Query))
            {
                host = host.Replace(uri.Query, "");
            }

            var imageRelativePath = DataAccess.GetPath(image);
            var imagePath = imageRelativePath != null
                ? $"{host}/{imageRelativePath}"
                : Url.Link("ImageContentRoute", new { image.Id });

            var thumbPath = Url.Link("ThumbContentRoute", new { image.Id });
            var annotationPath = "";
            var invertedPath = "";
            if (!string.IsNullOrEmpty(image.Annotation))
            {
                var annotationRelativePath = DataAccess.GetAnnotationPath(image);
                annotationPath = annotationRelativePath != null
                    ? $"{host}/{annotationRelativePath}"
                    : Url.Link("AnnotationContentRoute", new { image.Id });
            }
            if (!string.IsNullOrEmpty(image.Inverted))
            {
                var invertedRelativePath = DataAccess.GetInvertedPath(image);
                invertedPath = invertedRelativePath != null
                    ? $"{host}/{invertedRelativePath}"
                    : Url.Link("InvertedContentRoute", new { image.Id });
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
                        : Url.Link("ContentIdRoute", new { id = revision.ContentId });
                    var revisionThumbPath = Url.Link("ContentIdRoute", new { id = revision.Thumb.ContentId });
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
    }
}
