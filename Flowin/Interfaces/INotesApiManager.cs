using Flowin.Managers;
using Flowin.Objects;
using Microsoft.AspNetCore.Identity;

namespace Flowin.Interfaces
{
    /// <summary>
    /// An interface for manager working with API
    /// </summary>
    public interface INotesApiManager
    {
        /// <summary>
        /// Getting Dictionary of Ids and UpdatedAt timestamps from server`s DB
        /// </summary>
        /// <returns></returns>
        Task<Dictionary<Guid, DateTime>?> GetIdUpdateAt();

        /// <summary>
        /// Getting Note by Id from server`s DB
        /// </summary>
        /// <param name="Id">Needed note`s Id</param>
        /// <returns></returns>
        Task<RequestResultStatus<Note?>> GetNote(Guid Id);

        /// <summary>
        /// The whole List of notes from server DB`s Notes table
        /// </summary>
        /// <returns></returns>
        Task<List<Note>?> GetNotes();

        /// <summary>
        /// Saving note to server`s DB
        /// </summary>
        /// <param name="note">Note needs to save</param>
        /// <returns>Saved note</returns>
        Task<Note?> SaveNote(Note note);

        /// <summary>
        /// Deleting note from server`s DB
        /// </summary>
        /// <param name="id">Id note that needs to delete</param>
        /// <returns>IsDeleted?</returns>
        Task<bool> DeleteNote(Guid id);

        /// <summary>
        /// Updating note from server`s DB
        /// </summary>
        /// <param name="note">Note that needs to update</param>
        /// <returns>IsUpdated?</returns>
        Task<Note?> UpdateNote(Note note);

        /// <summary>
        /// Creating new User in server`s DB using password p.
        /// </summary>
        /// <param name="new_us"></param>
        /// <param name="p"></param>
        /// <returns>identityResult</returns>
        Task<RequestResultStatus<User>?> CreateUser(User new_us, string p);

        /// <summary>
        /// Logining into App by checking user`s email and password
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns>RequestResultStatus</returns>
        Task<RequestResultStatus<User?>> Login(string email, string password);


    }
}
