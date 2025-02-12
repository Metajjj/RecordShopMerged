using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

using RecordShop_BE;

namespace TestApp
{
    public class RepoTests
    {
        MyDbContext context;
        AlbumRepository core;

        [SetUp]
        public void Setup()
        {
            var mock = new Mock <IWebHostEnvironment> ();
            mock.Setup(m => m.EnvironmentName).Returns("Development");

            context = new (mock.Object);

            core = new (context);
        }

        [TearDown]
        public void TearDown()
        {
            context.Database.EnsureDeleted();
            //context.Dispose(); //Fails to create new if not manually disposed
        }

        [Test]
        public void GetAllAlbums_Success()
        {
            context.Add<Albums>(new Albums { Id = 2, Title = "Y" });
            context.SaveChanges();

            var res = core.GetAllAlbums().First();

            Assert.That(res.Id, Is.EqualTo(2));
            Assert.That(res.Title, Is.EqualTo("Y"));
            Assert.That(core.GetAllAlbums().Count == 1);
        }

        [Test]
        public void GetAlbumById()
        {
            context.Add<Albums>(new Albums { Id = 2, Title = "Y" });
            context.SaveChanges();

            var res = core.GetAlbumById(2);
            var res2 = core.GetAlbumById(1);

            Assert.That(res.Title, Is.EqualTo("Y"));
            Assert.That(res2, Is.Null);
        }

        [Test]
        public void PostAlbum()
        {
            //Testing auto-increments!

            context.AlbumTable.AddRange(new Albums { Id = 2, Title = "X" }, new Albums { Id=5, Title="X"});
            context.SaveChanges();

            core.PostAlbum(new Albums { Title = "T1" });

            core.PostAlbum(new Albums { Id = 4, Title = "T3" });
            core.PostAlbum(new Albums { Id = 3, Title = "T2" });

            var res = core.GetAllAlbums();

            Assert.Multiple(() =>
            {
                Assert.That(res.First().Id, Is.EqualTo(1));
                Assert.That(res.First().Title, Is.EqualTo("T1"));
            });

                //Ensure that user-specified IDs do not affect the db's assignment of IDs
            Assert.That(res.Where(a => a.Id == 3).First().Title, Is.EqualTo("T3"));
            Assert.That(res.Where(a => a.Id == 4).First().Title, Is.EqualTo("T2"));
        }

        [Test]
        public void PutAlbum()
        {
            context.AlbumTable.Add(new Albums { Id = 6, Title="A" }); context.SaveChanges();

            Assert.Throws<ArgumentNullException>(()=>core.PutAlbum(new Albums { Id = 1 }));
                //Entry doesn't exist

            Assert.That(core.PutAlbum(new Albums { Id = 6, Title = "A" }), Is.False);
                //Entry doesnt change

            Assert.That(core.PutAlbum(new Albums { Id = 6, Title = "B" }), Is.True);
                //Entry changes
        }

        [Test]
        public void DeleteAlbum()
        {
            context.AlbumTable.Add(new Albums { Id = 2, Title="Z" }); context.SaveChanges();

            Assert.Throws<ArgumentNullException>(()=>core.DeleteAlbumById(1));

            Assert.That(core.DeleteAlbumById(2), Is.Not.Null);
        }
    }
}
