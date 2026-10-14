using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NotesFlow.Objects;

namespace NotesFlow.Managers
{
    public class SynchronizeManager
    {
        private NoteDbManager _noteDbManager;
        private NotesApiManager _notesApiManager;

        public SynchronizeManager(NoteDbManager ndm, NotesApiManager nam)
        {

            System.Diagnostics.Debug.WriteLine("constructor synchr manager entered");

            _noteDbManager = ndm;
            _notesApiManager = nam;
        }

        public async Task SynchronizeSyncStatuses()
        {
            //if (_noteDbManager.IsSyncStatusesEmpty()) return;

            foreach (SyncStatus toSync in _noteDbManager.GetDeletedStatuses())
            {
                var response = await _notesApiManager.GetNote(toSync.NoteId);
                if (response.Status == RequestStatus.Success)
                {
                    if (response.data != null)
                        if (await _notesApiManager.DeleteNote(toSync.NoteId))
                        {
                            _noteDbManager.DeleteSyncStatus(toSync.NoteId);
                            continue;
                        }
                        else
                            continue;
                }
                if (response.Status == RequestStatus.NotFound)
                {
                    _noteDbManager.DeleteSyncStatus(toSync.NoteId);
                    continue;
                }
                continue;
            }

            Dictionary<Guid, DateTime>? postgrToUpd = await _notesApiManager.GetIdUpdateAt();
            List<SyncStatus> updSyncStatuses = _noteDbManager.GetUpdatedStatuses();

            if (postgrToUpd != null)
                foreach (SyncStatus toSync in updSyncStatuses)
                {
                    if (_noteDbManager.GetNote(toSync.NoteId)?.UpdatedAt < postgrToUpd[toSync.NoteId])
                    {
                        var request = await _notesApiManager.GetNote(toSync.NoteId);
                        if (request.data != null)
                        {
                            _noteDbManager.UpdateNote(request.data);
                            _noteDbManager.DeleteSyncStatus(toSync.NoteId);
                        }
                    }
                    else if (_noteDbManager.GetNote(toSync.NoteId)?.UpdatedAt > postgrToUpd[toSync.NoteId])
                    {
                        Note? request = _noteDbManager.GetNote(toSync.NoteId);
                        if (request != null)
                        {
                            await _notesApiManager.UpdateNote(request);
                            _noteDbManager.DeleteSyncStatus(toSync.NoteId);
                        }
                    }
                }
            postgrToUpd = await _notesApiManager.GetIdUpdateAt();
            if (postgrToUpd != null)
            {
                foreach (Note n in _noteDbManager.GetNotes())
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
                foreach (KeyValuePair<Guid, DateTime> updnote in postgrToUpd)
                {
                    Note? response = _noteDbManager.GetNote(updnote.Key);
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


            foreach (SyncStatus toSync in _noteDbManager.GetAllSyncStatuses())
            {
                switch (toSync.Status)
                {
                    case OperationStatus.Created:
                        {
                            Note? note = _noteDbManager.GetNote(toSync.NoteId);
                            note.CreatedAt = DateTime.SpecifyKind(note.CreatedAt, DateTimeKind.Utc);
                            note.UpdatedAt = DateTime.SpecifyKind(note.UpdatedAt, DateTimeKind.Utc);

                            if (note != null)
                                if (await _notesApiManager.SaveNote(note) != null)
                                {
                                    _noteDbManager.DeleteSyncStatus(toSync.NoteId);
                                }
                            break;
                        }
                    case OperationStatus.Updated:
                        {
                            Note? note = _noteDbManager.GetNote(toSync.NoteId);
                            if (note != null)
                                if (await _notesApiManager.UpdateNote(note) != null)
                                    _noteDbManager.DeleteSyncStatus(toSync.NoteId);
                            break;
                        }
                    case OperationStatus.Deleted:
                        {
                            var response = await _notesApiManager.GetNote(toSync.NoteId);
                            if (response.Status == RequestStatus.Success)
                            {
                                if (response.data != null)
                                    if (await _notesApiManager.DeleteNote(toSync.NoteId))
                                    {
                                        _noteDbManager.DeleteSyncStatus(toSync.NoteId);
                                        break;
                                    }
                                    else
                                        break;
                            }
                            if (response.Status == RequestStatus.NotFound)
                            {
                                _noteDbManager.DeleteSyncStatus(toSync.NoteId);
                                break;
                            }
                            break;
                        }
                }
            }
        }

        public List<Note> GetNotes()
        {
            return _noteDbManager.GetNotes();
        }

        public async Task<bool> SaveNote(Note note)
        {

            _noteDbManager.AddNote(note);
            _noteDbManager.AddSyncStatus(new SyncStatus(note.Id, 1));

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
                _noteDbManager.AddSyncStatus(new SyncStatus(note.Id, 2));


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
                _noteDbManager.AddSyncStatus(new SyncStatus(n.Id, 3));
            else
            {
                if (_noteDbManager.GetSyncStatus(n.Id).Status == OperationStatus.Created)
                {
                    _noteDbManager.DeleteSyncStatus(n.Id);
                    return true;
                }
                else
                    _noteDbManager.UpdateSyncStatus(new SyncStatus(n.Id, 3));
            }

            if (await _notesApiManager.DeleteNote(n.Id))
                _noteDbManager.DeleteSyncStatus(n.Id);
            else
                return false;

            return true;
        }


    }
}
