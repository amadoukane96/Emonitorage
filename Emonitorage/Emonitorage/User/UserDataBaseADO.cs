using System;
using System.Linq;
using System.Collections.Generic;

using Mono.Data.Sqlite;
using System.IO;
using System.Data;

namespace Emonitorage.shared
{
    class UserDataBase
    {
        static object locker = new object();
        public SqliteConnection connection;
        public string path;
        public UserDataBase(string dbPath)
        {
            path = dbPath;
        }
        public bool IsDB()
        {
            bool exists = File.Exists(path);
            return exists;
        }
        public void CreateDB()
        {
            connection = new SqliteConnection("Data Source=" + path);
            connection.Open();
            var commands = new[] {
                    "CREATE TABLE [User] (_id INTEGER PRIMARY KEY ASC,IdSS INTEGER,Firstname NTEXT,Lastname NTEXT, Is_Logged INTEGER, Profil INTEGER);"
                };
            foreach (var command in commands)
            {
                using (var c = connection.CreateCommand())
                {
                    c.CommandText = command;
                    c.ExecuteNonQuery();
                }
            }

        }

        User FromReader(SqliteDataReader r)
        {
            var u = new User();
            u.Id = Convert.ToInt32(r["_id"]);
            u.IdSS = Convert.ToInt32(r["IdSS"]);
            u.Firstname = r["Firstname"].ToString();
            u.Firstname = r["Lastname"].ToString();
            u.IsLogged = Convert.ToInt32(r["Is_Logged"]);
            u.Profil = Convert.ToInt32(r["Profil"]);
            return u;
        }

        public User GetUser(string username)
        {
            var u = new User();
            lock (locker)
            {
                connection = new SqliteConnection("Data Source=" + path);
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT [_id], [IdSS], [Firstname],[Lastname], [Is_Logged], [Profil] from [User] where _id=0";
                    var r = command.ExecuteReader();
                    while (r.Read())
                    {
                        u = FromReader(r);
                        break;
                    }
                }
                connection.Close();
            }
            return u;
        }

        public int ChangeStatus(string username, int nStatus)
        {
            lock (locker)
            {
                int r;
                connection = new SqliteConnection("Data Source=" + path);
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "UPDATE [User] SET [Is_Logged] = ? WHERE [_id] = 0;";
                    command.Parameters.Add(new SqliteParameter(DbType.Int32) { Value = 1 });
                    r = command.ExecuteNonQuery();
                }
                connection.Close();
                return r;
            }
        }

        public int AddUser(User item)
        {
            int r;
            lock (locker)
            {
                connection = new SqliteConnection("Data Source=" + path);
                connection.Open();
                connection = new SqliteConnection("Data Source=" + path);
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "INSERT INTO [User] ([_id],[IdSS],[Firstname],[LastName],[Is_Logged],[Profil]) VALUES (0 ,?,?,?,?,?)";
                    command.Parameters.Add(new SqliteParameter(DbType.Int32) { Value = item.IdSS });
                    command.Parameters.Add(new SqliteParameter(DbType.String) { Value = item.Firstname });
                    command.Parameters.Add(new SqliteParameter(DbType.String) { Value = item.Lastname });
                    command.Parameters.Add(new SqliteParameter(DbType.Int32) { Value = item.IsLogged });
                    command.Parameters.Add(new SqliteParameter(DbType.Int32) { Value = item.Profil });
                    r = command.ExecuteNonQuery();
                    connection.Close();
                    return r;

                }
            }


        }
    }
}
