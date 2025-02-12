using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;

using RecordShop_BE.Tables;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.FileProviders;
using System.Diagnostics;

namespace RecordShop_BE
{
    public class MyDbContext : DbContext
    {
        private IWebHostEnvironment host;

        //for testing
        public MyDbContext(IWebHostEnvironment env)
        { host = env; }

        public MyDbContext(DbContextOptions<MyDbContext> o) : base(o) { }
            
        public MyDbContext(DbContextOptions<MyDbContext> opt, IWebHostEnvironment env) : base(opt) { host = env;
            Database.EnsureCreated(); //Need to run for mem DB to work + populate!
        }

        public DbSet<Albums> AlbumTable { get; set; }

        //TODO migration doesnt like detecting memDB
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //Check if in development mode ?? run inmemory vs SQL -- this one overwrites, no need for constructor context!
            if (host.IsDevelopment())
            {   optionsBuilder.UseInMemoryDatabase("TempDB");

                //Alternative for OnModelCreate? EF 9.0 - doesnt work during dev website env
                //Runs during EnsureCreated()
                //optionsBuilder.UseSeeding((context, _) => { context.Set<Albums>().Add(new Albums { Id = 2, Title = "ABC" }); });
            }
            else if (host.IsProduction())
            {
                //throw new Exception("YELLOW"); //Works when run on .exe
                optionsBuilder.UseSqlServer($"Server={Secret.Server};Database={Secret.Database};User Id={Secret.User};Password={Secret.Password};Trust Server Certificate=True");
            }
            else { throw new NotImplementedException("Unexpected environment!"); }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        { modelBuilder.Entity<Albums>().HasData(new Albums { Id = 2, Title = "ABC" });  }
    }

    public class E : IWebHostEnvironment
    {
        public string WebRootPath { get; set; } = "";
        public IFileProvider WebRootFileProvider { get; set; } = null;
        public string ApplicationName { get; set; } = "";
        public IFileProvider ContentRootFileProvider { get; set; } = null;
        public string ContentRootPath { get; set; } = "";
        public string EnvironmentName { get; set; } = "";
    }
    public class ContextFactory : IDesignTimeDbContextFactory<MyDbContext>
    {
        public MyDbContext CreateDbContext(string[] args)
        {
                //Force into prod which has SQL when making migration updates
            var c = new MyDbContext(new E() { EnvironmentName = "Production" });
            return c;
        }
    }

}
