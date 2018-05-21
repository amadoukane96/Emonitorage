using System;
using System.Linq;
using System.Collections.Generic;

using Mono.Data.Sqlite;
using System.IO;
using System.Data;

namespace Emonitorage.shared
{
    public class AlarmDatabase
    {
        static object locker = new object();
        public SqliteConnection connection;
        public string path;

        /// <summary>
        /// On initialise une nouvelle instance de la base de donnees
        /// Si la bdd n existe pas on la cree avec les 2 tables.
        /// </summary>
        public AlarmDatabase(string dbPath)
        {
            path = dbPath;
            // create the tables
            bool exists = File.Exists(dbPath);

            if (!exists)
            {
                connection = new SqliteConnection("Data Source=" + dbPath);

                connection.Open();
                var commands = new[] {
                    "CREATE TABLE [Items] (_id INTEGER PRIMARY KEY ASC, id_Alarm INTEGER, Status INTEGER, Chambre NTEXT,Display NTEXT, Service INTEGER,Nom_PersonnelAidant NTEXT,Prenom_PersonnelAidant NTEXT, Nom_Occupant NTEXT, DtDebut NTEXT);"
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
            else
            {
                // already exists, do nothing. 
            }
        }

        // Ici on convertit une ligne de base de donnee en objet alarm
        AlarmItem FromReader(SqliteDataReader r)
        {
            var a = new AlarmItem();
            a.ID = Convert.ToInt32(r["_id"]);
            a.IDAlarm = Convert.ToInt32(r["id_Alarm"]);
            a.Status = Convert.ToInt32(r["Status"]) == 1 ? true : false;
            a.Service = Convert.ToInt32(r["Service"]);
            a.Chambre = r["Chambre"].ToString();
            a.Display = r["Display"].ToString();
            a.NomPersonnelAidant = r["Nom_PersonnelAidant"].ToString();
            a.PrenomPersonnelAidant = r["Prenom_PersonnelAidant"].ToString();
            a.NomOccupant = r["Nom_Occupant"].ToString();
            a.DtDebut = r["DtDebut"].ToString();
            return a;
        }

        // Ici on recupere toutes les lignes et appelle la fonction au dessus
        public IEnumerable<AlarmItem> GetItems()
        {
            var al = new List<AlarmItem>();

            lock (locker)
            {
                connection = new SqliteConnection("Data Source=" + path);
                connection.Open();
                using (var contents = connection.CreateCommand())
                {
                    contents.CommandText = "SELECT [_id],[id_Alarm],[Status],[Chambre],[Display],[Service],[Nom_PersonnelAidant],[Prenom_PersonnelAidant],[Nom_Occupant],[DtDebut] from [Items]";
                    var r = contents.ExecuteReader();
                    while (r.Read())
                    {
                        al.Add(FromReader(r));
                    }
                }
                connection.Close();
            }
            return al;
        }

        // Fonction pour recuperer une alarme en particulier de la base de donnees
        public AlarmItem GetItem(int id)
        {
            var a = new AlarmItem();
            lock (locker)
            {
                connection = new SqliteConnection("Data Source=" + path);
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT [_id],[id_Alarm],[Status],[Chambre],[Display],[Service],[Nom_PersonnelAidant],[Prenom_PersonnelAidant],[Nom_Occupant],[DtDebut] from [Items] WHERE [_id] = ?";
                    command.Parameters.Add(new SqliteParameter(DbType.Int32) { Value = id });
                    var r = command.ExecuteReader();
                    while (r.Read())
                    {
                        a = FromReader(r);
                        break;
                    }
                }
                connection.Close();
            }
            return a;
        }

        // Fonction pour changer le status d'une alarme
        public int ChangeStatus(int id, int nStatus)
        {
            lock (locker)
            {
                int r;
                connection = new SqliteConnection("Data Source=" + path);
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "UPDATE [Items] SET [Status] = ? WHERE [id_Alarm] = ?;";
                    command.Parameters.Add(new SqliteParameter(DbType.Int32) { Value = nStatus });
                    command.Parameters.Add(new SqliteParameter(DbType.Int32) { Value = id });
                    r = command.ExecuteNonQuery();
                }
                connection.Close();
                return r;
            }
        }

