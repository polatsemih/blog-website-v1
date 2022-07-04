using System;

namespace sup1.entity
{
    public class CommentReply
    {
        public int CommentReplyId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserImageUrl { get; set; }
        public string Message { get; set; }
        public DateTime DateAdded { get; set; }

        public Comment Comment { get; set; }
        public int? CommentId { get; set; }
    }
}