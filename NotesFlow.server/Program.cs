
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Reflection;
//using Microsoft.OpenApi.Models;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NotesFlow.server.Data;
using NotesFlow.server.Objects;

namespace NotesFlow.server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var connect = builder.Configuration.GetConnectionString("NotesFlow");
            builder.Services.AddDbContext<NotesDbContext>(s => s.UseNpgsql(connect));





            var app = builder.Build();

            // test connecting to PostgreSQL and EFCore 
            //Note n = new Note(Guid.NewGuid(), "hellllll", "lmao");
            var context = app.Services.CreateScope();
            using var fact = context.ServiceProvider.GetRequiredService<NotesDbContext>();

            try
            {
                fact.Database.EnsureCreated();
            }
            catch (SocketException exsocket)
            {
                System.Diagnostics.Debug.WriteLine("404 Postgres Connetion Error. .Database.EnsureCreated()");
                System.Diagnostics.Debug.WriteLine(exsocket.Message);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("404 Postgres Connetion Error. .Database.EnsureCreated()");
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            //fact.Notes.Add(n);
            //fact.SaveChanges();



            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();


            app.Run();
        }
    }
}
