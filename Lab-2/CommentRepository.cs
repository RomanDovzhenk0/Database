using Npgsql;
using System;
using static System.Math;
using System.Collections.Generic;

namespace Lab_2
{
    public class CommentRepository
    {
        private NpgsqlConnection connection;
        public CommentRepository(NpgsqlConnection connection)
        {
            this.connection = connection;
        }
        public List<Comment> GetAll()
        {
            NpgsqlCommand command = this.connection.CreateCommand();
            command.CommandText = @"SELECT * FROM comments";
            List<Comment> list = ReadCommentsFromCommand(command);

            return list;
        }
        public List<Comment> GetPage(int pageNumber, int pageSize) 
        {
            NpgsqlCommand command = this.connection.CreateCommand();
            command.CommandText = @"SELECT * FROM comments LIMIT @pageSize OFFSET @pageSize * (@pageNumber - 1)";
            command.Parameters.AddWithValue(@"pageSize", pageSize);
            command.Parameters.AddWithValue(@"pageNumber", pageNumber);

            List<Comment> list = ReadCommentsFromCommand(command);

            return list;
        }
        public List<Comment> GetPostPage(int pageNumber, int pageSize, Post post) 
        {
            NpgsqlCommand command = this.connection.CreateCommand();
            command.CommandText = @"SELECT * FROM comments WHERE postId = @postId LIMIT @pageSize OFFSET @pageSize * (@pageNumber - 1)";
            command.Parameters.AddWithValue(@"pageSize", pageSize);
            command.Parameters.AddWithValue(@"pageNumber", pageNumber);
            command.Parameters.AddWithValue(@"postId", post.id);

            List<Comment> list = ReadCommentsFromCommand(command);

            return list;
        }
        public List<Comment> SearchComments(string searchValue)
        {
            NpgsqlCommand command = this.connection.CreateCommand();
            command.CommandText = @"SELECT * FROM comments WHERE comment LIKE @filter";
            command.Parameters.AddWithValue("@filter", '%' + searchValue + '%');
            
            List<Comment> list = ReadCommentsFromCommand(command);

            return list;
        }
        static List<Comment> ReadCommentsFromCommand(NpgsqlCommand command)
        {
            NpgsqlDataReader reader = command.ExecuteReader();
            List<Comment> list = new List<Comment>();
            while (reader.Read())
            {
                Comment comment = ReadComments(reader);
                list.Add(comment);
            }
            reader.Close();
            return list;
        }
        public int GetTotalPages(int size) 
        {
            long nOperators = CountComments();
            double pageSize = size;
            return (int)Ceiling(nOperators / pageSize);
        }
        public int GetTotalPostPages(int size, Post post) 
        {
            long nOperators = CountPostComments(post.id);
            double pageSize = size;
            return (int)Ceiling(nOperators / pageSize);
        }
        public long CountComments()
        {
            NpgsqlCommand command = this.connection.CreateCommand();
            command.CommandText = @"SELECT COUNT(*) FROM comments";
            return (long)command.ExecuteScalar();
        }
        public long CountPostComments(int postId)
        {
            NpgsqlCommand command = this.connection.CreateCommand();
            command.CommandText = @"SELECT COUNT(*) FROM comments WHERE postId = @postId";
            command.Parameters.AddWithValue(@"postId", postId);
            return (long)command.ExecuteScalar();
        }
        public bool CommentExists(int id)
        {
            NpgsqlCommand command = this.connection.CreateCommand();
            command.CommandText = @"SELECT * FROM comments WHERE id = @id";
            command.Parameters.AddWithValue("@id", id);

            NpgsqlDataReader reader = command.ExecuteReader();

            bool result = reader.Read();
            return result;
        }
        public void Insert(Comment comment) 
        {
            NpgsqlCommand command = this.connection.CreateCommand();
            command.CommandText = 
            @"
                INSERT INTO comments (postid, authorid, comment) 
                VALUES (@postId, @authorId, @comment);
            ";
            command.Parameters.AddWithValue("@comment", comment.comment);
            command.Parameters.AddWithValue("@postId", comment.postId);
            command.Parameters.AddWithValue("@authorId", comment.authorId);
            command.ExecuteNonQuery();
        }
        public Comment GetById(int id) 
        { 
            NpgsqlCommand command = this.connection.CreateCommand();
            command.CommandText = @"SELECT * FROM comments WHERE id = @id";
            command.Parameters.AddWithValue("@id", id);
            
            NpgsqlDataReader reader = command.ExecuteReader();
            
            if (reader.Read())
            {
                Comment co = ReadComments(reader);
                reader.Close();
                return co;
            }
            else 
            {
                reader.Close();
                return null;
            } 
        }
        public int DeleteById(int id) 
        {
            NpgsqlCommand command = this.connection.CreateCommand();
            command.CommandText = @"DELETE FROM comments WHERE id = @id";
            command.Parameters.AddWithValue("@id", id);
            
            int nChanged = command.ExecuteNonQuery();
            
            if (nChanged == 0)
            {
                return 0;
            }
            else 
            {
                return 1;
            }
        }
        public void DeleteByPostId(int id) 
        {
            NpgsqlCommand command = this.connection.CreateCommand();
            command.CommandText = @"DELETE FROM comments WHERE postId = @id";
            command.Parameters.AddWithValue("@id", id);
            
            int nChanged = command.ExecuteNonQuery();
            
            if (nChanged == 0)
            {
                return;
            }
            else 
            {
                DeleteByPostId(id);
            }
        }
        public List<Comment> GetByPostId(int id)
        {
            NpgsqlCommand command = this.connection.CreateCommand();
            command.CommandText = @"SELECT * FROM comments WHERE postId = @id";
            command.Parameters.AddWithValue("@id", id);
            
            NpgsqlDataReader reader = command.ExecuteReader();
            List<Comment> list = new List<Comment>();  
            while(reader.Read())
            {
                Comment co = ReadComments(reader);
                list.Add(co);
            }
            reader.Close();
            return list;
        }
        public List<Comment> GetByAuthorId(int id) 
        { 
            NpgsqlCommand command = this.connection.CreateCommand();
            command.CommandText = @"SELECT * FROM comments WHERE authorId = @id";
            command.Parameters.AddWithValue("@id", id);
            
            NpgsqlDataReader reader = command.ExecuteReader();
            List<Comment> list = new List<Comment>();  
            while(reader.Read())
            {
                Comment co = ReadComments(reader);
                list.Add(co);
            }
            reader.Close();
            return list;
        }
        public int UpdateData(int id, string comment)
        {
            NpgsqlCommand command = this.connection.CreateCommand();
            command.CommandText = @"UPDATE comments SET comment = @comment WHERE id = @id";
            
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@comment", comment);

            int nChanged = command.ExecuteNonQuery();

            if (nChanged == 0)
            {
                return 0;
            }
            else 
            {
                return 1;
            }
        }
        static Comment ReadComments(NpgsqlDataReader reader)
        {
            // public Comment(string author, string comment, string createdAt, int postId, int authorId)
            // Console.WriteLine(@"{reader.GetString(0)}, {reader.GetString(2)}, {reader.GetString(3)}, {reader.GetString(4)}, {reader.GetString(5)}");
            Comment Comment = new Comment(){
                postId = Convert.ToInt32(reader.GetInt32(1)),
                comment = reader.GetString(3),
                authorId = Convert.ToInt32(reader.GetInt32(2))
            };
            Comment.id = Convert.ToInt32(reader.GetInt32(0));
            

            return Comment;
        }
    }
}
