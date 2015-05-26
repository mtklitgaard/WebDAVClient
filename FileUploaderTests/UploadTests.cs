using System;
using System.Collections.Generic;
using System.IO;
using FileUploader;
using FileUploader.Interfaces;
using Moq;
using NUnit.Framework;
using WebDAVClient.Interfaces;

namespace FileUploaderTests
{
    [TestFixture]
    public class UploadTests
    {
        private UploadFiles _classUnderTest;
        private Mock<IDirectoryWrapper> _directoryWrapper;
        private Mock<IWebDAVOperator> _webDAVOperator;
        private Mock<IServerAdapter> _serverAdapter;


        [SetUp]
        public void Setup()
        {
            _directoryWrapper = new Mock<IDirectoryWrapper>();
            _webDAVOperator = new Mock<IWebDAVOperator>();
            _serverAdapter = new Mock<IServerAdapter>();
            _classUnderTest = new UploadFiles(_directoryWrapper.Object);
        }

        public class Upload : UploadTests
        {
            [Test]
            public void CallsGetSubDirectoriesAndFiles_FromDirectoryWrapper()
            {
                var pathToDir = @"C:\TestUpload";
                var expected = new List<string>();
                _directoryWrapper.Setup(x => x.GetSubDirectoriesAndFiles(pathToDir)).Returns(expected);

                _classUnderTest.Upload(pathToDir);

                _directoryWrapper.Verify(x => x.GetSubDirectoriesAndFiles(pathToDir));
            }

            [Test]
            public void CreatesRootDirectory_OnWebDAVOperator()
            {
                var pathToDir = @"C:\TestUpload";
                var expected = new List<string>();
                _directoryWrapper.Setup(x => x.GetSubDirectoriesAndFiles(pathToDir)).Returns(expected);

                _classUnderTest.Upload(pathToDir);

                _directoryWrapper.Verify(x => x.GetSubDirectoriesAndFiles(pathToDir));   
            }
        }
    }
}
