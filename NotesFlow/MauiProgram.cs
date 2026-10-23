using Microsoft.Extensions.Logging;
using NotesFlow.Objects;
using NotesFlow.Managers;
using NotesFlow.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using JetBrains.Annotations;
using System.Diagnostics;
using Microsoft.Extensions.Http;

namespace NotesFlow
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

            string _dbpath = Path.Combine(FileSystem.AppDataDirectory, "notesflowdb.db");
            System.Diagnostics.Debug.WriteLine(_dbpath);
            builder.Services.AddMauiBlazorWebView();
            builder.Services.AddSingleton<NotesContainer>();
            //builder.Services.AddSingleton<JsonParserManager>();
            //builder.Services.AddSingleton<AppStorageManager>();
            builder.Services.AddSingleton<ISynchronizeManager, SynchronizeManager>();
            builder.Services.AddSingleton<NoteDbManager>();
            //builder.Services.AddDbContext<NoteDbContext>(options => options.UseSqlite("Data Source="+_dbpath));
            builder.Services.AddPooledDbContextFactory<NoteDbContext>(options => options.UseSqlite("Data Source=" + _dbpath));
            builder.Services.AddHttpClient<NotesApiManager>();

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
