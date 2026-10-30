using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flowin.Interfaces;

namespace Flowin.Objects
{
    public class LocalSessionContainer : NotesContainer
    {
        public User CurrentUser { get; set; }

        public LocalSessionContainer(ISynchronizeManager sm) : base(sm)
        {
            CurrentUser = new User("Egor", "Test", "egorik@test.ru", Guid.Empty);
            CurrentUser.Email = "egor@test.ru";
        }
        public User GetUser() => CurrentUser;

        public async Task SetUser(User user)
        {
            this.CurrentUser = user;
            this.notes = await _synchronizeManager.GetNotes(user);
            System.Diagnostics.Debug.WriteLine("USER CHECKING ->\n" +  user.Id + " " + user.Email);
        }

        public async Task<List<Note>> GetNotesAsync(User curr_user)
            => this.notes = await _synchronizeManager.GetNotes(curr_user);

        //public async Task GetNotesAsync(User curr_user)
        //    => this.notes = _synchronizeManager.GetNotes();

        public async Task SaveNote(User curr_user, Guid id, string title, string content)
        {
            Note n = new Note(curr_user.Id, id, title, content);
            notes.Add(n);
            await _synchronizeManager.SaveNote(curr_user, n);
        }

        public async Task UpdateNote(User curr_user, Note n)
        {
            int idx = notes.FindIndex(x => x.Id == n.Id);
            if (idx == -1)
                return;

            n.UpdatedAt = DateTime.UtcNow;
            notes[idx].Title = n.Title;
            notes[idx].Content = n.Content;

            await _synchronizeManager.UpdateNote(curr_user, n);
        }

        public async Task DeleteNote(User curr_user, Guid id)
        {
            Note? n = notes.FirstOrDefault(x => x.Id == id);
            if (n != null)
            {
                notes.Remove(n);
                await _synchronizeManager.DeleteNote(curr_user, n);
            }
        }

        //// getters
        //public List<Note> GetNotes(User curr_user) => notes;
        //public int GetNotesCount(User curr_user) => notes.Count;
        //public Guid GetId(User curr_user) => Guid.NewGuid();
    }
}
