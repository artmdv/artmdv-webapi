using System;
using System.Collections.Generic;
using System.Text;
using artmdv_webapi.Areas.v2.Core;
using artmdv_webapi.Areas.v2.Infrastructure;
using artmdv_webapi.Areas.v2.Repository;
using Moq;
using NUnit.Framework;

namespace WebApi.Tests
{
    [TestFixture]
    public class RepositoryTests
    {
        Mock<IConfigurationManager> cfgMngr;

        [SetUp]
        public void Setup()
        {
            cfgMngr = new Mock<IConfigurationManager>();
            cfgMngr.Setup(x => x.GetValue(It.Is<string>(s => s == "database"))).Returns("v2");
        }

        [Test]
        public void IfImageDoesNotExistGenerateFilenameReturnsFilename()
        {
            var testFilename = "testFilename.jpg";
            var filemock = new Mock<IFile>();
            filemock.Setup(x => x.Exists(It.IsAny<string>())).Returns(false);
            var repo = new ImageRepository(filemock.Object, new Mock<IDirectory>().Object, cfgMngr.Object);
            var returnedFilename = repo.GenerateFileName(testFilename, string.Empty);
            Assert.That(returnedFilename, Is.EqualTo(testFilename));
        }

        [Test]
        public void IfImageExistsGenerateFilenameReturnsFilenameWithNumber1()
        {
            var testFilename = "testFilename.jpg";
            var testFilenameWith1 = "testFilename(1).jpg";
            var filemock = new Mock<IFile>();
            filemock.Setup(x => x.Exists(It.Is<string>(s=>s == "/" + testFilename))).Returns(true);
            filemock.Setup(x => x.Exists(It.Is<string>(s=>s != "/" + testFilename))).Returns(false);
            var repo = new ImageRepository(filemock.Object, new Mock<IDirectory>().Object, cfgMngr.Object);
            var returnedFilename = repo.GenerateFileName(testFilename, string.Empty);
            Assert.That(returnedFilename, Is.EqualTo(testFilenameWith1));
        }

        [Test]
        public void IfImageExistsWithNumber1GenerateFilenameReturnsFilenameWithNumber2()
        {
            var testFilename = "testFilename.jpg";
            var testFilenameWith1 = "testFilename(1).jpg";
            var testFilenameWith2 = "testFilename(2).jpg";
            var filemock = new Mock<IFile>();
            filemock.Setup(x => x.Exists(It.Is<string>(s => s == "/" + testFilename || s == "/" + testFilenameWith1))).Returns(true);
            filemock.Setup(x => x.Exists(It.Is<string>(s => s != "/" + testFilename && s != "/" + testFilenameWith1))).Returns(false);
            var repo = new ImageRepository(filemock.Object, new Mock<IDirectory>().Object, cfgMngr.Object);
            var returnedFilename = repo.GenerateFileName(testFilename, string.Empty);
            Assert.That(returnedFilename, Is.EqualTo(testFilenameWith2));
        }
    }
}
