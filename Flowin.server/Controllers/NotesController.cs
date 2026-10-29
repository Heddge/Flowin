using System.Net.NetworkInformation;
using Microsoft.AspNetCore.Mvc;
using Flowin.server.Data;
using Flowin.server.Objects;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Flowin.contracts;

namespace Flowin.server.Controllers
{
    [ApiController]
    [Route("api/")]
    public class NotesController : ControllerBase
    {
        private NotesDbContext _notesDbContext;
        private readonly UserManager<User> _userManager;
        public NotesController(NotesDbContext ndc, UserManager<User> um)
        {
            _notesDbContext = ndc;
            _userManager = um;
        }

        [HttpPost("Notes/")]
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

        [HttpGet("Notes/")]
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

        [HttpGet("Notes/{id}")]
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

        [HttpPut("Notes/{id}")]
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

        [HttpDelete("Notes/{id}")]
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

        [HttpPost("AspNetUsers/")]
        public async Task<ActionResult<User?>?> CreateUserPOST([FromBody] RegisterRequest rr)
        {
            try
            {
                if (rr == null) return NotFound();
                User us = new User(rr.Name, rr.Surname, rr.Email);
                var resp = await _userManager.CreateAsync(us, rr.Password);
                //добавить проверку наличия ошибок. если их нет - savechanges.

                if (!resp.Succeeded)
                {
                    foreach (var error in resp.Errors)
                        System.Diagnostics.Debug.WriteLine($"{error.Code}: {error.Description}");

                    return NotFound();
                }
                return _notesDbContext.SaveChanges() == 1 ? Ok() : StatusCode(500, "Server Error");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return StatusCode(503);
            }
        }
    }
}
