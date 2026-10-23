using Microsoft.EntityFrameworkCore;
using NotesFlow.server.Objects;

namespace NotesFlow.server.Data
{
    public class NotesDbContext : DbContext
    {
        public DbSet<Note> Notes { get; set; }
        public NotesDbContext(DbContextOptions<NotesDbContext> options) : base(options) { }
    }
}
