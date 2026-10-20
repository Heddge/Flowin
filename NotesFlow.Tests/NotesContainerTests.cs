using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using NotesFlow;
using NotesFlow.Managers;
using NotesFlow.Objects;
using NotesFlow.server.Data;
using Xunit;
using Moq;

namespace NotesFlow.Tests
{
    public class NotesContainerTests
    {
        private NotesFlow.Objects.NotesContainer? _notesContainer;

        [Fact]
        public void CanCreateNotesContainer()
        {
            IDbContextFactory<NoteDbContext> fac = Mock.Of<IDbContextFactory<NoteDbContext>>();
            HttpClient client = new HttpClient();
            NotesApiManager nam = new NotesApiManager(client);
            NoteDbManager ndm = new NoteDbManager(fac);
            SynchronizeManager sm = new SynchronizeManager(ndm, nam);
            Assert.NotNull(new NotesContainer(sm));
        }
    }
}
