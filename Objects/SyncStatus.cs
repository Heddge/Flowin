using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotesFlow.Objects
{
    public enum OperationStatus { Created = 1, Updated, Deleted }
    public class SyncStatus
    {
        [Key]
        public Guid NoteId { get; set; }
        public OperationStatus Status { get; set; }
        public SyncStatus() { }
        public SyncStatus(Guid nId, int status)
        {
            NoteId = nId;
            Status = (OperationStatus)status;
        }
    }
}
