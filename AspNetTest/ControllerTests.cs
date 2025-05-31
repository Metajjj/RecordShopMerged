global using RecordShop_BE.Controllers;
global using RecordShop_BE.Services;
global using RecordShop_BE.Repositories;
global using RecordShop_BE.Tables;
global using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace TestApp
{
    public class ControllerTests
    {
        Mock<IAlbumService> mock;
        AlbumController core;

        [SetUp]
        public void Setup()
        {
            mock = new ();

            core = new (mock.Object);
        }

        [Test]
        public void GetAllAlbums_Success()
        {
            //ARRANGE
            mock.Setup(
                ms => ms.GetAllAlbums()).Returns(
                [new Albums { Title = "AC/DC", Id = 13 }]
            );

            //ACT

            var result = core.GetallAlbums();

            //ASSERT

            Assert.That(result, Is.TypeOf<OkObjectResult>());
        }

        [Test]
        public void GetAlbumById()
        {
            mock.Setup(ms => ms.GetAlbumById(
                It.IsAny<string>() ))
                .Returns(new Albums { Title = "AC/DC", Id = 13 });

            mock.Setup(ms => ms.GetAlbumById("1")).Returns<Albums>(null);

            mock.Setup(ms => ms.GetAlbumById("a")).Throws<FormatException>();

            Assert.Multiple(
                () =>
                {
                    var res = core.GetAlbumById("13");
                    Assert.That(res, Is.TypeOf<OkObjectResult>());

                    res = core.GetAlbumById("1");
                    Assert.That(res, Is.TypeOf<NotFoundObjectResult>());

                    res = core.GetAlbumById("a");
                    Assert.That(res, Is.TypeOf<BadRequestObjectResult>());
                }                
            );
        }

        [Test]
        public void PostAlbum()
        {
            mock.Setup(m => m.PostAlbum(It.IsAny<Albums>())).Returns( new Albums { Id = 3 });

            var res = core.PostAlbum(new Albums { Id=3 });

            Assert.That(res, Is.TypeOf<OkObjectResult>());
        }

        [Test]
        public void PutAlbumAsync()
        {
            mock.Setup(m => m.PutAlbum(It.IsAny<Albums>())).Returns(true);

            //var htpc = new DefaultHttpContext { };

            //using (var mem = new MemoryStream()) //Have to keep till not used anymore!
            //{
            //    mem.Write(
            //        JsonSerializer.Serialize(new Albums()).Select(Convert.ToByte).ToArray()
            //    );
            //    mem.Position = 0;
            //    htpc.Request.Body = mem; 
            // }

            //        //make re-readable

            //Err no json token when deserializing ??
            var res = core.PutAlbum(new Albums());
            Assert.That(res, Is.TypeOf<OkObjectResult>());  //Expected


            res = core.PutAlbum(new Albums());
            //Assert.That(res, Is.TypeOf<BadRequestObjectResult>()); //Wrong format
            //TODO Cannot test wrong format! HTPC => Albums


            mock.Setup(m => m.PutAlbum(It.IsAny<Albums>())).Returns(false); //No changes made
            res = core.PutAlbum(new Albums());
            Assert.That(res, Is.TypeOf<AcceptedResult>());


            mock.Setup(m => m.PutAlbum(It.IsAny<Albums>())).Throws<ArgumentNullException>(); //Could not find
            res = core.PutAlbum(new Albums());
            Assert.That(res, Is.TypeOf<NotFoundObjectResult>());

        }

        [Test]
        public void DeleteAlbum()
        {
            //TODO delete album test
            mock.Setup(ms => ms.DeleteAlbumById(
               It.IsAny<string>()))
               .Returns(new Albums { Title = "AC/DC", Id = 13 });

            mock.Setup(ms => ms.DeleteAlbumById("1")).Returns<Albums>(null);

            mock.Setup(ms => ms.DeleteAlbumById("a")).Throws<FormatException>();

            Assert.Multiple(
                () =>
                {
                    var res = core.DeleteAlbumById("13");
                    Assert.That(res, Is.TypeOf<OkObjectResult>());

                    res = core.DeleteAlbumById("1");
                    Assert.That(res, Is.TypeOf<NotFoundObjectResult>());

                    res = core.DeleteAlbumById("a");
                    Assert.That(res, Is.TypeOf<BadRequestObjectResult>());
                }
            );
        }
    }

}
