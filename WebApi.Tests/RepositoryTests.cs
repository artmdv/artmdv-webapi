using System;
using System.Collections.Generic;
using System.Text;
using artmdv_webapi.Areas.v2.Repository;
using NUnit.Framework;

namespace WebApi.Tests
{
    [TestFixture]
    public class RepositoryTests
    {
        [Test]
        public void IfImageDoesNotExistGenerateFilenameReturnsFilename()
        {
            var testFilename = "testFilename.jpg";
            var repo = new ImageRepository();
            var returnedFilename = repo.GenerateFileName(testFilename);
            Assert.That(returnedFilename, Is.EqualTo(testFilename));
        }

        [Test]
        public void IfImageExistsGenerateFilenameReturnsFilenameWithNumber1()
        {
            var testFilename = "testFilename.jpg";
            var testFilenameWith1 = "testFilename(1).jpg";
            var repo = new ImageRepository();
            var returnedFilename = repo.GenerateFileName(testFilename);
            Assert.That(returnedFilename, Is.EqualTo(testFilenameWith1));
        }

        [Test]
        public void IfImageExistsWithNumber1GenerateFilenameReturnsFilenameWithNumber2()
        {
            var testFilename = "testFilename.jpg";
            var testFilenameWith2 = "testFilename(2).jpg";
            var repo = new ImageRepository();
            var returnedFilename = repo.GenerateFileName(testFilename);
            Assert.That(returnedFilename, Is.EqualTo(testFilenameWith2));
        }
    }
}
