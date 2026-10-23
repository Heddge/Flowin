using NotesFlow.Managers;
using NotesFlow.Objects;

namespace NotesFlow.Interfaces
{
    /// <summary>
    /// An interface for local db manager
    /// </summary>
    public interface INoteDbManager
    {
        // TO WORK WITH NOTES IN SQLITE
        public bool AddNote(Note n);

        public Note? GetNote(Guid id);

        public List<Note> GetNotes();
        public bool UpdateNote(Note n);
        public bool DeleteNote(Note n);

        // TO WORK WITH SYNCSTATUS TABLE IN SQLITE

        public bool IsSyncStatusesEmpty();

        public List<SyncStatus> GetDeletedStatuses();
        public List<SyncStatus> GetUpdatedStatuses();

        public List<SyncStatus> GetCreatedStatuses();

        public List<SyncStatus> GetAllSyncStatuses();

        public bool IsThereAnyStatus(Guid id);

        public SyncStatus? GetSyncStatus(Guid id);

        public bool AddSyncStatus(SyncStatus status);
        public bool UpdateSyncStatus(SyncStatus status);

        public bool DeleteSyncStatus(Guid id);
    }
}
