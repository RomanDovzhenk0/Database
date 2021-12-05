using System;

namespace Lab_3
{
    public class User
    {
        public int id;
        public string username;
        public string password_hash;
        public string fullname;
        public string acctype;
        public string createdAt;
        // public User(string username, string hash, string fullname, string acctype, string createdAt)
        // {
        //     this.username = username;
        //     this.password_hash = hash;
        //     this.fullname = fullname;
        //     this.acctype = acctype;
        //     this.createdAt = createdAt;
        // }
        public override string ToString()
        {
            return $"[{id}]   [{username}]   [{fullname}]   [{acctype}]";
        }
    }
}
//add post Author Heading Post
//remove post id
//add comment Author postId comment
//remove comment id