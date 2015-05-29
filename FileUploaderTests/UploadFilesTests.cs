using System.Collections.Generic;
using FileUploader.Interfaces;
using Moq;
using NUnit.Framework;
using WebDAVClient.Interfaces;

namespace FileUploaderTests
{
    [TestFixture]
    public class UploadFilesTests
    {
        private FileUploader.UploadFiles _classUnderTest;
        private Mock<IDirectoryWrapper> _directoryWrapper;
        private Mock<IWebDAVOperator> _webDAVOperator;
        private Mock<IFileCreator> _fileCreator;


        [SetUp]
        public void Setup()
        {
            _directoryWrapper = new Mock<IDirectoryWrapper>();
            _webDAVOperator = new Mock<IWebDAVOperator>();
            _fileCreator = new Mock<IFileCreator>();
            _classUnderTest = new FileUploader.UploadFiles(_directoryWrapper.Object, _webDAVOperator.Object, _fileCreator.Object);
        }

        public class UploadFiles : UploadFilesTests
        {
            [Test]
            public void CallsGetDirectories_FromDirectoryWrapper()
            {
                var pathToDir = @"C:\TestUpload";
                var expected = new List<string>();
                _directoryWrapper.Setup(x => x.GetTopDirectories(pathToDir)).Returns(expected);

                _classUnderTest.Upload(pathToDir);

                _directoryWrapper.Verify(x => x.GetTopDirectories(pathToDir));
            }

            [Test]
            public void CreatesRootDirectoryByStrippingOffTheDriveLetter_OnWebDAVOperator()
            {
                var expectedRootFolder = "TestUpload";
                var pathToDir = @"C:\" + expectedRootFolder;
                var expected = new List<string>();
                _directoryWrapper.Setup(x => x.GetTopDirectories(pathToDir)).Returns(expected);
                
                _classUnderTest.Upload(pathToDir);

                _webDAVOperator.Verify(x => x.CreateDir(It.Is<string>(y => y.Equals("/")), expectedRootFolder));
            }   
            
            [Test]
            public void StripsOffTheDriveLetterWhenTheDriveLetterIsNotC_OnWebDAVOperator()
            {
                var expectedRootFolder = "TestUpload";
                var pathToDir = @"E:\" + expectedRootFolder;
                var expected = new List<string>();
                _directoryWrapper.Setup(x => x.GetTopDirectories(pathToDir)).Returns(expected);
                
                _classUnderTest.Upload(pathToDir);

                _webDAVOperator.Verify(x => x.CreateDir(It.Is<string>(y => y.Equals("/")), expectedRootFolder));
            }

            [Test]
            public void CallsCreateDirectoryOnEveryDirectoryUnderTheRoot()
            {
                var expectedRootFolder = "TestUpload";
                var pathToDir = @"C:\" + expectedRootFolder;
                var expected = new List<string>
                {
                    "C:\\TestUpload\\folder2",
                    "C:\\TestUpload\\New folder"
                };
                _directoryWrapper.Setup(x => x.GetTopDirectories(pathToDir)).Returns(expected);
                
                _classUnderTest.Upload(pathToDir);
                
                _webDAVOperator.Verify(x => x.CreateDir(expectedRootFolder, "folder2"));
                _webDAVOperator.Verify(x => x.CreateDir(expectedRootFolder, "New folder"));
            } 
            
            [Test]
            public void CallsCreateDirectoryOnEveryDirectoryUnderTheRoot_WhenFolderSubdirectoryIsMoreThanOneDeep()
            {
                var expectedRootFolder = "TestUpload";
                var pathToDir = @"C:\" + expectedRootFolder;
                 var expected1 = new List<string>
                {
                    "C:\\TestUpload\\folder2"
                };

                var expected2 = new List<string>
                {
                    "C:\\TestUpload\\folder2\\New folder"
                };

                _directoryWrapper.SetupSequence(x => x.GetTopDirectories(It.IsAny<string>())).Returns(expected1).Returns(expected2);
                
                _classUnderTest.Upload(pathToDir);
                
                _webDAVOperator.Verify(x => x.CreateDir(expectedRootFolder, "folder2"));
                _webDAVOperator.Verify(x => x.CreateDir(expectedRootFolder + "\\folder2", "New folder"));
            } 
            
            [Test]
            public void CallCreateDirectoriesIfMoreDirectoriesExist_WhenFolderSubdirectoryIsMoreThanOneDeep()
            {
                var expectedRootFolder = "TestUpload";
                var pathToDir = @"C:\" + expectedRootFolder;
                var expected1 = new List<string>
                {
                    "C:\\TestUpload\\folder2"
                };

                var expected2 = new List<string>
                {
                    "C:\\TestUpload\\folder2\\New folder"
                };

                var expected3 = new List<string>
                {
                     "C:\\TestUpload\\folder2\\New folder\\Test"
                };
                   
                _directoryWrapper.SetupSequence(x => x.GetTopDirectories(It.IsAny<string>())).Returns(expected1).Returns(expected2).Returns(expected3);
                
                _classUnderTest.Upload(pathToDir);
                
                _webDAVOperator.Verify(x => x.CreateDir(expectedRootFolder, "folder2"));
                _webDAVOperator.Verify(x => x.CreateDir(expectedRootFolder + "\\folder2", "New folder"));
                _webDAVOperator.Verify(x => x.CreateDir(expectedRootFolder + "\\folder2\\New folder", "Test"));
            }

            [Test]
            public void CallsFileCreatorWithDirectory_InOrderToStartTheFileUploadProcess()
            {
                var expectedRootFolder = "TestUpload";
                var uploadFolder = expectedRootFolder + "\\" + "folder2";
                var pathToDir = @"C:\" + expectedRootFolder;
                var expected = new List<string>
                {
                    "C:\\TestUpload\\folder2"
                };
                _directoryWrapper.Setup(x => x.GetTopDirectories(pathToDir)).Returns(expected);

                _classUnderTest.Upload(pathToDir);

                _webDAVOperator.Verify(x => x.CreateDir(expectedRootFolder, "folder2"));
                _fileCreator.Verify(x => x.Upload(expected[0], uploadFolder));
            } 
        }
    }
}
