using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Flowin.Managers;
using Flowin.Interfaces;
using System.Security;

namespace Flowin.Tests
{
    public class SynchronizeManagerTests
    {
        [Fact]
        public async Task SynchronizeRunsPhasesInOrder()
        {
            //Mock<INoteDbManager> mock_ndm = new Mock<INoteDbManager>();
            //Mock<INotesApiManager> mock_nam = new Mock<INotesApiManager>();
            //Mock<ISynchronizeManager> mock_sm = new Mock<ISynchronizeManager>();
            //SynchronizeManager sm = new SynchronizeManager(mock_ndm.Object, mock_nam.Object);

            //mock_sm.Setup(x => 
            //{
            //    x.
            //})
            //await sm.Synchronize();
        }
    }
}
