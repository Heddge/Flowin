
using System.Net.Sockets;
using Microsoft.EntityFrameworkCore;
using NotesFlow.server.Data;
using Microsoft.AspNetCore.Identity;
using NotesFlow.server.Objects;

namespace NotesFlow.server
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var connect = builder.Configuration.GetConnectionString("NotesDatabase");
            builder.Services.AddDbContext<NotesDbContext>(s => s.UseNpgsql(connect));
            builder.Services.AddIdentityCore<User>().AddEntityFrameworkStores<NotesDbContext>();//указание Identity хранить пользователей через EFcore DB context для PostgreSQL



            var app = builder.Build();

            // test connecting to PostgreSQL and EFCore 
            //Note n = new Note(Guid.NewGuid(), "hellllll", "lmao");
            var context = app.Services.CreateScope();
            using var fact = context.ServiceProvider.GetRequiredService<NotesDbContext>();
            using var fact2 = context.ServiceProvider.GetRequiredService<UserManager<User>>();

            //User us = new User("Egor", "Test");
            //us.Email = "egor@test.ru";
            //us.UserName = "egorTest";
            //var salam = await fact2.CreateAsync(us, "Testpassword123456-");
            //System.Diagnostics.Debug.WriteLine("FINDED PERSON BY EMAIL -> " + salam.Succeeded);
            //foreach (var err in salam.Errors)
            //    System.Diagnostics.Debug.WriteLine("FINDED PERSON BY EMAIL -> " + err.Code + err.Description);

            //var a = await fact2.FindByEmailAsync("egor@test.ru");
            //System.Diagnostics.Debug.WriteLine("FINDED PERSON BY EMAIL -> " + a?.Name + "213");

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
