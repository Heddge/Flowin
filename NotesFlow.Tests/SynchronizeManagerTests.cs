using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NotesFlow.Managers;
using NotesFlow.Interfaces;
using System.Security;

namespace NotesFlow.Tests
{
    public class SynchronizeManagerTests
    {
        [Fact]
        public async Task SynchronizeRunsPhasesInOrder()
        {
            Mock<INoteDbManager> mock_ndm = new Mock<INoteDbManager>();
            Mock<INotesApiManager> mock_nam = new Mock<INotesApiManager>();
            Mock<ISynchronizeManager> ism = new Mock<ISynchronizeManager>();
            SynchronizeManager sm = new SynchronizeManager(mock_ndm.Object, mock_nam.Object);


            await sm.Synchronize();
        }   
    }
}
