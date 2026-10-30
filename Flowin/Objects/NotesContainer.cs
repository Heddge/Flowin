using Flowin.Managers;
using Flowin.Interfaces;

namespace Flowin.Objects
{
    public class NotesContainer
    {
        protected List<Note> notes;
        protected ISynchronizeManager _synchronizeManager;

        public NotesContainer(ISynchronizeManager sm)
        {
            _synchronizeManager = sm;
            notes = new List<Note>();
        }

        public async Task Synchronizate()
        {
            await _synchronizeManager.Synchronize();
            this.notes = _synchronizeManager.GetNotes();
        }

        public async Task GetNotesAsync()
            => this.notes = _synchronizeManager.GetNotes();

        public async Task SaveNote(Guid id, string title, string content)
        {
            Note n = new Note(Guid.Empty, id, title, content);
            notes.Add(n);
            await _synchronizeManager.SaveNote(n);
        }

        public async Task UpdateNote(Note n)
        {
            int idx = notes.FindIndex(x => x.Id == n.Id);
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
                notes.Remove(n);
                await _synchronizeManager.DeleteNote(n);
            }
        }

        // getters
        public List<Note> GetNotes() => notes;
        public int GetNotesCount() => notes.Count;
        public Guid GetId() => Guid.NewGuid();
    }
}
