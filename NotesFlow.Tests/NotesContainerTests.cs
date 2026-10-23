using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using NotesFlow;
using NotesFlow.Managers;
using NotesFlow.Objects;
<<<<<<< Updated upstream
using NotesFlow.server.Data;
using Xunit;
using Moq;
=======
using NotesFlow.Interfaces;
using NotesFlow.server.Data;
using Xunit;
using Moq;
using System.Threading.Tasks;
>>>>>>> Stashed changes

namespace NotesFlow.Tests
{
    public class NotesContainerTests
    {
<<<<<<< Updated upstream
        private NotesFlow.Objects.NotesContainer? _notesContainer;
=======
        //private NotesFlow.Objects.NotesContainer? _notesContainer;
>>>>>>> Stashed changes

        [Fact]
        public void CanCreateNotesContainer()
        {
<<<<<<< Updated upstream
            IDbContextFactory<NoteDbContext> fac = Mock.Of<IDbContextFactory<NoteDbContext>>();
            HttpClient client = new HttpClient();
            NotesApiManager nam = new NotesApiManager(client);
            NoteDbManager ndm = new NoteDbManager(fac);
            SynchronizeManager sm = new SynchronizeManager(ndm, nam);
            Assert.NotNull(new NotesContainer(sm));
=======
            Mock<ISynchronizeManager> mock = new Mock<ISynchronizeManager>(MockBehavior.Loose);
            NotesContainer nc = new NotesContainer(mock.Object);
            Assert.NotNull(nc);
        }

        [Fact]
        public void NotesAreNullAfterCreating()
        {
            Mock<ISynchronizeManager> m = new Mock<ISynchronizeManager>(MockBehavior.Loose); // Strict - более строгий, добавть позже по необх
            NotesContainer nc = new NotesContainer(m.Object);
            Assert.NotNull(nc.GetNotes());
            Assert.Equal(0, nc.GetNotesCount());
        }

        [Fact]
        public async Task SaveNoteToCache()
        {
            Mock<ISynchronizeManager> mock = new Mock<ISynchronizeManager>(MockBehavior.Loose);
            Note to_save = new Note(Guid.NewGuid(), "hi", "means hello");
            List<Note> exepcting = new List<Note>() { to_save };
            mock.Setup(x => x.SaveNote(to_save)).ReturnsAsync(false);
            NotesContainer nc = new NotesContainer(mock.Object);
            await nc.SaveNote(to_save.Id, to_save.Title, to_save.Content);
            Assert.Equal(exepcting, nc.GetNotes());
>>>>>>> Stashed changes
        }
    }
}
