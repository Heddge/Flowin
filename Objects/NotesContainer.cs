using NotesFlow.Managers;

namespace NotesFlow.Objects
{
    public class NotesContainer
    {
        private List<Note> notes;
        private SynchronizeManager _synchronizeManager;

        public NotesContainer(SynchronizeManager sm)
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
            Note n = new Note(id, title, content);
            notes.Add(n);
            await _synchronizeManager.SaveNote(n);
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
            }
        }

        // getters
        public List<Note> GetNotes() => notes;
        public Guid GetId() => Guid.NewGuid();
    }
}
