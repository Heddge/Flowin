using Flowin.Interfaces;
using Flowin.Objects;

namespace Flowin.Managers
{
    public class AuthenticationManager
    {
        INotesApiManager notesApiManager;
        NotificationManager _notificationmanager;
        public AuthenticationManager(INotesApiManager inam, NotificationManager nfm)
        { 
            notesApiManager = inam;
            _notificationmanager = nfm;
        }
        public async Task<bool> Registration(string name, string surname,
            string email, string password, string second_password)
        {
            if (string.IsNullOrEmpty(name) ||
                string.IsNullOrEmpty(surname) ||
                string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(password) ||
                string.IsNullOrEmpty(second_password))
            {
                _notificationmanager.AddErrorToQueue(
                    new Notification(204, "Есть незаполненные поля, повторите попытку снова.", 4));
                await _notificationmanager.ShowNotification();
                return false;
            }

            if (password != second_password)
            {
                _notificationmanager.AddErrorToQueue(
                    new Notification(204, "Пароли отличаются. Повторите попытку снова.", 4));
                await _notificationmanager.ShowNotification();
                return false;
            }

            if (!email.Contains('@') || !email.Contains('.'))
            {
                _notificationmanager.AddErrorToQueue(
                    new Notification(204, "Почта невалидна. Повторите попытку снова.", 4));
                await _notificationmanager.ShowNotification();
                return false;
            }

            if (await notesApiManager.IsAvaliable())
            {
                try
                {
                    User new_user = new User(name, surname, email, Guid.NewGuid());

                    await notesApiManager.CreateUser(new_user, password);
                    return true;
                }
                catch (Exception ex)
                {

                    _notificationmanager.AddErrorToQueue(
                            new Notification(500, "Сервер недоступен. Повторите попытку позже", 4));
                    await _notificationmanager.ShowNotification();
                    return false;
                }
            }
            else
            {
                _notificationmanager.AddErrorToQueue(
                        new Notification(500, "Сервер недоступен. Повторите попытку позже", 4));
                await _notificationmanager.ShowNotification();
                return false;
            }
            }

        public async Task<User?> Logining(string email, string password)
        {
            if (string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(password))
            {
                _notificationmanager.AddErrorToQueue(
                    new Notification(204, "Есть незаполненные поля, повторите попытку снова.", 4));
                await _notificationmanager.ShowNotification();
                return null;
            }

            if (await notesApiManager.IsAvaliable())
            {
                try
                {
                    var response = await notesApiManager.Login(email, password);
                    if (response == null)
                    {
                        _notificationmanager.AddErrorToQueue(
                            new Notification(500, "Что-то пошло не так. Повторите попытку.", 4));
                        await _notificationmanager.ShowNotification();
                        return null;
                    }
                    return response.data;
                }
                catch (Exception ex)
                {

                    _notificationmanager.AddErrorToQueue(
                            new Notification(500, "Сервер недоступен. Повторите попытку позже", 4));
                    await _notificationmanager.ShowNotification();
                    return null;
                }
            }
            else
            {
                _notificationmanager.AddErrorToQueue(
                        new Notification(500, "Сервер недоступен. Повторите попытку позже", 4));
                await _notificationmanager.ShowNotification();
                return null;
            }
        }
    }
}
