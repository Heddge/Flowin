using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using NotesFlow.Objects;

namespace NotesFlow.Managers
{
    /// <summary>
    /// Менеджер общения NoetContainer (UI) (временно) и PostgreSQL.
    /// </summary>
    public class NotesApiManager
    {
        private HttpClient _httpClient;
        private string _apiUrl = "http://localhost:5172/api/Notes";
        public NotesApiManager(HttpClient hc)
        {
            System.Diagnostics.Debug.WriteLine("notesapimaanager constructor entered");

            _httpClient = hc;
        }

        public async Task<Note?> GetNote(Guid Id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<Note>(_apiUrl + "/" + Id);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("404. HTTP exception NotesApiManager.GetNote(id).");
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<List<Note>?> GetNotes()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<Note>>(_apiUrl);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("404. HTTP exception NotesApiManager.GetNotes().");
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<Note?> SaveNote(Note note)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync<Note>(_apiUrl, note);
                if (!response.IsSuccessStatusCode)
                    return null;

                return await response.Content.ReadFromJsonAsync<Note>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("404. HTTP exception NotesApiManager.SaveNote(note).");
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<bool> DeleteNote(Guid id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync(_apiUrl + "/" + id.ToString());
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("404. HTTP exception NotesApiManager.Delete(id).");
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<Note?> UpdateNote(Note note)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync<Note>(_apiUrl + "/" + note.Id.ToString(), note);

                if (!response.IsSuccessStatusCode)
                    return null;

                return await response.Content.ReadFromJsonAsync<Note>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("404. HTTP exception NotesApiManager.UpdateNote(note).");
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
