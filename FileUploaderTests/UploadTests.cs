using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace FileUploaderTests
{
    [TestFixture]
    public class UploadTests
    {
        [SetUp]
        public void Setup()
        {
            
        }

        public class Upload : UploadTests
        {
            [Test]
            public void CallsGetSubDirectories_FromListDirectories()
            {
                
            }
        }
    }
}
