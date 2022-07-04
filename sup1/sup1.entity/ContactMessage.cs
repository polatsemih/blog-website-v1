using System;

namespace sup1.entity
{
    public class ContactMessage
    {
        public int MessageId { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public DateTime DateAdded { get; set; }
        public string UserId { get; set; }
    }
}