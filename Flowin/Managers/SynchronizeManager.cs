using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flowin.Interfaces;
using Flowin.Objects;

namespace Flowin.Managers
{
    public class SynchronizeManager : ISynchronizeManager
    {
        private INoteDbManager _noteDbManager;
        private INotesApiManager _notesApiManager;
        private SemaphoreSlim _semaphoreSlim;

        public SynchronizeManager(INoteDbManager ndm, INotesApiManager nam)
        {
            _noteDbManager = ndm;
            _notesApiManager = nam;
            _semaphoreSlim = new SemaphoreSlim(1);
        }

        /// <summary>
        /// Deleting Deleted in SQL notes in PostgreSQL
        /// </summary>
        /// <returns>Asyncronus work.</returns>
        private async Task SynchronizeDeleted()
        {
            foreach (SyncStatus toSync in _noteDbManager.GetDeletedStatuses())
                if (await _notesApiManager.DeleteNote(toSync.NoteId))
                    _noteDbManager.DeleteSyncStatus(toSync.NoteId);
        }

        /// <summary>
        /// Comparing notes w syncstatus Updated with notes in PostgreSQL. Newer are donwloading to needed DB
        /// </summary>
        /// <returns></returns>
        private async Task SynchronizeUpdated()
        {
            Dictionary<Guid, DateTime>? postgrToUpd = await _notesApiManager.GetIdUpdateAt();
            List<SyncStatus> updSyncStatuses = _noteDbManager.GetUpdatedStatuses();

            if (postgrToUpd != null)
                foreach (SyncStatus toSync in updSyncStatuses)
                {
                    var local_note = _noteDbManager.GetNote(toSync.NoteId);
                    if (postgrToUpd.ContainsKey(toSync.NoteId) && local_note != null)
                    {
                        var pgsql_updat_note = postgrToUpd[toSync.NoteId];
                        if (local_note.UpdatedAt < pgsql_updat_note)
                        {
                            var request = await _notesApiManager.GetNote(toSync.NoteId);
                            if (request.data != null)
                            {
                                _noteDbManager.UpdateNote(request.data);
                                _noteDbManager.DeleteSyncStatus(toSync.NoteId);
                            }
                        }
                        else if (local_note.UpdatedAt > pgsql_updat_note)
                        {
                            if (await _notesApiManager.UpdateNote(local_note) != null)
                                _noteDbManager.DeleteSyncStatus(toSync.NoteId);
                        }
                        else
                        {
                            var response = await _notesApiManager.GetNote(local_note.Id);
                            if (local_note.Content.Length > response?.data?.Content.Length)
                            {
                                if (await _notesApiManager.UpdateNote(local_note) != null)
                                    _noteDbManager.DeleteSyncStatus(toSync.NoteId);
                            }
                            else
                            {
                                var request = await _notesApiManager.GetNote(toSync.NoteId);
                                if (request.data != null)
                                {
                                    _noteDbManager.UpdateNote(request.data);
                                    _noteDbManager.DeleteSyncStatus(toSync.NoteId);
                                }
                            }
                        }

                    }
                }
        }

        /// <summary>
        /// Comparing all PostgreSQL notes with SQL notes.
        /// </summary>
        /// <returns></returns>
        private async Task SynchronizeAll()
        {
            Dictionary<Guid, DateTime>? postgrToUpd = await _notesApiManager.GetIdUpdateAt();
            if (postgrToUpd == null)
                return;
            var local_notes = _noteDbManager.GetNotes();
            foreach (Note n in local_notes)
            {
                if (postgrToUpd.ContainsKey(n.Id))
                {
                    if (n.UpdatedAt < postgrToUpd[n.Id])
                    {
                        var request = await _notesApiManager.GetNote(n.Id);
                        if (request.data != null) _noteDbManager.UpdateNote(request.data);
                    }
                    if (n.UpdatedAt > postgrToUpd[n.Id])
                    {
                        Note? request = _noteDbManager.GetNote(n.Id);
                        if (request != null) await _notesApiManager.UpdateNote(request);
                    }
                }
            }
            foreach (KeyValuePair<Guid, DateTime> updnote in postgrToUpd)
            {
                Note? response = local_notes.FirstOrDefault(x => x.Id == updnote.Key);
                if (response != null)
                {
                    if (updnote.Value > response.UpdatedAt)
                    {
                        var request = await _notesApiManager.GetNote(updnote.Key);
                        if (request.data != null) _noteDbManager.UpdateNote(request.data);
                    }
                    if (updnote.Value < response.UpdatedAt)
                    {
                        Note? request = _noteDbManager.GetNote(updnote.Key);
                        if (request != null) await _notesApiManager.UpdateNote(request);
                    }
                }
                else
                {
                    var request = await _notesApiManager.GetNote(updnote.Key);
                    if (request.data != null) _noteDbManager.AddNote(request.data);
                }
            }
        }

