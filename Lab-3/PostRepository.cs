using Npgsql;
using System;
using static System.Math;
using System.Collections.Generic;

namespace Lab_3
{
    public class PostRepository
    {
        private NpgsqlConnection connection;
        public PostRepository(NpgsqlConnection connection)
        {
            this.connection = connection;
        }
        public List<Post> GetAll()
        {
            NpgsqlCommand command = this.connection.CreateCommand();
            command.CommandText = @"SELECT * FROM posts";
            List<Post> list = ReadPostsFromCommand(command);

            return list;
        }
        public int GetTotalPages(int size) 
        {
            long nOperators = CountPosts();
            double pageSize = size;
            return (int)Ceiling(nOperators / pageSize);
        }
        public int GetSearchPagesCount(int size, string searchValue)
        {
            if(string.IsNullOrEmpty(searchValue))
            {
                return this.GetTotalPages(size);
            }
            return (int)Ceiling(this.SearchPosts(searchValue).Count / (double)size);
        }
        public List<Post> GetSearchPage(int pageNumber, int pageSize, string searchValue)
        {
            if(string.IsNullOrEmpty(searchValue))
            {
                return this.GetPage(pageNumber, pageSize);
            }
            NpgsqlCommand command = this.connection.CreateCommand();
            command.CommandText = @"SELECT * FROM posts WHERE post LIKE @filter OR heading LIKE @filter LIMIT @pageSize OFFSET @pageSize * (@pageNumber - 1)";
            command.Parameters.AddWithValue(@"pageSize", pageSize);
            command.Parameters.AddWithValue(@"pageNumber", pageNumber);
            command.Parameters.AddWithValue("@filter", '%' + searchValue + '%');

            List<Post> list = ReadPostsFromCommand(command);

            return list;
        }
        public List<Post> GetSearchMyPage(int pageNumber, int pageSize, int authorId, string searchValue)
        {
            if(string.IsNullOrEmpty(searchValue))
            {
                return this.GetMyPage(pageNumber, pageSize, authorId);
            }
            NpgsqlCommand command = this.connection.CreateCommand();

            command.CommandText = @"SELECT * FROM posts WHERE authorId = @authorId AND (heading LIKE @filter OR post LIKE @filter) LIMIT @pageSize OFFSET @pageSize * (@pageNumber - 1)";
            command.Parameters.AddWithValue("@authorId", authorId);
            command.Parameters.AddWithValue(@"pageSize", pageSize);
            command.Parameters.AddWithValue(@"pageNumber", pageNumber);
            command.Parameters.AddWithValue("@filter", '%' + searchValue + '%');

            List<Post> list = ReadPostsFromCommand(command);

            return list;
        }
        public List<Post> SearchPosts(string searchValue)
        {
            NpgsqlCommand command = this.connection.CreateCommand();
            command.CommandText = @"SELECT * FROM posts WHERE post LIKE @filter OR heading LIKE @filter";
            command.Parameters.AddWithValue("@filter", '%' + searchValue + '%');
            
            List<Post> list = ReadPostsFromCommand(command);

            return list;
        }
        public List<Post> SearchMyPosts(string searchValue, int authorId)
        {
            NpgsqlCommand command = this.connection.CreateCommand();
            command.CommandText = @"SELECT * FROM posts WHERE authorId = @authorId AND (heading LIKE @filter OR post LIKE @filter)";
            command.Parameters.AddWithValue("@authorId", authorId);
            command.Parameters.AddWithValue("@filter", '%' + searchValue + '%');
            
            List<Post> list = ReadPostsFromCommand(command);

            return list;
        }
        public int GetSearchMyTotalPages(int size, int authorId, string searchValue) 
        {
            long nOperators = CountMySearchPosts(authorId, searchValue);
            double pageSize = size;
            return (int)Ceiling(nOperators / pageSize);
        }
        public int GetMyTotalPages(int size, int authorId) 
        {
            long nOperators = CountMyPosts(authorId);
            double pageSize = size;
            return (int)Ceiling(nOperators / pageSize);
        }
        public bool PostExists(int id)
        {
            NpgsqlCommand command = this.connection.CreateCommand();
            command.CommandText = @"SELECT * FROM posts WHERE id = @id";
            command.Parameters.AddWithValue("@id", id);

            NpgsqlDataReader reader = command.ExecuteReader();

            bool result = reader.Read();
            return result;
        }
        public void Insert(Post post) 
        {
            NpgsqlCommand command = this.connection.CreateCommand();
            command.CommandText = 
            @"
                INSERT INTO public.posts(authorid, heading, post, author)
                VALUES (@authorId, @heading, @post, @author);
            ";
            command.Parameters.AddWithValue("@heading", post.heading);
            command.Parameters.AddWithValue("@post", post.post);
            command.Parameters.AddWithValue("@author", post.author);
            command.Parameters.AddWithValue("@authorId", post.authorId);
            command.ExecuteNonQuery();
        }
        public long CountPosts()
        {
            NpgsqlCommand command = this.connection.CreateCommand();
            command.CommandText = @"SELECT COUNT(*) FROM posts";
            return (long)command.ExecuteScalar();
        }
        public long CountMySearchPosts(int authorId, string searchValue)
        {
            NpgsqlCommand command = this.connection.CreateCommand();
            command.CommandText = @"SELECT COUNT(*) FROM posts WHERE authorId = @authorId AND (heading LIKE @filter OR post LIKE @filter)";
            command.Parameters.AddWithValue("@authorId", authorId);
            command.Parameters.AddWithValue("@filter", '%' + searchValue + '%');
            return (long)command.ExecuteScalar();
        }
        public long CountMyPosts(int authorId)
        {
            NpgsqlCommand command = this.connection.CreateCommand();
            command.CommandText = @"SELECT COUNT(*) FROM posts WHERE authorId = @authorId";
            command.Parameters.AddWithValue(@"authorId", authorId);
            return (long)command.ExecuteScalar();
        }
        public Post GetById(int id) 
        { 
            NpgsqlCommand command = this.connection.CreateCommand();
            command.CommandText = @"SELECT * FROM posts WHERE id = @id";
            command.Parameters.AddWithValue("@id", id);
            
            NpgsqlDataReader reader = command.ExecuteReader();
            
            if (reader.Read())
            {
                Post po = ReadPost(reader);
                reader.Close();
                return po;
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
            command.CommandText = @"DELETE FROM posts WHERE id = @id";
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
        public List<Post> GetByUserId(int id)
        {
            NpgsqlCommand command = this.connection.CreateCommand();
            command.CommandText = @"SELECT * FROM posts WHERE authorId = @id";
            command.Parameters.AddWithValue("@id", id);
            
            NpgsqlDataReader reader = command.ExecuteReader();
            List<Post> list = new List<Post>();  
            while(reader.Read())
            {
                Post po = ReadPost(reader);
                list.Add(po);
            };
            reader.Close();
            return list;
        }
        public int UpdateData(int id, string heading, string post)
        {
            NpgsqlCommand command = this.connection.CreateCommand();
            command.CommandText = @"UPDATE posts SET heading = @heading, post = @post WHERE id = @id";
            
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@heading", heading);
            command.Parameters.AddWithValue("@post", post);

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
        static Post ReadPost(NpgsqlDataReader reader)
        {
            Post Post = new Post(){
                author = reader.GetString(4),
                heading = reader.GetString(2),
                post = reader.GetString(3),
                authorId = Convert.ToInt32(reader.GetInt32(1)),
            };
            Post.id = Convert.ToInt32(reader.GetInt32(0));
            
            return Post;
        }
        public List<Post> GetPage(int pageNumber, int pageSize) 
        {
            NpgsqlCommand command = this.connection.CreateCommand();
            command.CommandText = @"SELECT * FROM posts LIMIT @pageSize OFFSET @pageSize * (@pageNumber - 1)";
            command.Parameters.AddWithValue(@"pageSize", pageSize);
            command.Parameters.AddWithValue(@"pageNumber", pageNumber);

            List<Post> list = ReadPostsFromCommand(command);

            return list;
        }
        public List<Post> GetMyPage(int pageNumber, int pageSize, int authorId) 
        {
            NpgsqlCommand command = this.connection.CreateCommand();
            command.CommandText = @"SELECT * FROM posts WHERE authorId = @authorId LIMIT @pageSize OFFSET @pageSize * (@pageNumber - 1)";
            command.Parameters.AddWithValue(@"pageSize", pageSize);
            command.Parameters.AddWithValue(@"pageNumber", pageNumber);
            command.Parameters.AddWithValue(@"authorId", authorId);

            List<Post> list = ReadPostsFromCommand(command);

            return list;
        }
        static List<Post> ReadPostsFromCommand(NpgsqlCommand command)
        {
            NpgsqlDataReader reader;
            reader = command.ExecuteReader();
            List<Post> list = new List<Post>();
            while (reader.Read())
            {
                Post post = ReadPost(reader);
                list.Add(post);
            }
            reader.Close();
            return list;
        }
    }
}
