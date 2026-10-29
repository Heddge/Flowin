using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flowin.Objects;

namespace Flowin.Interfaces
{
    /// <summary>
    /// Interface for worker with DBs.
    /// </summary>
    public interface ISynchronizeManager
    {
        /// <summary>
        /// Main sycnhronization local updates with server`s.
        /// </summary>
        Task Synchronize();


        /// <summary>
        /// Just Notes List getter.
        /// </summary>
        /// <returns>List-Note-</returns>
        List<Note> GetNotes();


        /// <summary>
        /// Saving Note to both DB`s.
        /// </summary>
        /// <param name="note">Note that needed to save.</param>
        /// <returns>isSaved?</returns>
        Task<bool> SaveNote(Note note);



        /// <summary>
        /// Updating Note in both DB`s.
        /// </summary>
        /// <param name="note">Note that needed to update.</param>
        /// <returns>isUpdated?</returns>
        Task<bool> UpdateNote(Note note);



        /// <summary>
        /// Deleting Note in both DB`s.
        /// </summary>
        /// <param name="note">Note that needed to delete.</param>
        /// <returns>isDeleted?</returns>
        Task<bool> DeleteNote(Note note);
    }
}
