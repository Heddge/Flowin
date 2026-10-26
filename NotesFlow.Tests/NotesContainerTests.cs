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
            Assert.True(saved is not null &&
                        to_save.Id == saved.Id &&
                        to_save.Content == saved.Content &&
                        to_save.Title == saved.Title);
        }

        [Fact]
        public async Task SaveNoteCallsSynchronizeManager()
        {
            Mock<ISynchronizeManager> mock = new Mock<ISynchronizeManager>(MockBehavior.Loose);
            NotesContainer nc = new NotesContainer(mock.Object);
            //mock.Setup(x => x.SaveNote(It.IsAny<Note>())).ReturnsAsync(false); // можно убрать, нам не важен результат SaveNote
            await nc.SaveNote(Guid.NewGuid(), "hi", "hello");
            mock.Verify(x => x.SaveNote(It.IsAny<Note>()), Times.Once);
        }


        [Fact]
        public async Task SaveNotePassesCorrectNoteForSycnhrManager()
        {
            Mock<ISynchronizeManager> mock = new Mock<ISynchronizeManager>(MockBehavior.Loose);
            NotesContainer nc = new NotesContainer(mock.Object);
            Note n = new Note(Guid.NewGuid(), "hi", "hello");
            await nc.SaveNote(n.Id, n.Title, n.Content);
            mock.Verify(x => x.SaveNote(It.Is<Note>(x => x.IsValidNote() &&
                                                    x.Id == n.Id &&
                                                    x.Title == n.Title &&
                                                    x.Content == n.Content)), Times.Once);
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
        public async Task UpdateNoteInAppsCacheTitleContent()
        {
            Mock<ISynchronizeManager> mock = new Mock<ISynchronizeManager>(MockBehavior.Loose);
            NotesContainer nc = new NotesContainer(mock.Object);
            Note to_update = new Note(Guid.NewGuid(), "hi", "hello");
            await nc.SaveNote(to_update.Id, to_update.Title, to_update.Content);
            to_update.Content = "updated";
            to_update.Title = "title too";
            await nc.UpdateNote(to_update);
            mock.Verify(x => x.UpdateNote(It.Is<Note>(x => x.IsValidNote() &&
                                                      x.Id == to_update.Id &&
                                                      x.Title == "title too" &&
                                                      x.Content == "updated")), Times.Once);
            Assert.True(to_update == nc.GetNotes().FirstOrDefault());
        }

        [Fact]
        public async Task UpdateNoteInAppsCacheUpdatedAt()
        {
            Mock<ISynchronizeManager> mock = new Mock<ISynchronizeManager>(MockBehavior.Loose);
            NotesContainer nc = new NotesContainer(mock.Object);
            Note to_update = new Note(Guid.NewGuid(), "hi", "hello");
            mock.Setup(x => x.SaveNote(to_update)).ReturnsAsync(false);
            mock.Setup(x => x.UpdateNote(to_update)).ReturnsAsync(false);
            await nc.SaveNote(to_update.Id, to_update.Title, to_update.Content);
            
            await nc.UpdateNote(to_update);
            Assert.True(to_update.UpdatedAt != nc.GetNotes().FirstOrDefault()?.UpdatedAt);
        }

        
        [Fact]
        public async Task UpdateNotePassesCorrectNoteForSycnhrManager()
        {
            Mock<ISynchronizeManager> mock = new Mock<ISynchronizeManager>(MockBehavior.Loose);
            NotesContainer nc = new NotesContainer(mock.Object);
            Note n = new Note(Guid.NewGuid(), "hi", "hello");
            mock.Setup(x => x.SaveNote(n)).ReturnsAsync(false);
            await nc.SaveNote(n.Id, n.Title, n.Content);
            n.Content = "Updated";
            mock.Setup(x => x.UpdateNote(n)).ReturnsAsync(false);
            ////or:
            //mock.Setup(x => x.UpdateNote(n)).
            //    Callback<Note>(
            //    note => 
            //    { 
            //        Assert.True(note.IsValidNote()); 
            //    }
            //    );
            await nc.UpdateNote(n);
            mock.Verify(x => x.UpdateNote(It.Is<Note>(x => x.IsValidNote())), Times.Once);
        }

        [Fact]
        public async Task UpdateNoteDoesNothingForUnknownId()
        {
            Mock<ISynchronizeManager> mock = new Mock<ISynchronizeManager>(MockBehavior.Loose);
            NotesContainer nc = new NotesContainer(mock.Object);
            Note n = new Note(Guid.NewGuid(), "hi", "hello");
            mock.Setup(x => x.SaveNote(n)).ReturnsAsync(false);
            await nc.SaveNote(n.Id, n.Title, n.Content);
            n.Content = "Updated";
            n.Id = Guid.Empty;
            await nc.UpdateNote(n);
            mock.Verify(x => x.UpdateNote(It.IsAny<Note>()), Times.Never);
        }

        [Fact]
        public async Task DeleteNoteFromAppsCache()
        {
            Mock<ISynchronizeManager> mock = new Mock<ISynchronizeManager>(MockBehavior.Loose);
            NotesContainer nc = new NotesContainer(mock.Object);
            Note to_delete = new Note(Guid.NewGuid(), "hi", "hello");
            mock.Setup(x => x.SaveNote(to_delete)).ReturnsAsync(false);
            await nc.SaveNote(to_delete.Id, to_delete.Title, to_delete.Content);
            mock.Setup(x => x.DeleteNote(to_delete)).ReturnsAsync(false);
            await nc.DeleteNote(to_delete.Id);
            Assert.Equal(0, nc.GetNotesCount());
        }


        [Fact]
        public async Task DeleteNotePassesCorrectNoteForSycnhrManager()
        {
            Mock<ISynchronizeManager> mock = new Mock<ISynchronizeManager>(MockBehavior.Loose);
            NotesContainer nc = new NotesContainer(mock.Object);
            Note n = new Note(Guid.NewGuid(), "hi", "hello");
            mock.Setup(x => x.SaveNote(n)).ReturnsAsync(false);
            await nc.SaveNote(n.Id, n.Title, n.Content);
            mock.Setup(x => x.DeleteNote(n)).
                Callback<Note>(
                note =>
                {
                    Assert.True(note.IsValidNote());
                }
                ).ReturnsAsync(false);
            await nc.DeleteNote(n.Id);
            mock.Verify(x => x.DeleteNote(It.Is<Note>(x => x.IsValidNote())), Times.Once);
        }

        [Fact]
        public async Task DeleteNoteDoesNothingForUnknownId()
        {
            Mock<ISynchronizeManager> mock = new Mock<ISynchronizeManager>(MockBehavior.Loose);
            NotesContainer nc = new NotesContainer(mock.Object);
            Note n = new Note(Guid.NewGuid(), "hi", "hello");
            mock.Setup(x => x.SaveNote(n)).ReturnsAsync(false);
            await nc.SaveNote(n.Id, n.Title, n.Content);
            n.Id = Guid.Empty;
            await nc.DeleteNote(n.Id);
            mock.Verify(x => x.DeleteNote(n), Times.Never);
        }

        [Fact]
        public async Task SynchronizateRefreshesCacheAfterSync()
        {
            Mock<ISynchronizeManager> mock = new Mock<ISynchronizeManager>(MockBehavior.Loose);
            NotesContainer nc = new NotesContainer(mock.Object);
            await nc.Synchronizate();
            mock.Verify(x => x.GetNotes(), Times.Once);
        }
    }
}
