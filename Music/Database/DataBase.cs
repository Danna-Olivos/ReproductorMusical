using System;
using System.Data.SQLite;
using System.IO;

namespace Database
{
    public class DataBase
    {
        private static DataBase? instance = null;
        private SQLiteConnection connection{get;set;}

        //creating database
        private DataBase()
        {
            string dataPath = "./Music/Database/Data/MyMusic.db";
            bool dataExists = File.Exists(dataPath);
            string connectionString = $"Source={dataPath};Version=3;";
           
            connection = new SQLiteConnection(connectionString);
            connection.Open();
            Console.WriteLine("Connection is now opened");

            using (var command = new SQLiteCommand("PRAGMA foreign_keys = ON;", connection))
            {
                command.ExecuteNonQuery();
            }
            
            if (!dataExists)
            {
                CreateTables();
            }
        }

        //singleton to ensure only one database
        public static DataBase Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new DataBase();
                }
                return instance;
            }
        }

        //create tables for database
        private void CreateTables()
        {
            string createTablesQuery = @"
            CREATE TABLE types (
                id_type INTEGER PRIMARY KEY,
                description TEXT
            );

            INSERT INTO types VALUES(0, 'Person');
            INSERT INTO types VALUES(1, 'Group');
            INSERT INTO types VALUES(2, 'Unknown');

            CREATE TABLE performers (
                id_performer INTEGER PRIMARY KEY,
                id_type INTEGER,
                name TEXT,
                FOREIGN KEY (id_type) REFERENCES types(id_type)
            );

            CREATE TABLE persons (
                id_person INTEGER PRIMARY KEY,
                stage_name TEXT,
                real_name TEXT,
                birth_date TEXT,
                death_date TEXT
            );

            CREATE TABLE groups (
                id_group INTEGER PRIMARY KEY,
                name TEXT,
                start_date TEXT,
                end_date TEXT
            );

            CREATE TABLE in_group (
                id_person INTEGER,
                id_group INTEGER,
                PRIMARY KEY (id_person, id_group),
                FOREIGN KEY (id_person) REFERENCES persons(id_person),
                FOREIGN KEY (id_group) REFERENCES groups(id_group)
            );

            CREATE TABLE albums (
                id_album INTEGER PRIMARY KEY,
                path TEXT,
                name TEXT,
                year INTEGER
            );

            CREATE TABLE rolas (
                id_rola INTEGER PRIMARY KEY,
                id_performer INTEGER,
                id_album INTEGER,
                path TEXT,
                title TEXT,
                track INTEGER,
                year INTEGER,
                genre TEXT,
                FOREIGN KEY (id_performer) REFERENCES performers(id_performer),
                FOREIGN KEY (id_album) REFERENCES albums(id_album)
            );
        ";

        using (var command = new SQLiteCommand(createTablesQuery, connection))
        {
            command.ExecuteNonQuery();
        }
        Console.WriteLine("Data base created successfully");
        }

        //make, update, remove, retreive PERFORMERS
        public bool MakePerformer (Performer performer)
        {
            bool added = false;
            try
            {
                string query = "INSERT OR IGNORE INTO performers (id_type, name)" +
                                "VALUES (@id_type, @name)";
                
                using(SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id_type", performer.type.IdType);
                    command.Parameters.AddWithValue("@name", performer.Name);
                    int rowsAffected = command.ExecuteNonQuery();

                    if(rowsAffected > 0){
                        added = true;
                    }
        
                }
                Console.WriteLine("Performer added");
                return added;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while inserting performer: " + ex.Message);
            }
            return added;
        }

        public bool UpdatePerformer (Performer performer)
        {
            bool updated = false;
            try
            {
                string query = "UPDATE performers set id_type = @id_type, name = @name where id_performer = @id_performer" ;
                
                using(SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id_performer", performer.IdPerformer);
                    command.Parameters.AddWithValue("@id_type", performer.type.IdType);
                    command.Parameters.AddWithValue("@name", performer.Name);
                    int rowsAffected = command.ExecuteNonQuery();

                    if(rowsAffected > 0){
                        updated = true;
                    }
        
                }
                Console.WriteLine("Performer updated");
                return updated;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while updating performer: " + ex.Message);
            }
            return updated;
        }

        public bool RemovePerformer (Performer performer)
        {
            bool removed = false;
            try
            {
                string query = "DELETE from performers where id_performer = @id_performer" ;
                
                using(SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id_performer", performer.IdPerformer);
                    int rowsAffected = command.ExecuteNonQuery();

                    if(rowsAffected > 0){
                        removed = true;
                    }
        
                }
                Console.WriteLine("Performer removed");
                return removed;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while removing performer: " + ex.Message);
            }
            return removed;
        }

        // public Performer RetreivePerformer(string name)
        // {
        //     //luego vemos xd, hay que checar si se deben de indexar otras columnas para hacer las consultas
        // }

        //make, update, remove, retreive PERSONS
        public bool MakePerson (Person person)
        {
            bool added = false;
            try
            {
                string query = "INSERT OR IGNORE INTO persons (stage_name, real_name, birth_date, death_date)" +
                                "VALUES (@stage_name, @real_name, @birth_date, @death_date)";
                
                using(SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@stage_name", person.StageName);
                    command.Parameters.AddWithValue("@real_name", person.RealName);
                    command.Parameters.AddWithValue("@birth_date", person.BirthDate);
                    command.Parameters.AddWithValue("@death_date", person.DeathDate);
                    int rowsAffected = command.ExecuteNonQuery();

                    if(rowsAffected > 0){
                        added = true;
                    }
        
                }
                Console.WriteLine("Performer added");
                return added;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while inserting performer: " + ex.Message);
            }
            return added;
        }

        public bool UpdatePerson (Person person)
        {
            bool added = false;
            try
            {
                string query = "UPDATE persons stage_name=@stage_name, real_name= @real_name, birth_date=@birth_date, death_date=@death_date where id_person = @id_person";
                
                using(SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id_person", person.IdPerson);
                    command.Parameters.AddWithValue("@stage_name", person.StageName);
                    command.Parameters.AddWithValue("@real_name", person.RealName);
                    command.Parameters.AddWithValue("@birth_date", person.BirthDate);
                    command.Parameters.AddWithValue("@death_date", person.DeathDate);
                    int rowsAffected = command.ExecuteNonQuery();

                    if(rowsAffected > 0){
                        added = true;
                    }
        
                }
                Console.WriteLine("Person updated");
                return added;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while updating performer: " + ex.Message);
            }
            return added;
        }

        public bool RemovePerson (Person person)
        {
            bool added = false;
            try
            {
                string query = "DELETE from persons where id_person = @id_person";
                
                using(SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id_person", person.IdPerson);
                    int rowsAffected = command.ExecuteNonQuery();

                    if(rowsAffected > 0){
                        added = true;
                    }
        
                }
                Console.WriteLine("Person removed");
                return added;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while removing performer: " + ex.Message);
            }
            return added;
        }

        //Disconnection method        
        public void Disconnect()
        {
            if (connection != null && connection.State == System.Data.ConnectionState.Open)
            {
                connection.Close();
                Console.WriteLine("Connection closed");
            }
        }
    }
    
}
