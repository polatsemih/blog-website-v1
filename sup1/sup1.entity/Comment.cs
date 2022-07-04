using System;
using System.Collections.Generic;

namespace sup1.entity
{
    public class Comment
    {
        public int CommentId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserImageUrl { get; set; }
        public string Message { get; set; }
        public DateTime DateAdded { get; set; }

        public Article Article { get; set; }
        public int? ArticleId { get; set; }

        public List<CommentReply> CommentReplys { get; set; }
    }
}