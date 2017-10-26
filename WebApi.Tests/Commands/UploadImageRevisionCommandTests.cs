using System.IO;
using System.Text;
using artmdv_webapi.Areas.v2.Commands;
using artmdv_webapi.Areas.v2.Models;
using NUnit.Framework;

namespace WebApi.Tests.Commands
{
    [TestFixture]
    public class UploadImageRevisionCommandTests
    {
        [Test]
        public void PassingRevisionDtoAndFileInfoGetsCorrectlyConverted()
        {
            var dto = new ImageRevisionDto();
            dto.description = "desc";
            dto.imageId = "imageId";

            var stream = new MemoryStream(Encoding.UTF8.GetBytes("asdf"));
            var filename = "testname";

            var cmd = new UploadImageRevisionCommand(dto, stream, filename);

            Assert.NotNull(cmd.File);
            Assert.That(cmd.Revision.Filename, Is.EqualTo(filename));
            Assert.That(cmd.Revision.Description, Is.EqualTo(dto.description));
            Assert.That(cmd.ImageId, Is.EqualTo(dto.imageId));
            Assert.That(cmd.Revision.Filename, Is.EqualTo(filename));
            Assert.That(cmd.Revision.RevisionDate, Is.Not.Null);
            Assert.That(cmd.Revision.Thumb.Filename, Is.EqualTo($"thumb_{filename}"));
            
        }
    }
}
