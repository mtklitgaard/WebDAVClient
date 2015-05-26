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


        [SetUp]
        public void Setup()
        {
            _directoryWrapper = new Mock<IDirectoryWrapper>();
            _webDAVOperator = new Mock<IWebDAVOperator>();
            _classUnderTest = new UploadFiles(_directoryWrapper.Object, _webDAVOperator.Object);
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
            public void CreatesRootDirectoryByStrippingOffTheDriveLetter_OnWebDAVOperator()
            {
                var expectedRootFolder = "TestUpload";
                var pathToDir = @"C:\" + expectedRootFolder;
                var expected = new List<string>();
                _directoryWrapper.Setup(x => x.GetSubDirectoriesAndFiles(pathToDir)).Returns(expected);
                
                _classUnderTest.Upload(pathToDir);

                _directoryWrapper.Verify(x => x.GetSubDirectoriesAndFiles(pathToDir));
                _webDAVOperator.Verify(x => x.CreateDir(It.Is<string>(y => y.Equals("/")), expectedRootFolder));
            }   
            
            [Test]
            public void StripsOffTheDriveLetterWhenTheDriveLetterIsNotC_OnWebDAVOperator()
            {
                var expectedRootFolder = "TestUpload";
                var pathToDir = @"E:\" + expectedRootFolder;
                var expected = new List<string>();
                _directoryWrapper.Setup(x => x.GetSubDirectoriesAndFiles(pathToDir)).Returns(expected);
                
                _classUnderTest.Upload(pathToDir);

                _directoryWrapper.Verify(x => x.GetSubDirectoriesAndFiles(pathToDir));
                _webDAVOperator.Verify(x => x.CreateDir(It.Is<string>(y => y.Equals("/")), expectedRootFolder));
            }
        }
    }
}
