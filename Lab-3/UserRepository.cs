using Npgsql;
using System;
using static System.Math;
using System.Collections.Generic;

namespace Lab_3
{
    public class UserRepository
    {
        private NpgsqlConnection connection;
        public UserRepository(NpgsqlConnection connection)
        {
            this.connection = connection;
        }

        public List<User> GetAll()
        {
            NpgsqlCommand command = this.connection.CreateCommand();
            command.CommandText = @"SELECT * FROM users";
            List<User> list = ReadUsersFromCommand(command);

            return list;
        }
        public void Insert(User user) 
        {
            NpgsqlCommand command = this.connection.CreateCommand();
            command.CommandText = 
            @"
                INSERT INTO users (password_hash, fullname, acctype, username) 
                VALUES (@password_hash, @fullname, @acctype, @username);
            ";
            command.Parameters.AddWithValue("@username", user.username);
            command.Parameters.AddWithValue("@password_hash", user.password_hash);
            command.Parameters.AddWithValue("@fullname", user.fullname);
            command.Parameters.AddWithValue("@acctype", user.acctype);
            command.ExecuteNonQuery();
        }
        public int GetTotalPages(int size) 
        {
            long nOperators = CountUsers();
            double pageSize = size;
            return (int)Ceiling(nOperators / pageSize);
        }
        public List<User> GetPage(int pageNumber, int pageSize) 
        {
            NpgsqlCommand command = this.connection.CreateCommand();
            command.CommandText = @"SELECT * FROM users LIMIT @pageSize OFFSET @pageSize * (@pageNumber - 1)";
            command.Parameters.AddWithValue("@pageSize", pageSize);
            command.Parameters.AddWithValue("@pageNumber", pageNumber);

            List<User> list = ReadUsersFromCommand(command);

            return list;
        }
        public int GetSearchTotalPages(int size, string searchValue)
        {
            if(string.IsNullOrEmpty(searchValue))
            {
                return this.GetTotalPages(size);
            }
            return (int)Ceiling(this.SearchUsers(searchValue).Count / (double)size);
        }
        public List<User> GetSearchPage(int pageNumber, int pageSize, string searchValue)
        {
            if(string.IsNullOrEmpty(searchValue))
            {
                return this.GetPage(pageNumber, pageSize);
            }
            NpgsqlCommand command = this.connection.CreateCommand();
            command.CommandText = @"SELECT * FROM users WHERE username LIKE @filter OR fullname LIKE @filter LIMIT @pageSize OFFSET @pageSize * (@pageNumber - 1)";
            command.Parameters.AddWithValue("@pageSize", pageSize);
            command.Parameters.AddWithValue("@pageNumber", pageNumber);
            command.Parameters.AddWithValue("@filter", '%' + searchValue + '%');

            List<User> list = ReadUsersFromCommand(command);

            return list;
        }
        public bool UserExists(string username)
        {
            NpgsqlCommand command = this.connection.CreateCommand();
            command.CommandText = @"SELECT * FROM users WHERE username = @username";
            command.Parameters.AddWithValue("@username", username);
            NpgsqlDataReader reader = command.ExecuteReader();
            bool result = reader.Read();
            reader.Close();
            return result;
        }
        public long CountUsers()
        {
            NpgsqlCommand command = this.connection.CreateCommand();
            command.CommandText = @"SELECT COUNT(*) FROM users";
            return (long)command.ExecuteScalar();
        }
        public User GetByUsername(string username) 
        { 
            NpgsqlCommand command = this.connection.CreateCommand();
            command.CommandText = @"SELECT * FROM users WHERE username = @username";
            command.Parameters.AddWithValue("@username", username);
            
            NpgsqlDataReader reader = command.ExecuteReader();
            
            if (reader.Read())
            {
                User user = ReadUser(reader);
                reader.Close();
                return user;
            }
            else 
            {
                reader.Close();
                return null;
            } 
        }
        public User GetById(int id)
        {
            NpgsqlCommand command = this.connection.CreateCommand();
            command.CommandText = @"SELECT * FROM users WHERE id = @id";
            command.Parameters.AddWithValue("@id", id);
            
            NpgsqlDataReader reader = command.ExecuteReader();
            
            if (reader.Read())
            {
                User user = ReadUser(reader);
                reader.Close();
                return user;
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
            command.CommandText = @"DELETE FROM users WHERE id = @id";
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
        public int ChangeAcctypeData(string username, string acctype)
        {
            NpgsqlCommand command = this.connection.CreateCommand();
            command.CommandText = @"UPDATE users SET acctype = @acctype WHERE username = @username";
            
            command.Parameters.AddWithValue("@username", username);
            command.Parameters.AddWithValue("@acctype", acctype);

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
        public int ChangePassword(string username, string password_hash)
        {
            NpgsqlCommand command = this.connection.CreateCommand();
            command.CommandText = @"UPDATE users SET password_hash = @password_hash WHERE username = @username";
            
            command.Parameters.AddWithValue("@username", username);
            command.Parameters.AddWithValue("@password_hash", password_hash);

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
        public int ChangeUsername(string username, string newUsername)
        {
            NpgsqlCommand command = this.connection.CreateCommand();
            command.CommandText = @"UPDATE users SET username = @newUsername WHERE username = @username";
            
            command.Parameters.AddWithValue("@username", username);
            command.Parameters.AddWithValue("@newUsername", newUsername);

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
        public List<User> SearchUsers(string searchValue)
        {
            NpgsqlCommand command = this.connection.CreateCommand();
            command.CommandText = @"SELECT * FROM users WHERE username LIKE @filter OR fullname LIKE @filter";
            command.Parameters.AddWithValue("@filter", '%' + searchValue + '%');
            
            List<User> list = ReadUsersFromCommand(command);

            return list;
        }
        static List<User> ReadUsersFromCommand(NpgsqlCommand command)
        {
            NpgsqlDataReader reader;
            reader = command.ExecuteReader();
            List<User> list = new List<User>();
            while (reader.Read())
            {
                User user = ReadUser(reader);
                list.Add(user);
            }
            reader.Close();
            return list;
        }
        static User ReadUser(NpgsqlDataReader reader)
        {
            User User = new User(){
                        username = reader.GetString(4),
                        password_hash = reader.GetString(1),
                        fullname = reader.GetString(2), 
                        acctype = reader.GetString(3)
                    };
            User.id = Convert.ToInt32(reader.GetInt32(0));
            
            return User;
        }
    }
}
