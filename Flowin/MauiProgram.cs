using Microsoft.Extensions.Logging;
using Flowin.Objects;
using Flowin.Managers;
using Flowin.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using JetBrains.Annotations;
using System.Diagnostics;
using Microsoft.Extensions.Http;
using Microsoft.AspNetCore.Identity;

namespace Flowin
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            string _dbpath = Path.Combine(FileSystem.AppDataDirectory, "Flowindb.db");
            System.Diagnostics.Debug.WriteLine(_dbpath + " ASHALA");
            builder.Services.AddMauiBlazorWebView();
            builder.Services.AddSingleton<NotesContainer>();
            builder.Services.AddSingleton<LocalSessionContainer>();
            builder.Services.AddSingleton<SemaphoreSlim>();
            //builder.Services.AddSingleton<JsonParserManager>();
            //builder.Services.AddSingleton<AppStorageManager>();
            builder.Services.AddSingleton<ISynchronizeManager, SynchronizeManager>();
            builder.Services.AddSingleton<INoteDbManager, NoteDbManager>();
            //builder.Services.AddDbContext<NoteDbContext>(options => options.UseSqlite("Data Source="+_dbpath));
            builder.Services.AddPooledDbContextFactory<NoteDbContext>(options => options.UseSqlite("Data Source=" + _dbpath));
            builder.Services.AddHttpClient<INotesApiManager, NotesApiManager>();
            builder.Services.AddSingleton<AuthenticationManager>();
            builder.Services.AddSingleton<UserSessionManager>();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            var app = builder.Build();

            using var scope = app.Services.CreateScope();
            var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<NoteDbContext>>();
            using var context = factory.CreateDbContext();
            context.Database.EnsureCreated();

            System.Diagnostics.Debug.WriteLine("entered maui program");


            try
            {
                System.Diagnostics.Debug.WriteLine("notescontainer creating");
                var n = app.Services.GetRequiredService<NotesContainer>();
                var l = app.Services.GetRequiredService<LocalSessionContainer>();
                var ls = app.Services.GetRequiredService<AuthenticationManager>();
                System.Diagnostics.Debug.WriteLine("notescontainer created");

            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine(ex.Message);
                throw;
            }
            context.Database.ExecuteSqlRaw("""
    CREATE TABLE IF NOT EXISTS SyncStatuses (
        NoteId TEXT NOT NULL PRIMARY KEY,
        Status INTEGER NOT NULL
    );
    """);

            return app;
        }
    }
}
