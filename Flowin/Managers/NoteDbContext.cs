using Microsoft.EntityFrameworkCore;
using Flowin.Objects;

namespace Flowin.Managers
{
    public class NoteDbContext : DbContext
    {
        public DbSet<Note> Notes { get; set; }
        public DbSet<SyncStatus> SyncStatuses { get; set; }
        public NoteDbContext(DbContextOptions<NoteDbContext> options) : base(options) { }
    }
}
