namespace Domain.Abstractions
{
    public interface INotification
    {
        public string Id { get; set; }
        public DateTime DateCreated { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public bool SendEmail { get; set; }
        public bool SendSMS { get; set; }
        public bool SendPush { get; set; }
        public bool SendBrowser { get; set; }

        public IEnumerable<TEMSUser> GetUsers();
        public void MarkSeen(TEMSUser user);
        public bool IsSeen(TEMSUser user);
    }
}
