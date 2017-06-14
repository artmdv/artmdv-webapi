using System;
using System.IO;
using artmdv_webapi.Areas.v2.Controllers;
using artmdv_webapi.Areas.v2.DataAccess;
using artmdv_webapi.Areas.v2.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using MongoDB.Bson;
using Moq;
using NUnit.Framework;

namespace WebApi.Tests
{
    [TestFixture]
    public class ControllerTests
    {
        [Test]
        public void CallingImageContentReturnsStream()
        {
            var dataAccessMock = new Mock<IImageDataAccess>();
            var stream = new MemoryStream();
            stream.WriteByte(1);
            dataAccessMock.Setup(x => x.GetImageContent(It.IsAny<string>())).Returns(stream);
            var controller = new ImagesController(dataAccessMock.Object);
            var guid = Guid.NewGuid().ToString();
            var result = controller.GetImageContent(guid);

            //check if its image/jpeg

            Assert.NotNull(result);
//            Assert.IsTrue(true);
        }

        [Test]
        public void CallingGetAnnotationReturnsStream()
        {
            var dataAccessMock = new Mock<IImageDataAccess>();
            var stream = new MemoryStream();
            stream.WriteByte(1);
            dataAccessMock.Setup(x => x.GetAnnotationContent(It.IsAny<string>())).Returns(stream);
            var controller = new ImagesController(dataAccessMock.Object);
            var guid = Guid.NewGuid().ToString();
            var result = controller.GetAnnotation(guid);

            //check if its image/jpeg
            Assert.NotNull(result);
        }

        [Test]
        public void CallingGetContentByIdReturnsStream()
        {
            var dataAccessMock = new Mock<IImageDataAccess>();
            var stream = new MemoryStream();
            stream.WriteByte(1);
            dataAccessMock.Setup(x => x.GetByContentId(It.IsAny<string>())).Returns(stream);
            var controller = new ImagesController(dataAccessMock.Object);
            var guid = Guid.NewGuid().ToString();
            var result = controller.GetContentById(guid);

            //check if its image/jpeg
            Assert.NotNull(result);
        }

        [Test]
        public void CallingGetImageReturnsImageObject()
        {
            var imagePath = "ImagePath";
            var host = "testHost";
            var path = "/testPath";
            var image = new Image { Id = ObjectId.GenerateNewId() };

            var dataAccessMock = new Mock<IImageDataAccess>();

            dataAccessMock.Setup(x => x.Get(It.IsAny<string>())).Returns(image);
            dataAccessMock.Setup(x => x.GetPath(It.IsAny<Image>())).Returns("ImagePath");

            var controller = new ImagesController(dataAccessMock.Object);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            var request = controller.Request;
            request.Host = new HostString(host);
            request.Path = new PathString(path);
            request.Scheme = "http";
            var urlMock = new Mock<IUrlHelper>();
            urlMock.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>())).Returns("urlHelperLink");
            controller.Url = urlMock.Object;

            var guid = Guid.NewGuid().ToString();
            var result = controller.GetImage(guid);

            //check if its image/jpeg
            Assert.NotNull(result);
            Assert.That(result.Image.Id, Is.EqualTo(image.Id.ToString()));
        }
    }
}
