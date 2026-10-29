using Microsoft.EntityFrameworkCore;
using Flowin.server.Objects;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;


namespace Flowin.server.Data
{
    public class NotesDbContext : IdentityUserContext<User, Guid>
    {
        public DbSet<Note> Notes { get; set; }
        public NotesDbContext(DbContextOptions<NotesDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder mb)
        {
            mb.Entity<Note>().HasOne<User>().WithMany().HasForeignKey("OwnerId");
            mb.Entity<User>().HasKey("Id");
            base.OnModelCreating(mb);
        }
    }
}
