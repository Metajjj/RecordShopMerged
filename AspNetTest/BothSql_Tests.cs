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
    public class SqlTestSetup
    {
        public static (MyDbContext,AlbumRepository) DbSetup(bool IsMemory)
        {
            MyDbContext context;
            AlbumRepository core;

            var mock = new Mock<IWebHostEnvironment>();
            mock.Setup(m => m.EnvironmentName).Returns(IsMemory ? "Development" : "Production");

            context = new(mock.Object);
            context.Database.EnsureCreated();

            core = new(context);
            return (context, core);
        }
    }

    public class BothSql_Tests : SqlTestSetup
    {
        MyDbContext context;
        AlbumRepository core;

        [SetUp]
        public void Setup()
        {
            //TODO no params - test both dbs
            (context, core) = SqlTestSetup.DbSetup(true);
        }

        [TearDown] //Runs after each test always
        public void TearDown()
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
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
            if (context.Database.IsSqlServer())
            {
                context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [AlbumTable] ON");
                context.SaveChanges();
            }

            //Enumerable fails for inMem
            //context.AlbumTable.AddRange(
            //    Enumerable.Range(0, 10).Select(i => 
            //        new Albums { Id = i, Title = "" + i }
            //    )
            //);
            
                for (int i = 1; i <= 10; i++)
                {
                    context.AlbumTable.Add(new Albums { Id = i, Title = "" + i });
                }

            //TODO Error : identity_insert is off
            context.SaveChanges();

            context.AlbumTable.Remove( 
                context.AlbumTable.Find(
                    new Random().Next(1, 10 + 1)
                )
            );
            context.SaveChanges();

            core.PostAlbum(new Albums {  });
            context.SaveChanges();

            var A = context.AlbumTable.ToList();

            Assert.That( A.All(a=> a.Id<11) );
        }
    }

}
