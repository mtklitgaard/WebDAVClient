﻿using System;
using System.Collections.Generic;
using System.IO;
using FileUploader;
using FileUploader.Interfaces;
using Moq;
using NUnit.Framework;

namespace FileUploaderTests
{
    [TestFixture]
    public class UploadTests
    {
        private UploadFiles _classUnderTest;
        private Mock<IDirectoryWrapper> _directoryWrapper;


        [SetUp]
        public void Setup()
        {
            _directoryWrapper = new Mock<IDirectoryWrapper>();
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

                //String[] allfiles = Directory.GetFiles(pathToDir, "*.*", SearchOption.AllDirectories);
                //foreach (var allfile in allfiles)
                //{
                //    var filepath = allfile;
                //}
            }
        }
    }
}
