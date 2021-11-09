namespace Lab_2
{
    public class Comment
    {
        public int id;
        public string author;
        public string createdAt;
        public string comment;
        public int postId;
        public int authorId;

        // public Comment(string author, string comment, string createdAt, int postId, int authorId)
        // {
        //     this.author = author;
        //     this.postId = postId;
        //     this.createdAt = createdAt;
        //     this.comment = comment;
        //     this.authorId = authorId;
        // }
        public override string ToString()
        {
            return $"[{id}]   [{postId}]   [{authorId}]   [{comment}]";
        }
    }
}