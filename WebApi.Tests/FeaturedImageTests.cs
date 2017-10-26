using System;
using System.Threading.Tasks;
using artmdv_webapi.Areas.v2.Command;
using artmdv_webapi.Areas.v2.CommandHandlers;
using artmdv_webapi.Areas.v2.Models;
using artmdv_webapi.Areas.v2.Query;
using artmdv_webapi.Areas.v2.Repository;
using MongoDB.Bson;
using Moq;
using NUnit.Framework;

namespace WebApi.Tests
{
    [TestFixture]
    public class FeaturedImageTests
    {
        [Test]
        public async Task CallingFeaturedImageQueryGetReturnsFeaturedImage()
        {
            var image = new FeaturedImage
            {
                Date = DateTime.Now,
                ImageId = ObjectId.GenerateNewId()
            };
            var repositoryMock = new Mock<IFeaturedImageRepository>();
            repositoryMock.Setup(x => x.GetLatest()).Returns(image);
            var query = new FeaturedImageQuery(repositoryMock.Object);
            var featuredImage = await query.Get(null).ConfigureAwait(false);
            Assert.That(featuredImage, Is.Not.Null);
            Assert.That(featuredImage.Date, Is.Not.Null);
            Assert.That(featuredImage.ImageId, Is.Not.Null);
            Assert.That(featuredImage.ImageId, Is.Not.EqualTo(ObjectId.Empty));
            Assert.That(featuredImage.Date.Year, Is.GreaterThan(1));
        }

        [Test]
        public void CallingSetFeaturedImageCommandWritesToRepository()
        {
            var repositoryMock = new Mock<IFeaturedImageRepository>();
            var handler = new SetFeaturedImageHandler(repositoryMock.Object);
            handler.HandleAsync(new SetFeaturedImageCommand(ObjectId.GenerateNewId().ToString()));

            repositoryMock.Verify(x=>x.Save(It.IsAny<FeaturedImage>()));
        }

        [Test]
        public void CallingSetFeaturedImageCommandWritesToRepositoryWithDate()
        {
            var repositoryMock = new Mock<IFeaturedImageRepository>();
            var handler = new SetFeaturedImageHandler(repositoryMock.Object);
            handler.HandleAsync(new SetFeaturedImageCommand(ObjectId.GenerateNewId().ToString()));

            repositoryMock.Verify(x=>x.Save(It.Is<FeaturedImage>(img=>img.ImageId != null && img.ImageId != ObjectId.Empty && img.Date != null && img.Date != default(DateTime) )));
        }
    }
}
