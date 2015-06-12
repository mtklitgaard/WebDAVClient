using System.Collections.Generic;
using System.IO;
using FileUploader;
using FileUploader.Interfaces;
using Moq;
using NUnit.Framework;
using WebDAVClient.Interfaces;

namespace FileUploaderTests
{
    public class FileCreatorTests
    {
        private Mock<IWebDAVOperator> _webDAVOperator;
        private Mock<IDirectoryUtilityWrapper> _directoryUtilityWrapper;
        private FileCreator _classUnderTest;

        [SetUp]
        public void Setup()
        {
            _webDAVOperator = new Mock<IWebDAVOperator>();
            _directoryUtilityWrapper = new Mock<IDirectoryUtilityWrapper>();
            _classUnderTest = new FileCreator(_webDAVOperator.Object, _directoryUtilityWrapper.Object);
        }

        public class Upload : FileCreatorTests
        {
            [Test]
            public void CallsGetFilesAndCallsUploadFiles_WhenFilesExist()
            {
                var directoryName = "test";
                var fileLocation = "file1";
                var files = new List<string>
                {
                    fileLocation
                };

                _directoryUtilityWrapper.Setup(x => x.GetAllFiles(directoryName)).Returns(files);
                
                _classUnderTest.Upload(directoryName, "test");

                _directoryUtilityWrapper.Verify(x => x.GetAllFiles(directoryName));
                _webDAVOperator.Verify(x => x.Upload(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<string>()));
            }
        }
    }
}
