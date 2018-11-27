using System.Collections.Specialized;
using System.Text;
using System.Web;
using System.Web.Routing;
using Xunit;
using FakeItEasy;
using Web_API;
using System.Web.Http.Controllers;
using Web_API.Controllers;
using System.Web.Http;
using System.Web.Http.Routing;
using System.Net.Http;
using Moq;
using System.IO;
using System.Net.Http.Headers;
using Web_API.CustomResponse;
using TypeMock.ArrangeActAssert;
using System.Web.SessionState;
using Services;
using Models;
using System.Threading.Tasks;
using System;

namespace Web_API_xUnit_Test
{
    public class PackageTests
    {
      

        [Fact]
        public void PackagePost_ErrorWhileUploading()
        {

            var MockRequest = new Mock<HttpRequestBase>();

            MockRequest.Setup(x => x.Headers.Get("Authorization")).Returns(string.Empty);
            MockRequest.Setup(x => x.Form.Get("Name")).Returns("Package Name");
            MockRequest.Setup(x => x.Files.Count).Returns(1);
            MockRequest.Setup(x => x.Files[0].ContentLength).Returns(900000);
            MockRequest.Setup(x => x.Files[0].ContentType).Returns("application/pdf");

            MockRequest.Setup(x => x.Files[0].FileName).Returns("test1.pdf");
            MockRequest.Setup(x => x.Files[0].InputStream).Returns(new MemoryStream());

            var dir = Path.GetFullPath(".");
            MockRequest.Setup(x => x.PhysicalApplicationPath).Returns(dir);

            var MockUserService = new Mock<UserService>();
            MockUserService.Setup(x => x.TokenToUser(It.IsAny<string>())).Returns(new User());

            var MockPackageService = new Mock<PackageService>();
            MockPackageService.SetReturnsDefault < Task<Package>>( null );

            var MockFileService = new Mock<FileService>();
            MockFileService.Setup(x => x.Insert(It.IsAny<Models.File>())).Throws(new Exception());

            var pc = new PackageController(MockFileService.Object, MockPackageService.Object , MockUserService.Object, MockRequest.Object);

            var res = pc.Post();
            Assert.IsType<FailedResponse>(res);
        }

        [Fact]
        public void PackagePost_CheckForFileType()
        {

            var MockRequest = new Mock<HttpRequestBase>();

            MockRequest.Setup(x => x.Form.Get("Name")).Returns("Package Name");
            MockRequest.Setup(x => x.Files.Count).Returns(1);
            MockRequest.Setup(x => x.Files[0].ContentLength).Returns(900000);
            MockRequest.Setup(x => x.Files[0].ContentType).Returns("application/json");

          

            var pc = new PackageController(null, null, null , MockRequest.Object);

            var res = pc.Post();
            Assert.IsType<FailedResponse>(res);
        }


        [Fact]
        public void PackagePost_CheckForFileSize()
        {

            var MockRequest = new Mock<HttpRequestBase>();

            MockRequest.Setup(x => x.Form.Get("Name")).Returns("Package Name");
            MockRequest.Setup(x => x.Files.Count).Returns(1);
            MockRequest.Setup(x => x.Files[0].ContentLength).Returns(1500000);


            var pc = new PackageController(null, null , null , MockRequest.Object);

            var res =  pc.Post();

            Assert.IsType<FailedResponse>(res);
        }

        [Fact]
        public void PackagePost_CheckForFilesCount()
        {

            var MockRequest = new Mock<HttpRequestBase>();

            MockRequest.Setup(x => x.Form.Get("Name")).Returns("Package Name");
            MockRequest.Setup(x => x.Files.Count).Returns(0);


            var pc = new PackageController(null , null , null, MockRequest.Object);

            var res =  pc.Post();

            Assert.IsType<FailedResponse>(res);
        }

        [Fact]
        public void PackagePost_CheckForName()
        {

            var MockRequest = new Mock<HttpRequestBase>();

            MockRequest.Setup(x => x.Form.Get("Name")).Returns(string.Empty);
            
            var pc = new PackageController(null, null , null, MockRequest.Object);

            var res =  pc.Post();

            Assert.IsType<FailedResponse>(res);
        }




    }
}
