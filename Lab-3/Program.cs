using System;
using Npgsql;
using System.Collections.Generic;
using System.Diagnostics;

namespace Lab_3
{
    class Program
    {
        static void PrintInfo()
        {
            Console.WriteLine(">>>COMMANDLIST:");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("getUsers");
            Console.WriteLine("getUser [id]");
            Console.WriteLine("createUser [username] [password] [fullname] [acctype]");
            Console.WriteLine("deleteUser [id]");
            Console.WriteLine("changeUserAcctypeData [username] [new_acctype]");
            Console.WriteLine("changeUserPassword [username] [new_password]");
            Console.WriteLine("changeUsername [username] [new_username]");
            Console.WriteLine("searchUsers [searchValue]");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("getPosts");
            Console.WriteLine("getPost [id]");
            Console.WriteLine("getPostsByUserId [userId]");
            Console.WriteLine("createPost [authorId] [heading] [post] [author]");
            Console.WriteLine("deletePost [id]");
            Console.WriteLine("changePost [id] [heading] [post]");
            Console.WriteLine("searchPosts [searchValue]");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("getComments");
            Console.WriteLine("getComment [id]");
            Console.WriteLine("getCommentsByUserId [userId]");
            Console.WriteLine("getCommentsByPostId [postId]");
            Console.WriteLine("createComment [postId] [authorId] [comment]");
            Console.WriteLine("deleteComment [id]");
            Console.WriteLine("changeComment [id] [comment]");
            Console.WriteLine("searchComments [searchValue]");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("random [nrows]");
            Console.WriteLine("exit");
            Console.ForegroundColor = ConsoleColor.White;
        }
        static void UserEditProcess(string[] parts, UserRepository ur, PostRepository pr, CommentRepository cr)
        {
            if(parts[0] == "getUsers" && parts.Length == 1)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Users list:");
                foreach(User u in ur.GetAll())
                {
                    Console.WriteLine(u);
                }
                Console.ForegroundColor = ConsoleColor.White;
            }
            else if(parts[0] == "getUser" && parts.Length == 2)
            {
                int id;
                bool success = int.TryParse(parts[1], out id);
                if(success && id > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    User u = ur.GetById(id);
                    if(u == default)
                    {
                        Console.WriteLine("Can't find user");
                    }
                    else
                    {
                        Console.WriteLine(ur.GetById(id));
                    }
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Id must be integer and > 0");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
            else if(parts[0] == "createUser" && parts.Length == 5)
            {
                if(ur.UserExists(parts[1]))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"User with username:{parts[1]} already exist");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    User user = new User()
                    {
                        username = parts[1],
                        password_hash = parts[2],
                        fullname = parts[3],
                        acctype = parts[4]
                    };
                    Console.WriteLine($"User was added");
                    ur.Insert(user);
                }
            }
            else if(parts[0] == "deleteUser" && parts.Length == 2)
            {
                int id;
                bool success = int.TryParse(parts[1], out id);
                if(success && id > 0)
                {

                    if(ur.DeleteById(id) == 1)
                    {
                        List<Post> userpostlist = pr.GetByUserId(id);
                        List<Comment> usercommentlist = cr.GetByAuthorId(id);
                        foreach(Post p in userpostlist)
                        {
                            cr.DeleteByPostId(p.id);
                            pr.DeleteById(p.id);
                        }
                        foreach(Comment c in usercommentlist)
                        {
                            cr.DeleteById(c.id);
                        }
                        Console.WriteLine($"User was deleted");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Can't find user with userID:{id}");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Id must be integer and > 0");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
            else if(parts[0] == "changeUserAcctypeData" && parts.Length == 3)
            {
                if(ur.ChangeAcctypeData(parts[1], parts[2]) == 1)
                {
                    Console.WriteLine("Acctype data was changed");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Can't find user with username:{parts[1]}");
                    Console.ForegroundColor = ConsoleColor.White;
                }

            }
            else if(parts[0] == "changeUserPassword" && parts.Length == 3)
            {
                if(ur.ChangePassword(parts[1], parts[2]) == 1)
                {
                    Console.WriteLine("Password was changed");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Can't find user with username:{parts[1]}");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
            else if(parts[0] == "changeUsername" && parts.Length == 3)
            {
                if(ur.ChangeUsername(parts[1], parts[2]) == 1)
                {
                    Console.WriteLine("Username was changed");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Can't find user with username:{parts[1]}");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
            else if(parts[0] == "searchUsers" && parts.Length == 2)
            {
                Console.WriteLine("Founded results:");
                foreach(User u in ur.SearchUsers(parts[1]))
                {
                    Console.WriteLine(u);
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Invalid command");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
        static void PostEditProcess(string[] parts, PostRepository pr, CommentRepository cr)
        {
            if(parts[0] == "getPosts" && parts.Length == 1)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Posts list:");
                foreach(Post p in pr.GetAll())
                {
                    Console.WriteLine(p);
                }
                Console.ForegroundColor = ConsoleColor.White;
            }
            else if(parts[0] == "getPost" && parts.Length == 2)
            {
                int id;
                bool success = int.TryParse(parts[1], out id);
                if(success && id > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Post p = pr.GetById(id);
                    if(p == default)
                    {
                        Console.WriteLine("Can't find post");
                    }
                    else
                    {
                        Console.WriteLine(pr.GetById(id));
                    }
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Id must be integer and > 0");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
            else if(parts[0] == "getPostByUserId" && parts.Length == 2)
            {
                int id;
                bool success = int.TryParse(parts[1], out id);
                if(success && id > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    List<Post> posts = pr.GetByUserId(id);
                    Console.WriteLine($"Posts by author with id:{id}");
                    foreach(Post p in posts)
                    {
                        Console.WriteLine(p);
                    }
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Id must be integer and > 0");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
            else if(parts[0] == "createPost" && parts.Length == 5)
            {
                int id;
                bool success = int.TryParse(parts[1], out id);
                if(success && id > 0)
                {
                    Post post = new Post()
                    {
                        authorId = Convert.ToInt32(parts[1]),
                        heading = parts[2],
                        post = parts[3],
                        author = parts[4]
                    };
                    Console.WriteLine($"Post was added");
                    pr.Insert(post);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Id must be integer and > 0");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
            else if(parts[0] == "deletePost" && parts.Length == 2)
            {
                int id;
                bool success = int.TryParse(parts[1], out id);
                if(success && id > 0)
                {

                    if(pr.DeleteById(id) == 1)
                    {
                        cr.DeleteByPostId(id);
                        Console.WriteLine($"Post was deleted");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Can't find post with ID:{id}");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Id must be integer and > 0");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
            else if(parts[0] == "changePost" && parts.Length == 4)
            {
                int id;
                bool success = int.TryParse(parts[1], out id);
                if(success && id > 0)
                {
                    if(pr.UpdateData(id, parts[2], parts[3]) == 1)
                    {
                        Console.WriteLine("Post data was changed");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Can't find post with id:{parts[1]}");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Id must be integer and > 0");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
            else if(parts[0] == "searchPosts" && parts.Length == 2)
            {
                Console.WriteLine("Founded results:");
                foreach(Post p in pr.SearchPosts(parts[1]))
                {
                    Console.WriteLine(p);
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Invalid command");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
        static void CommentEditProcess(string[] parts, CommentRepository cr)
        {
            if(parts[0] == "getComments" && parts.Length == 1)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Comments list:");
                foreach(Comment c in cr.GetAll())
                {
                    Console.WriteLine(c);
                }
                Console.ForegroundColor = ConsoleColor.White;
            }
            else if(parts[0] == "getComments" && parts.Length == 2)
            {
                int id;
                bool success = int.TryParse(parts[1], out id);
                if(success && id > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Comment c = cr.GetById(id);
                    if(c == default)
                    {
                        Console.WriteLine("Can't find comment");
                    }
                    else
                    {
                        Console.WriteLine(cr.GetById(id));
                    }
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Id must be integer and > 0");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
            else if(parts[0] == "getCommentsByUserId" && parts.Length == 2)
            {
                int id;
                bool success = int.TryParse(parts[1], out id);
                if(success && id > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    List<Comment> comments = cr.GetByAuthorId(id);
                    Console.WriteLine($"Comments by author with id:{id}");
                    foreach(Comment c in comments)
                    {
                        Console.WriteLine(c);
                    }
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Id must be integer and > 0");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
            else if(parts[0] == "getCommentsByPostId" && parts.Length == 2)
            {
                int id;
                bool success = int.TryParse(parts[1], out id);
                if(success && id > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    List<Comment> comments = cr.GetByPostId(id);
                    Console.WriteLine($"Comments by post with id:{id}");
                    foreach(Comment c in comments)
                    {
                        Console.WriteLine(c);
                    }
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Id must be integer and > 0");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
            else if(parts[0] == "createComment" && parts.Length == 4)
            {
                int postid;
                bool success1 = int.TryParse(parts[1], out postid);
                int authorid;
                bool success2 = int.TryParse(parts[1], out authorid);
                if(success1 && postid > 0 && success2 && authorid > 0)
                {
                    Comment comment = new Comment()
                    {
                        authorId = Convert.ToInt32(parts[2]),
                        postId = Convert.ToInt32(parts[1]),
                        comment = parts[3]
                    };
                    Console.WriteLine($"Comment was added");
                    cr.Insert(comment);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Id must be integer and > 0");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
            else if(parts[0] == "deleteComment" && parts.Length == 2)
            {
                int id;
                bool success = int.TryParse(parts[1], out id);
                if(success && id > 0)
                {

                    if(cr.DeleteById(id) == 1)
                    {
                        Console.WriteLine($"Comment was deleted");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Can't find comment with ID:{id}");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Id must be integer and > 0");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
            else if(parts[0] == "changeComment" && parts.Length == 3)
            {
                int id;
                bool success = int.TryParse(parts[1], out id);
                if(success && id > 0)
                {
                    if(cr.UpdateData(id, parts[2]) == 1)
                    {
                        Console.WriteLine("Comment data was changed");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Can't find comment with id:{parts[1]}");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Id must be integer and > 0");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
            else if(parts[0] == "searchComments" && parts.Length == 2)
            {
                Console.WriteLine("Founded results:");
                foreach(Comment c in cr.SearchComments(parts[1]))
                {
                    Console.WriteLine(c);
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Invalid command");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
        static void Main(string[] args)
        {
            Stopwatch sWatch = new Stopwatch();
            string connect_parameters = "Server=localhost;Port=5432;User Id=postgres;Password=admin;Database=facebook;";
            NpgsqlConnection connection = new NpgsqlConnection(connect_parameters);
            connection.Open();
            UserRepository ur = new UserRepository(connection);
            PostRepository pr = new PostRepository(connection);
            CommentRepository cr = new CommentRepository(connection);
            PrintInfo();
            string command = "";
            while(command != "exit")
            {
                command = Console.ReadLine();
                string[] parts = command.Split(' ');
                if(parts[0] == "random" && parts.Length == 2)
                {
                    int nrows;
                    bool success = int.TryParse(parts[1], out nrows);
                    if(success && nrows > 0)
                    {
                        sWatch.Reset();
                        sWatch.Start();
                        //Console.WriteLine(nrows);
                        NpgsqlCommand comm = connection.CreateCommand();
                        comm.CommandText = 
                        @"
                            SELECT fillrandomvalues(@nrows)
                        ";
                        comm.Parameters.AddWithValue("@nrows", nrows);
                        comm.ExecuteNonQuery();
                        sWatch.Stop();
                        Console.WriteLine ($"Действие выполнено,затраченое время: {sWatch.ElapsedMilliseconds.ToString()} мс");
                        continue;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Nrows must be integer and > 0");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
                string[] usersCommand = new string[]{"getUsers", "getUser", "createUser", "deleteUser", "changeUserAcctypeData", "changeUserPassword", "changeUsername", "searchUsers"};
                string[] postsCommand = new string[]{"getPosts", "getPost", "getPostsByUserId", "createPost", "deletePost", "changePost", "searchPosts"};
                string[] commentsCommand = new string[]{"getComments", "getComment", "getCommentsByUserId", "getCommentsByPostId", "createComment", "deleteComment", "changeComment", "searchComments"};
                bool marker = false;
                foreach(string i in usersCommand)
                {
                    if(parts[0] == i)
                    {
                        marker = true;
                        sWatch.Reset();
                        sWatch.Start();
                        UserEditProcess(parts, ur, pr, cr);
                        sWatch.Stop();
                        Console.WriteLine ($"Действие выполнено,затраченое время: {sWatch.ElapsedMilliseconds.ToString()} мс");
                    }
                }
                foreach(string i in postsCommand)
                {
                    if(parts[0] == i)
                    {
                        marker = true;
                        sWatch.Start();
                        PostEditProcess(parts, pr, cr);
                        sWatch.Stop();
                        Console.WriteLine ($"Действие выполнено,затраченое время: {sWatch.ElapsedMilliseconds.ToString()} мс");
                    }
                }
                foreach(string i in commentsCommand)
                {
                    if(parts[0] == i)
                    {
                        marker = true;
                        sWatch.Start();
                        CommentEditProcess(parts, cr);
                        sWatch.Stop();
                        Console.WriteLine ($"Действие выполнено,затраченое время: {sWatch.ElapsedMilliseconds.ToString()} мс");
                    }
                }
                if(command != "exit" && marker == false)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid Command");
                    Console.ForegroundColor = ConsoleColor.White;                    
                }
            }
            connection.Close();
        }
    }
}
