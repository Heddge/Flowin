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
            if (_noteDbManager.IsSyncStatusesEmpty()) return;

            foreach (SyncStatus toSync in _noteDbManager.GetAllSyncStatuses())
            {
                switch (toSync.Status)
                {
                    case OperationStatus.Created:
                        {
                            Note? note = _noteDbManager.GetNote(toSync.NoteId);
                            if (note != null)
                                if (await _notesApiManager.SaveNote(note) != null)
                                    _noteDbManager.DeleteSyncStatus(toSync.NoteId);
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
                            if (await _notesApiManager.GetNote(toSync.NoteId) != null)
                            {
                                if (await _notesApiManager.DeleteNote(toSync.NoteId))
                                {
                                    _noteDbManager.DeleteSyncStatus(toSync.NoteId);
                                    break;
                                }
                                else
                                    break;
                            }
                            _noteDbManager.DeleteSyncStatus(toSync.NoteId);
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