        // Fonction pour ajouter une alarme a la base de donnees
        public int AddItem(AlarmItem item)
        {
            int r;
            lock (locker)
            {
                if (item.ID != 0)
                {
                    connection = new SqliteConnection("Data Source=" + path);
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "UPDATE [Items] SET [Display]=?,[Prenom_PersonnelAidant]=?,[Service]=?, [id_Alarm] = ?, [Status] = ?, [Chambre] = ?, [Nom_PersonnelAidant], [Nom_Occupant], [DtDebut] = ? WHERE [_id] = ?;";
                        command.Parameters.Add(new SqliteParameter(DbType.String) { Value = item.Display });
                        command.Parameters.Add(new SqliteParameter(DbType.String) { Value = item.PrenomPersonnelAidant });
                        command.Parameters.Add(new SqliteParameter(DbType.Int32) { Value = item.Service });
                        command.Parameters.Add(new SqliteParameter(DbType.Int32) { Value = item.IDAlarm });
                        command.Parameters.Add(new SqliteParameter(DbType.Int32) { Value = item.Status });
                        command.Parameters.Add(new SqliteParameter(DbType.String) { Value = item.Chambre });
                        command.Parameters.Add(new SqliteParameter(DbType.String) { Value = item.NomPersonnelAidant });
                        command.Parameters.Add(new SqliteParameter(DbType.String) { Value = item.NomOccupant });
                        command.Parameters.Add(new SqliteParameter(DbType.String) { Value = item.DtDebut });
                        command.Parameters.Add(new SqliteParameter(DbType.Int32) { Value = item.ID });
                        r = command.ExecuteNonQuery();
                    }
                    connection.Close();
                    return r;
                }
                else
                {
                    connection = new SqliteConnection("Data Source=" + path);
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "INSERT INTO [Items] ([Display],[Prenom_PersonnelAidant],[Service],[id_Alarm], [Status], [Chambre], [Nom_PersonnelAidant], [Nom_Occupant], [DtDebut]) VALUES (?,?,?,?,?,?,?,?,?)";
                        command.Parameters.Add(new SqliteParameter(DbType.String) { Value = item.Display });
                        command.Parameters.Add(new SqliteParameter(DbType.String) { Value = item.PrenomPersonnelAidant });
                        command.Parameters.Add(new SqliteParameter(DbType.Int32) { Value = item.Service });
                        command.Parameters.Add(new SqliteParameter(DbType.Int32) { Value = item.IDAlarm });
                        command.Parameters.Add(new SqliteParameter(DbType.Int32) { Value = item.Status });
                        command.Parameters.Add(new SqliteParameter(DbType.String) { Value = item.Chambre });
                        command.Parameters.Add(new SqliteParameter(DbType.String) { Value = item.NomPersonnelAidant });
                        command.Parameters.Add(new SqliteParameter(DbType.String) { Value = item.NomOccupant });
                        command.Parameters.Add(new SqliteParameter(DbType.String) { Value = item.DtDebut });
                        r = command.ExecuteNonQuery();
                    }
                    Console.WriteLine("Je cree");
                    connection.Close();
                    return r;
                }

            }
        }

        // Fonction pour supprimer une alarme de la base de donnees
        public int DeleteItem(int id)
        {
            lock (locker)
            {
                int r;
                connection = new SqliteConnection("Data Source=" + path);
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "DELETE FROM [Items] WHERE [_id] = ?;";
                    command.Parameters.Add(new SqliteParameter(DbType.Int32) { Value = id });
                    r = command.ExecuteNonQuery();
                }
                connection.Close();
                return r;
            }
        }
        public int DeleteItems()
        {
            lock (locker)
            {
                int r;
                connection = new SqliteConnection("Data Source=" + path);
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "DELETE FROM [Items]";
                    r = command.ExecuteNonQuery();
                }
                connection.Close();
                return r;
            }
        }


        public int DeleteTable()
        {
            lock (locker)
            {
                int r;
                connection = new SqliteConnection("Data Source=" + path);
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "DROP TABLE [Items];";
                    r = command.ExecuteNonQuery();
                }
                connection.Close();
                return r;
            }

        }
    }
}