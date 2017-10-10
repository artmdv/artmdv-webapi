using System;
using System.IO;
using artmdv_webapi.Areas.v2.Controllers;
using artmdv_webapi.Areas.v2.Core;
using artmdv_webapi.Areas.v2.Models;
using artmdv_webapi.Areas.v2.Query;
using artmdv_webapi.Areas.v2.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Moq;
using NUnit.Framework;

namespace WebApi.Tests
{
    [TestFixture]
    public class ControllerTests
    {
        private Mock<ISecurityHandler> _securityHandler;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _securityHandler = new Mock<ISecurityHandler>();
            _securityHandler.Setup(x => x.IsValidPassword(It.IsAny<string>())).Returns(true);
        }

        [Test]
        public void CallingImageContentReturnsStream()
        {
            var dataAccessMock = new Mock<IImageRepository>();
            var stream = new MemoryStream();
            stream.WriteByte(1);
            dataAccessMock.Setup(x => x.GetImageContent(It.IsAny<string>())).Returns(stream);
            var controller = new ImagesController(dataAccessMock.Object, null, null);
            var guid = Guid.NewGuid().ToString();
            var result = controller.GetImageContent(guid);

            //check if its image/jpeg

            Assert.NotNull(result);
//            Assert.IsTrue(true);
        }

        [Test]
        public void CallingGetAnnotationReturnsStream()
        {
            var dataAccessMock = new Mock<IImageRepository>();
            var stream = new MemoryStream();
            stream.WriteByte(1);
            dataAccessMock.Setup(x => x.GetAnnotationContent(It.IsAny<string>())).Returns(stream);
            var controller = new ImagesController(dataAccessMock.Object, null, null);
            var guid = Guid.NewGuid().ToString();
            var result = controller.GetAnnotation(guid);

            //check if its image/jpeg
            Assert.NotNull(result);
        }

        [Test]
        public void CallingGetContentByIdReturnsStream()
        {
            var dataAccessMock = new Mock<IImageRepository>();
            var stream = new MemoryStream();
            stream.WriteByte(1);
            dataAccessMock.Setup(x => x.GetByContentId(It.IsAny<string>())).Returns(stream);
            var controller = new ImagesController(dataAccessMock.Object, null, null);
            var guid = Guid.NewGuid().ToString();
            var result = controller.GetContentById(guid);

            //check if its image/jpeg
            Assert.NotNull(result);
        }

        [Test]
        public void CallingGetImageReturnsImageObject()
        {
            
            var image = new Image { Id = ObjectId.GenerateNewId() };

            var dataAccessMock = new Mock<IImageRepository>();

            dataAccessMock.Setup(x => x.Get(It.IsAny<string>())).Returns(image);
            dataAccessMock.Setup(x => x.GetPath(It.IsAny<Image>())).Returns("ImagePath");

            var controller = new ImagesController(dataAccessMock.Object, null, null);
            SetupImagePath(controller);

            var guid = Guid.NewGuid().ToString();
            var result = controller.GetImage(guid);

            //check if its image/jpeg
            Assert.NotNull(result);
            Assert.That(result.Image.Id, Is.EqualTo(image.Id.ToString()));
        }

        [Test]
        public void CallingGetFeaturedImageCallsQueryGet()
        {
            var featuredImageQueryMock = new Mock<IQuery<FeaturedImageViewModel, QueryFilter>>();
            var controller = new ImagesController(null, featuredImageQueryMock.Object, null);
            
            controller.GetFeaturedImage();

            featuredImageQueryMock.Verify(x=>x.Get(It.IsAny<QueryFilter>()));
        }

        private void SetupImagePath(Controller controller)
        {
            var host = "testHost";
            var path = "/testPath";

            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            var request = controller.Request;
            request.Host = new HostString(host);
            request.Path = new PathString(path);
            request.Scheme = "http";
            var urlMock = new Mock<IUrlHelper>();
            urlMock.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>())).Returns("urlHelperLink");
            controller.Url = urlMock.Object;
        }
    }
}
