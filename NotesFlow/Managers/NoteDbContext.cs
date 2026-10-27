using Microsoft.EntityFrameworkCore;
using NotesFlow.Objects;

namespace NotesFlow.Managers
{
    public class NoteDbContext : DbContext
    {
        public DbSet<Note> Notes { get; set; }
        public DbSet<SyncStatus> SyncStatuses { get; set; }

        public NoteDbContext(DbContextOptions<NoteDbContext> options) : base(options) { }
    }
}
