using System.Net.NetworkInformation;
using Microsoft.AspNetCore.Mvc;
using NotesFlow.server.Data;
using NotesFlow.server.Objects;

namespace NotesFlow.server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotesController : ControllerBase
    {
        private NotesDbContext _notesDbContext;
        public NotesController(NotesDbContext ndc)
        {
            _notesDbContext = ndc;
        }

        [HttpPost]
        public ActionResult<Note> AddNotePOST(Note n)
        {
            _notesDbContext.Notes.Add(n);
            try
            {
                return _notesDbContext.SaveChanges() == 1 ? StatusCode(201, n) : StatusCode(400, n);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }
        }

        [HttpGet]
        public List<Note> GetNotes()
        {
            try
            {
                return _notesDbContext.Notes.ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return new List<Note>();
            }
        }

        [HttpGet("{id}")]
        public ActionResult<Note> GetNote(Guid id)
        {
            try
            {
                Note? n = _notesDbContext.Notes.Find(id);
                if (n == null)
                    return NotFound();
                return Ok(n);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return NotFound();
            }
        }

        [HttpPut("{id}")]
        public ActionResult<Note> UpdateNote(Note n)
        {
            try
            {
                Note? note = _notesDbContext.Notes.Find(n.Id);
                if (note == null) return NotFound();

                note.Title = n.Title;
                note.Content = n.Content;
                note.UpdatedAt = DateTime.UtcNow;

                _notesDbContext.SaveChanges();
                return Ok(note);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ERROR NotesController.UpdateNote(id, n)");
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteNoteDELETE(Guid id)
        {
            try
            {
                Note? n = _notesDbContext.Notes.Find(id);
                if (n == null) return NotFound();

                _notesDbContext.Notes.Remove(n);

                return _notesDbContext.SaveChanges() == 1 ? NoContent() : StatusCode(500, "Server Error");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return StatusCode(503);
            }
        }

    }
}
