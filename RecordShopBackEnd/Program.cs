
using System.Runtime.CompilerServices;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using RecordShop_BE.Repositories;
using RecordShop_BE.Services;
using RecordShop_BE.Tables;

namespace RecordShop_BE
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<MyDbContext>();

                //Dependency inject to create elsewhere - helps with migration when DE for DB

            builder.Services.AddScoped<IAlbumRepository, AlbumRepository>();
            builder.Services.AddScoped<IAlbumService, AlbumService>();

            //builder.Services.AddCors(); //Allow cross-origin access??

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            //Console.WriteLine("NAME: "+db.Database.ProviderName);

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            
            app.Run();
        }
    }
}
