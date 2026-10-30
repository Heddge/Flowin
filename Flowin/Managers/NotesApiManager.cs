using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Flowin.Objects;
using Microsoft.AspNetCore;
using System.Text.Json;
using Flowin.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Flowin.contracts;

namespace Flowin.Managers
{
    /// <summary>
    /// Менеджер общения NoetContainer (UI) (временно) и PostgreSQL.
    /// </summary>
    public class NotesApiManager : INotesApiManager
    {
        //private UserManager<User> _userManager;UserManager<User> userManager
        private HttpClient _httpClient;
        private string _apiUrlNotes = "http://localhost:5172/api/Notes";
        private string _apiUrlUsers = "http://localhost:5172/api/AspNetUsers";
        public NotesApiManager(HttpClient hc)
        {
            //System.Diagnostics.Debug.WriteLine("notesapimaanager constructor entered");
            _httpClient = hc;
            //_userManager = userManager;
        }

        public async Task<Dictionary<Guid, DateTime>?> GetIdUpdateAt()
        {
            try
            {
                var response = await GetNotes();
                if (response == null)
                    return null;
                return response.Select(x => new KeyValuePair<Guid, DateTime>(x.Id, x.UpdatedAt)).ToDictionary<Guid, DateTime>();
            }
            catch (HttpRequestException ex_request_err)
            {
                System.Diagnostics.Debug.WriteLine("404/503. HTTP exception NotesApiManager.GetNote(id). ServiceUnavaliable or NotFound");
                System.Diagnostics.Debug.WriteLine(ex_request_err.Message);
                return null;
            }
            catch (UriFormatException ex_uri)
            {
                System.Diagnostics.Debug.WriteLine("HTTP exception NotesApiManager.GetNote(id). IncorrectApiUrl");
                System.Diagnostics.Debug.WriteLine(ex_uri.Message);
                return null;
            }
            catch (JsonException ex_json)
            {
                System.Diagnostics.Debug.WriteLine("HTTP exception NotesApiManager.GetNote(id). IncorrectJson");
                System.Diagnostics.Debug.WriteLine(ex_json.Message);
                return null;
            }
            catch (OperationCanceledException ex_canceled)
            {
                System.Diagnostics.Debug.WriteLine("HTTP exception NotesApiManager.GetNote(id). ServiceUnavaliable");
                System.Diagnostics.Debug.WriteLine(ex_canceled.Message);
                return null;
            }
        }

        public async Task<RequestResultStatus<Note?>> GetNote(Guid Id)
        {
            try
            {
                var response = await _httpClient.GetAsync(_apiUrlNotes + "/" + Id);

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    System.Diagnostics.Debug.WriteLine($"GET URL: {response.RequestMessage?.RequestUri}");
                    System.Diagnostics.Debug.WriteLine($" HTTP STATUS: {(int)response.StatusCode}");
                    System.Diagnostics.Debug.WriteLine(await response.Content.ReadFromJsonAsync<Note>());
                    return new RequestResultStatus<Note?>(RequestStatus.NotFound, null);
                }

                return new RequestResultStatus<Note?>(RequestStatus.Success, await response.Content.ReadFromJsonAsync<Note?>());



            }
            catch (HttpRequestException ex_request_err)
            {
                System.Diagnostics.Debug.WriteLine("404/503. HTTP exception NotesApiManager.GetNote(id). ServiceUnavaliable or NotFound");
                System.Diagnostics.Debug.WriteLine(ex_request_err.Message);
                return new RequestResultStatus<Note?>(RequestStatus.ServiceUnavaliable, null);
            }
            catch (UriFormatException ex_uri)
            {
                System.Diagnostics.Debug.WriteLine("HTTP exception NotesApiManager.GetNote(id). IncorrectApiUrl");
                System.Diagnostics.Debug.WriteLine(ex_uri.Message);
                return new RequestResultStatus<Note?>(RequestStatus.IncorrectApiUrl, null);
            }
            catch (JsonException ex_json)
            {
                System.Diagnostics.Debug.WriteLine("HTTP exception NotesApiManager.GetNote(id). IncorrectJson");
                System.Diagnostics.Debug.WriteLine(ex_json.Message);
                return new RequestResultStatus<Note?>(RequestStatus.IncorrectJson, null);
            }
            catch (OperationCanceledException ex_canceled)
            {
                System.Diagnostics.Debug.WriteLine("HTTP exception NotesApiManager.GetNote(id). ServiceUnavaliable");
                System.Diagnostics.Debug.WriteLine(ex_canceled.Message);
                return new RequestResultStatus<Note?>(RequestStatus.ServiceUnavaliable, null);
            }
        }

        public async Task<List<Note>?> GetNotes()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<Note>>(_apiUrlNotes);
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
                var response = await _httpClient.PostAsJsonAsync<Note>(_apiUrlNotes, note);
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

        //public async Task<Note?> SaveNote(User curr_user, Note note)
        //{
        //    try
        //    {
        //        var response = await _httpClient.PostAsJsonAsync<Note>(_apiUrlNotes, note);
        //        if (!response.IsSuccessStatusCode)
        //            return null;

        //        return await response.Content.ReadFromJsonAsync<Note>();
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Diagnostics.Debug.WriteLine("404. HTTP exception NotesApiManager.SaveNote(note).");
        //        System.Diagnostics.Debug.WriteLine(ex.Message);
        //        return null;
        //    }
        //}


        public async Task<bool> DeleteNote(Guid id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync(_apiUrlNotes + "/" + id.ToString());
                if (response.IsSuccessStatusCode || (int)response.StatusCode == 204 || (int)response.StatusCode == 404)
                    return true;
                System.Diagnostics.Debug.WriteLine("Unexpected error code. HTTP exception NotesApiManager.Delete(id).");
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("500+. HTTP exception NotesApiManager.Delete(id).");
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<Note?> UpdateNote(Note note)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync<Note>(_apiUrlNotes + "/" + note.Id, note);

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

        public async Task<RequestResultStatus<User>?> CreateUser(User new_us, string p)
        {
            RegisterRequest rr = new RegisterRequest(new_us.Name, new_us.Surname, new_us.Email, p);
            var resp = await _httpClient.PostAsJsonAsync(_apiUrlUsers+"/Register", rr);
            if (!resp.IsSuccessStatusCode)
                return new RequestResultStatus<User>(RequestStatus.NotFound, new_us);
            return new RequestResultStatus<User>(RequestStatus.Success, new_us);
        }

        public async Task<RequestResultStatus<User?>> Login(string email, string password)
        {
            LoginRequest lr = new LoginRequest(email, password);
            var resp = await _httpClient.PostAsJsonAsync(_apiUrlUsers+"/Login", lr);
            if (resp.IsSuccessStatusCode)
            {
                var content = await resp.Content.ReadFromJsonAsync<User>();
                if (content != null)
                    return new RequestResultStatus<User?>(RequestStatus.Success, content);

                return new RequestResultStatus<User?>(RequestStatus.NotFound, null);
            }

            return new RequestResultStatus<User?>(RequestStatus.NotFound, null);
        }
    }
}
