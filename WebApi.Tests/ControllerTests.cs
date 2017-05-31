using System;
using System.IO;
using artmdv_webapi.Areas.v2.Controllers;
using artmdv_webapi.Areas.v2.DataAccess;
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
    }
}