        /// <summary>
        /// Posting Created in SQL notes to PostgreSQL
        /// </summary>
        /// <returns></returns>
        private async Task SynchronizeCreated()
        {
            foreach (SyncStatus toSync in _noteDbManager.GetCreatedStatuses())
            {
                Note? note = _noteDbManager.GetNote(toSync.NoteId);
                if (note != null)
                {
                    note.CreatedAt = DateTime.SpecifyKind(note.CreatedAt, DateTimeKind.Utc);
                    note.UpdatedAt = DateTime.SpecifyKind(note.UpdatedAt, DateTimeKind.Utc);

                    if (await _notesApiManager.SaveNote(note) != null)
                        _noteDbManager.DeleteSyncStatus(toSync.NoteId);
                }
            }
        }

        /// <summary>
        /// Main app`s synchronization method
        /// </summary>
        /// <returns></returns>
        public async Task Synchronize()
        {
            try
            {
                await _semaphoreSlim.WaitAsync();
                await SynchronizeDeleted().ConfigureAwait(false);
                await SynchronizeUpdated().ConfigureAwait(false);
                await SynchronizeAll().ConfigureAwait(false);
                await SynchronizeCreated().ConfigureAwait(false);
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public List<Note> GetNotes()
            => _noteDbManager.GetNotes();

        public List<Note> GetNotes(User curr_user)
            => _noteDbManager.GetNotes(curr_user);


        public async Task<bool> SaveNote(Note note)
        {
            _noteDbManager.AddNote(note);
            _noteDbManager.AddSyncStatus(new SyncStatus(note.Id,Guid.Empty, 1));

            if (await _notesApiManager.SaveNote(note) != null)
            {
                _noteDbManager.DeleteSyncStatus(note.Id);
                return true;
            }

            return false;
        }

        public async Task<bool> SaveNote(User curr_user, Note note)
        {
            _noteDbManager.AddNote(note);
            _noteDbManager.AddSyncStatus(new SyncStatus(note.Id, curr_user.Id, 1));

            if (await _notesApiManager.SaveNote(note) != null)
            {
                _noteDbManager.DeleteSyncStatus(note.Id);
                return true;
            }

            return false;
        }


        public async Task<bool> UpdateNote(Note note)
        {
            _noteDbManager.UpdateNote(note);
            if (!_noteDbManager.IsThereAnyStatus(note.Id))
                _noteDbManager.AddSyncStatus(new SyncStatus(note.Id, Guid.Empty, 2));


            if (await _notesApiManager.UpdateNote(note) != null)
            {
                _noteDbManager.DeleteSyncStatus(note.Id);
                return true;
            }

            return false;
        }

        public async Task<bool> UpdateNote(User curr_user, Note note)
        {
            _noteDbManager.UpdateNote(note);
            if (!_noteDbManager.IsThereAnyStatus(note.Id))
                _noteDbManager.AddSyncStatus(new SyncStatus(note.Id, curr_user.Id, 2));


            if (await _notesApiManager.UpdateNote(note) != null)
            {
                _noteDbManager.DeleteSyncStatus(note.Id);
                return true;
            }

            return false;
        }

        public async Task<bool> DeleteNote(Note n)
        {
            _noteDbManager.DeleteNote(n);

            if (!_noteDbManager.IsThereAnyStatus(n.Id))
                _noteDbManager.AddSyncStatus(new SyncStatus(n.Id, Guid.Empty, 3));
            else
            {
                if (_noteDbManager.GetSyncStatus(n.Id)?.Status == OperationStatus.Created)
                {
                    _noteDbManager.DeleteSyncStatus(n.Id);
                    return true;
                }
                else
                    _noteDbManager.UpdateSyncStatus(new SyncStatus(n.Id, Guid.Empty, 3));
            }

            if (await _notesApiManager.DeleteNote(n.Id))
                _noteDbManager.DeleteSyncStatus(n.Id);
            else
                return false;

            return true;
        }

        public async Task<bool> DeleteNote(User curr_user, Note n)
        {
            _noteDbManager.DeleteNote(n);

            if (!_noteDbManager.IsThereAnyStatus(n.Id))
                _noteDbManager.AddSyncStatus(new SyncStatus(n.Id, curr_user.Id, 3));
            else
            {
                if (_noteDbManager.GetSyncStatus(n.Id)?.Status == OperationStatus.Created)
                {
                    _noteDbManager.DeleteSyncStatus(n.Id);
                    return true;
                }
                else
                    _noteDbManager.UpdateSyncStatus(new SyncStatus(n.Id, curr_user.Id, 3));
            }

            if (await _notesApiManager.DeleteNote(n.Id))
                _noteDbManager.DeleteSyncStatus(n.Id);
            else
                return false;

            return true;
        }
    }
}
