using System.Collections.Generic;

namespace Lab_2
{
    public class Post
    {
        public int id;
        public string author;
        public int commentsCount = 0;
        public string createdAt;
        public string heading;
        public string post;
        public int authorId;
        public int pinned_commentId = -1;
        public List<Comment> commentList = null;

        // public Post(string heading, string post, string author, int commentsCount, string createdAt, int authorId)
        // {
        //     this.author = author;
        //     this.commentsCount = commentsCount;
        //     this.createdAt = createdAt;
        //     this.heading = heading;
        //     this.post = post;
        //     this.authorId = authorId;
        // }
        public override string ToString()
        {
            return $"[{id}]   [{authorId}]   [{heading}]   [{post}]   [{author}]";
        }
    }
}