using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using NotesFlow.Managers;

namespace NotesFlow.Objects
{
    public class NotesContainer
    {
        private List<Note> notes;
        // managers to work direct with DBs from NotesContainer
        
        //private NoteDbManager _noteDbManager;
        //private NotesApiManager _notesApiManager;
        private SynchronizeManager _synchronizeManager;

        public NotesContainer(SynchronizeManager sm)
        {
            //_noteDbManager = ndm;
            //_notesApiManager = nam;
            System.Diagnostics.Debug.WriteLine("constructor notescontainer entered");

            _synchronizeManager = sm;
            notes = new List<Note>();
            notes.Add(new Note(Guid.NewGuid(), "a", "b"));
            //GetNotesAsync();

            //notes = _noteDbManager.GetNotes();
            //GetNotesAsync();
        }

        public async Task Synchronizate()
            => await _synchronizeManager.SynchronizeSyncStatuses();

        public async Task GetNotesAsync()
        {
            System.Diagnostics.Debug.WriteLine("entered notescontainer.GetNotesAsync");
            this.notes = _synchronizeManager.GetNotes();
            
            //if (ns != null)
                //foreach (var note in ns)
                    //this.notes.Add(note);
        }

        public async Task SaveNote(Guid id, string title, string content)
        {

            Note n = new Note(id, title, content);
            notes.Add(n);
            await _synchronizeManager.SaveNote(n);

            //if (_noteDbManager.AddNote(n))
            //notes.Add(n);
        }

        public async Task UpdateNote(Guid id, Note n)
        {
            int idx = notes.FindIndex(x => x.Id == id);
            if (idx == -1)
                return;

            n.UpdatedAt = DateTime.UtcNow;
            notes[idx].Title = n.Title;
            notes[idx].Content = n.Content;

            await _synchronizeManager.UpdateNote(n);
        }

        public async Task DeleteNote(Guid id)
        {
            Note? n = notes.FirstOrDefault(x => x.Id == id);
            if (n != null)
            {
                await _synchronizeManager.DeleteNote(n);
                notes.Remove(n);

                //if (_noteDbManager.DeleteNote(n))
                //notes.Remove(n);
            }
        }

        // getters
        public List<Note> GetNotes() => notes;
        public Guid GetId() => Guid.NewGuid();
    }
}
