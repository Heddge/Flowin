using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using NotesFlow;
using NotesFlow.Managers;
using NotesFlow.Objects;
using NotesFlow.server.Data;
using Xunit;
using Moq;
using NotesFlow.Interfaces;
using System.Threading.Tasks;

namespace NotesFlow.Tests
{
    public class NotesContainerTests
    {
        [Fact]
        public void CanInitialiteNotesContainer()
        {
            Mock<ISynchronizeManager> mock = new Mock<ISynchronizeManager>(MockBehavior.Loose);
            NotesContainer nc = new NotesContainer(mock.Object);
            Assert.NotNull(nc);
        }

        [Fact]
        public void ListNotesIsEmptyAfterInitialize()
        {
            Mock<ISynchronizeManager> mock = new Mock<ISynchronizeManager>(MockBehavior.Loose);
            NotesContainer nc = new NotesContainer(mock.Object);
            Assert.NotNull(nc.GetNotes());
            Assert.Equal(0, nc.GetNotesCount());
        }

        [Fact]
        public async Task SaveNoteToAppsCache()
        {
            Mock<ISynchronizeManager> mock = new Mock<ISynchronizeManager>(MockBehavior.Loose);
            NotesContainer nc = new NotesContainer(mock.Object);
            Note to_save = new Note(Guid.NewGuid(), "hi", "hello");
            mock.Setup(x => x.SaveNote(to_save)).ReturnsAsync(false);
            await nc.SaveNote(to_save.Id, to_save.Title, to_save.Content);
            Note? saved = nc.GetNotes().FirstOrDefault();
            Assert.True(to_save == saved);
        }

        [Fact]
        public void GetNotesFromAppsCache()
        {
            Mock<ISynchronizeManager> mock = new Mock<ISynchronizeManager>(MockBehavior.Loose);
            NotesContainer nc = new NotesContainer(mock.Object);
            Assert.NotNull(nc.GetNotes());
            Assert.Equal(0, nc.GetNotesCount());
        }

        [Fact]
        public async Task UpdateNoteInAppsCache()
        {
            Mock<ISynchronizeManager> mock = new Mock<ISynchronizeManager>(MockBehavior.Loose);
            NotesContainer nc = new NotesContainer(mock.Object);
            Note to_update = new Note(Guid.NewGuid(), "hi", "hello");
            mock.Setup(x => x.SaveNote(to_update)).ReturnsAsync(false);
            mock.Setup(x => x.UpdateNote(to_update)).ReturnsAsync(false);
            await nc.SaveNote(to_update.Id, to_update.Title, to_update.Content);
            to_update.Content = "updated";
            await nc.UpdateNote(to_update);
            Assert.True(to_update == nc.GetNotes().FirstOrDefault());
        }


        [Fact]
        public async Task DeleteNoteFromAppsCache()
        {
            Mock<ISynchronizeManager> mock = new Mock<ISynchronizeManager>(MockBehavior.Loose);
            NotesContainer nc = new NotesContainer(mock.Object);
            Note to_delete = new Note(Guid.NewGuid(), "hi", "hello");
            mock.Setup(x => x.SaveNote(to_delete)).ReturnsAsync(false);
            mock.Setup(x => x.DeleteNote(to_delete)).ReturnsAsync(false);
            await nc.SaveNote(to_delete.Id, to_delete.Title, to_delete.Content);
            await nc.DeleteNote(to_delete.Id);
            Assert.Equal(0, nc.GetNotesCount());
        }


    }
}
