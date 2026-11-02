namespace Flowin.Managers
{
    public enum NotificationType { Success = 1, Information, Warning, Error };
    public struct Notification
    {
        public int HttpCode;
        public string Message;
        public NotificationType Type;

        public Notification() { }
        public Notification(int code, string message, int status)
        {
            HttpCode = code;
            Message = message;
            Type = (NotificationType)status;
        }
    }

    public class NotificationManager
    {
        private bool isShowing = false;
        private List<Notification> notifications_log { get; set; } = new List<Notification>();
        private Queue<Notification> notifications_que { get; set; } = new Queue<Notification>();
        private Notification current_notification { get; set; } = new Notification();
        private TimeSpan showing_time { get; set; } = TimeSpan.FromSeconds(3);

        public event Action<Notification?>? showing;
        public event Action<Notification?>? hiding;

        public NotificationManager() { }

        public void AddErrorToQueue(Notification err)
        {
            notifications_que.Enqueue(err);
            notifications_log.Add(err);
        }

        public async Task ShowNotification()
        {
            Notification s;
            if (notifications_que.Count > 0 && !isShowing)
            {
                s = notifications_que.Dequeue();
                isShowing = true;
                showing?.Invoke(s);
                await Task.Delay(showing_time);
                hiding?.Invoke(s);
                isShowing = false;
                if (notifications_que.Count > 0)
                    await ShowNotification();
            }
        }

        public IReadOnlyList<Notification> ShowErrorLog()
        {
            return notifications_log;
        }

        public void ChangeShowTime(int seconds)
        {
            showing_time = TimeSpan.FromSeconds(seconds);

        }
    }
}
