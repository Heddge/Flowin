using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Flowin.Interfaces;
using Flowin.Objects;

namespace Flowin.Managers
{
    /// <summary>
    /// Менеджер общения NoteContainer (UI) и SQLite.
    /// </summary>
    public class NoteDbManager : INoteDbManager
    {
        IDbContextFactory<NoteDbContext> _contextFactory;
        public NoteDbManager(IDbContextFactory<NoteDbContext> context)
        {
            System.Diagnostics.Debug.WriteLine("notesdbmaanager constructor entered");
            _contextFactory = context;
        }

        // TO WORK WITH NOTES IN SQLITE
        public bool AddNote(Note n)
        {
            using var factory = _contextFactory.CreateDbContext();
            factory.Notes.Add(n);
            return factory.SaveChanges() == 1;
        }

        //public bool AddNote(User curr_user, Note n)
        //{
        //    using var factory = _contextFactory.CreateDbContext();
        //    factory.Notes.Add(n);
        //    return factory.SaveChanges() == 1;
        //}

        public Note? GetNote(Guid id)
        {
            using var factory = _contextFactory.CreateDbContext();
            return factory.Notes.Find(id);
        }

        public List<Note> GetNotes()
        {
            List<Note> result = new List<Note>();
            using var factory = _contextFactory.CreateDbContext();
            foreach (var note in factory.Notes.ToList())
            {
                note.CreatedAt = DateTime.SpecifyKind(note.CreatedAt, DateTimeKind.Utc);
                note.UpdatedAt = DateTime.SpecifyKind(note.UpdatedAt, DateTimeKind.Utc);
                result.Add(note);
            }

            return result;
        }

        public List<Note> GetNotes(User curr_user)
        {
            List<Note> result = new List<Note>();
            using var factory = _contextFactory.CreateDbContext();

            var Id = curr_user.Id;

            foreach (var note in factory.Notes.Where(x => x.OwnerId == Id).ToList())
            {
                note.CreatedAt = DateTime.SpecifyKind(note.CreatedAt, DateTimeKind.Utc);
                note.UpdatedAt = DateTime.SpecifyKind(note.UpdatedAt, DateTimeKind.Utc);
                result.Add(note);
            }

            return result;
        }


        public bool UpdateNote(Note n)
        {
            using var factory = _contextFactory.CreateDbContext();
            factory.Notes.Update(n);
            return factory.SaveChanges() == 1;
        }

        public bool DeleteNote(Note n)
        {
            try
            {
                using var factory = _contextFactory.CreateDbContext();
                factory.Notes.Remove(n);
                return factory.SaveChanges() == 1;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Exception in NoteDbManager.DeleteNote(Note)");
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return false;
            }
        }

        // TO WORK WITH SYNCSTATUS TABLE IN SQLITE

        public bool IsSyncStatusesEmpty()
        {
            using var factory = _contextFactory.CreateDbContext();
            return factory.SyncStatuses.Count() == 0;
        }

        public List<SyncStatus> GetDeletedStatuses()
        {
            using var factory = _contextFactory.CreateDbContext();
            return factory.SyncStatuses.Where(x => x.Status == OperationStatus.Deleted).ToList<SyncStatus>();
        }
        public List<SyncStatus> GetUpdatedStatuses()
        {
            using var factory = _contextFactory.CreateDbContext();
            return factory.SyncStatuses.Where(x => x.Status == OperationStatus.Updated).ToList<SyncStatus>();
        }

        public List<SyncStatus> GetCreatedStatuses()
        {
            using var factory = _contextFactory.CreateDbContext();
            return factory.SyncStatuses.Where(x => x.Status == OperationStatus.Created).ToList<SyncStatus>();
        }

        public List<SyncStatus> GetAllSyncStatuses()
        {
            using var factory = _contextFactory.CreateDbContext();
            return factory.SyncStatuses.ToList();
        }

        public bool IsThereAnyStatus(Guid id)
        {
            using var factory = _contextFactory.CreateDbContext();
            return factory.SyncStatuses.Find(id) != null;
        }

        public SyncStatus? GetSyncStatus(Guid id)
        {
            using var factory = _contextFactory.CreateDbContext();
            SyncStatus? status = factory.SyncStatuses.Find(id);
            return status;
        }

        public bool AddSyncStatus(SyncStatus status)
        {
            using var factory = _contextFactory.CreateDbContext();
            factory.SyncStatuses.Add(status);
            return factory.SaveChanges() == 1;
        }
        public bool UpdateSyncStatus(SyncStatus status)
        {
            using var factory = _contextFactory.CreateDbContext();
            var n = factory.SyncStatuses.Find(status.NoteId);
            if (n != null)
                n.Status = status.Status;
            return factory.SaveChanges() == 1;
        }

        public bool DeleteSyncStatus(Guid id)
        {
            using var factory = _contextFactory.CreateDbContext();
            var n = factory.SyncStatuses.Find(id);
            if (n != null)
                factory.SyncStatuses.Remove(n);
            return factory.SaveChanges() == 1;
        }
    }
}
