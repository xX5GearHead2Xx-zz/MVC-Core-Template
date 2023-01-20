namespace Ecommerce.ViewModels
{
    public class Notification
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public Enums.NotificationType NotificationType { get; set; }

        public Notification(string title, string message, Enums.NotificationType notificationType)
        {
            Title = title;
            Message = message;
            NotificationType = notificationType;
        }
    }
}
