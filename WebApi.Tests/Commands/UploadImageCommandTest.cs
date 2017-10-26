using System.IO;
using System.Text;
using artmdv_webapi.Areas.v2.Command;
using artmdv_webapi.Areas.v2.Models;
using NUnit.Framework;

namespace WebApi.Tests.Commands
{
    [TestFixture]
    public class UploadImageCommandTest
    {
        [Test]
        public void PassingImageDtoAndFileInfoGetsCorrectlyConverted()
        {
            var dto = new ImageUploadDto();
            dto.annotation = "a";
            dto.date = "date";
            dto.description = "desc";
            dto.inverted = "i";
            dto.tags = "one,two,three";
            dto.title = "title";

            var stream = new MemoryStream(Encoding.UTF8.GetBytes("asdf"));
            var filename = "testname";

            var cmd = new UploadImageCommand(dto, stream, filename);

            Assert.NotNull(cmd.File);
            Assert.That(cmd.Image.Filename, Is.EqualTo(filename));
            Assert.That(cmd.Image.Thumb.Filename, Is.EqualTo($"thumb_{filename}"));
            Assert.That(cmd.Image.Annotation, Is.EqualTo(dto.annotation));
            Assert.That(cmd.Image.Date, Is.EqualTo(dto.date));
            Assert.That(cmd.Image.Description, Is.EqualTo(dto.description));
            Assert.That(cmd.Image.Inverted, Is.EqualTo(dto.inverted));
            Assert.That(cmd.Image.Title, Is.EqualTo(dto.title));
            Assert.That(cmd.Image.Tags.Length, Is.EqualTo(3));

        }
    }
}
