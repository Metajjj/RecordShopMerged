using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp
{
    public class ServiceTests
    {
        Mock<IAlbumRepository> mock;
        AlbumService core;

        [SetUp]
        public void Setup()
        {
            mock = new ();

            core = new (mock.Object);
        }

        [Test]
        public void GetAllAlbums_Success()
        {
            mock.Setup(
                ms => ms.GetAllAlbums()).Returns(
                [new Albums { Title = "AC/DC", Id = 13 }]
            );


            var result = core.GetAllAlbums();


            Assert.That(result, Is.TypeOf<List<Albums>>());
            Assert.That(result.Count > 0);
        }

        [Test]
        public void GetAlbumById()
        {
            mock.Setup(ms => ms.GetAlbumById(It.IsAny<int>()))
                .Returns<int>( param => new Albums { Id = param, Title = "TestingService" } );


            var res = core.GetAlbumById("1");


            Assert.That(res.Id,Is.EqualTo(1));

            Assert.Throws<FormatException>(()=>core.GetAlbumById("a"));
        }

        [Test]
        public void PostAlbum()
        {
            mock.Setup(m => m.PostAlbum(It.IsAny<Albums>())).Returns(new Albums());

            core.PostAlbum(new Albums());

            mock.Verify(m=>m.PostAlbum(It.IsAny<Albums>()), Times.Once());
                //Make sure doesnt try to add the same album more than once
        }

        [Test]
        public void PutAlbum()
        {
            mock.Setup(m=>m.PutAlbum(It.IsAny<Albums>())).Returns(true);

            core.PutAlbum(new Albums());

            mock.Verify(m => m.PutAlbum(It.IsAny<Albums>()), Times.Once());
        }
    }
}
