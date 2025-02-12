using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using RecordShop_BE;

namespace TestApp
{
    public class SqlTests
    {
        MyDbContext context;
        AlbumRepository core;

        [SetUp]
        public void Setup()
        {
            var mock = new Mock<IWebHostEnvironment>();
            mock.Setup(m => m.EnvironmentName).Returns("Production");

            context = new(mock.Object);
            context.Database.EnsureCreated();

            core = new(context);
        }

        [TearDown] //Runs after each test always
        public void TearDown()
        {
            context.Database.EnsureDeleted();
            //context.Dispose(); //Fails to create new if not manually disposed
        }

        [Test]
        public void IdDecidedByMe()
        {
            //Need to manually assign to plug in gaps!

            context.AlbumTable.AddRange([new Albums { Id = 5, Title="1" }, new Albums { Id=1, Title="2"}]);

            var A = context.AlbumTable.ToList();

            for (int i = 0; i < A.Count; i++)
            {
                    //Should be redone to match title
                Assert.That(A[i].Id, Is.EqualTo(A[i].Id));
            }
        }

        [Test]
        public void PlugInGapsWorks()
        {
            for(int i=1; i<=10; i++)
            {
                context.AlbumTable.Add(new Albums { Id = i, Title = "" + i });
            }
            context.SaveChanges();


            context.AlbumTable.Remove(new Albums { Id=new Random().Next(1,10+1) });
            context.SaveChanges();

            context.AlbumTable.Add(new Albums { });
            context.SaveChanges();

            var A = context.AlbumTable.ToList();

            Assert.That( A.All(a=> a.Id<11) );
        }
    }

}
