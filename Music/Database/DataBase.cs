using System;
using System.Data.SQLite;
using System.IO;

namespace Database
{
    public class DataBase
    {
        private static DataBase? instance = null;
        private SQLiteConnection connection{get;set;}
        private static readonly string defaultPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CapyMusica", "MyMusic.db");


        //creating database
        private DataBase(string path) 
        {
            string? dataPath = Path.GetDirectoryName(path);
            if (path != ":memory:" && dataPath != null && !Directory.Exists(dataPath))
            {
                Directory.CreateDirectory(dataPath);
                Console.WriteLine($"Directory '{dataPath}' created.");
            }

            bool dataExists = File.Exists(path);
            string connectionString = $"Data Source={path};Version=3;";

            connection = new SQLiteConnection(connectionString);
            connection.Open();
            Console.WriteLine("Database connection open" + path);

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
                    instance = new DataBase(defaultPath);
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
                name TEXT UNIQUE,
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
                path TEXT UNIQUE,
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
                    command.Parameters.AddWithValue("@id_type", performer.Type);
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
                    command.Parameters.AddWithValue("@id_type", performer.Type);
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

        public int GetPerformerId(string performerName)
        {
            int id_performer = -1;
            try
            {
                string query = "SELECT id_performer FROM performers WHERE name = @name LIMIT 1";
                
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@name", performerName);
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            id_performer = reader.GetInt32(0);
                        }
                    }
                }
                if(id_performer == -1) //if its non existent
                {
                    using (SQLiteTransaction transaction = connection.BeginTransaction())
                    {
                        Performer performerObj = new Performer(performerName, Type.ArtistType.Unknown);
                        bool success = MakePerformer(performerObj); 

                        if (success)
                        {
                            query = "SELECT last_insert_rowid()";
                            using (SQLiteCommand command = new SQLiteCommand(query, connection))
                            {
                                id_performer = Convert.ToInt32(command.ExecuteScalar());
                            }
                        }

                        transaction.Commit();
                    }    
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error occurred while fetching performer '{performerName}': {ex.Message}");
            }
            return id_performer;
        }

        public List<Performer> ListPerformers()
        {
            List<Performer> performers = new List<Performer>();
            string query = "SELECT * FROM performers";
            
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int idPerformer = reader.GetInt32(0);
                        int idType = reader.GetInt32(1);
                        string name = reader.GetString(2);
                        
                        Performer performer = new Performer(idPerformer, name, (Type.ArtistType)idType);
                        performers.Add(performer);
                    }
                }
            }
            
            return performers;
        }

        public Performer RetreivePerformer(int id)
        {
            Performer? performer = null;
            string query = "SELECT * FROM performers WHERE id_performer = @id_performer";
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id_performer", id);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int idType = reader.GetInt32(1);
                        string name = reader.GetString(2);
                        performer = new Performer(id, name, (Type.ArtistType)idType);
                    }
                }
            }
            return performer;
        }

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
                string query = "UPDATE persons set stage_name=@stage_name, real_name= @real_name, birth_date=@birth_date, death_date=@death_date where id_person = @id_person";
                
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

        public List<Person> ListPersons()
        {
            List<Person> persons = new List<Person>();
            string query = "SELECT * FROM persons";
            
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int idPerson = reader.GetInt32(0);
                        string StageName = reader.GetString(1);
                        string RealName = reader.GetString(2);
                        string BirthDate = reader.GetString(3);
                        string DeathDate = reader.GetString(4);
                        
                        Person performer = new Person(idPerson,StageName,RealName,BirthDate,DeathDate);
                        persons.Add(performer);
                    }
                }
            }
            
            return persons;
        }

        //make, update, remove, retreive GROUPS
        public bool MakeGroup (Group group)
        {
            bool added = false;
            try
            {
                string query = "INSERT OR IGNORE INTO groups (name,start_date, end_date)" +
                                "VALUES (@name,@start_date, @end_date)";
                
                using(SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@name", group.GroupName);
                    command.Parameters.AddWithValue("@start_name", group.StartDate);
                    command.Parameters.AddWithValue("@end_date", group.EndDate);
                    int rowsAffected = command.ExecuteNonQuery();

                    if(rowsAffected > 0){
                        added = true;
                    }
        
                }
                Console.WriteLine("Group added");
                return added;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while inserting Group: " + ex.Message);
            }
            return added;
        }

        public bool UpdateGroup (Group group)
        {
            bool added = false;
            try
            {
                string query = "UPDATE groups set name = @name,start_date = @start_date, end_date = @end_date where id_group = @id_group" ;
                
                using(SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id_group", group.IdGroup);
                    command.Parameters.AddWithValue("@name", group.GroupName);
                    command.Parameters.AddWithValue("@start_name", group.StartDate);
                    command.Parameters.AddWithValue("@end_date", group.EndDate);
                    int rowsAffected = command.ExecuteNonQuery();

                    if(rowsAffected > 0){
                        added = true;
                    }
        
                }
                Console.WriteLine("Group updated");
                return added;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while updating Group: " + ex.Message);
            }
            return added;
        }

        public bool RemoveGroup (Group group)
        {
            bool added = false;
            try
            {
                string query = "DELETE from groups where id_group = @id_group" ;
                
                using(SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id_group", group.IdGroup);
                    int rowsAffected = command.ExecuteNonQuery();

                    if(rowsAffected > 0){
                        added = true;
                    }
        
                }
                Console.WriteLine("Group removed");
                return added;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while removing Group: " + ex.Message);
            }
            return added;
        }

        public List<Group> ListGroups()
        {
            List<Group> groups = new List<Group>();
            string query = "SELECT * FROM groups";
            
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int idGroup = reader.GetInt32(0);
                        string groupName = reader.GetString(1);
                        string startDate = reader.GetString(2);
                        string endDate = reader.GetString(3);
                        
                        Group gro = new Group(idGroup, groupName,startDate, endDate);
                        groups.Add(gro);
                    }
                }
            }
            return groups;
        }

        //make, update, remove, retreive ROLAS
        public bool MakeRolas (Songs rola)
        {
            bool added = false;
            try
            {
                string query = "INSERT OR IGNORE INTO rolas (id_performer, id_album, path, title, track, year, genre)" +
                                "VALUES (@id_performer, @id_album, @path, @title, @track, @year, @genre)";
                
                using(SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id_performer", rola.IdPerformer);
                    command.Parameters.AddWithValue("@id_album", rola.IdAlbum);
                    command.Parameters.AddWithValue("@path", rola.Path);
                    command.Parameters.AddWithValue("@title", rola.Title);
                    command.Parameters.AddWithValue("@track", rola.Track);
                    command.Parameters.AddWithValue("@year", rola.Year);
                    command.Parameters.AddWithValue("@genre", rola.Genre);
                    int rowsAffected = command.ExecuteNonQuery();

                    if(rowsAffected > 0){
                        added = true;
                    }
        
                }
                Console.WriteLine("Song added");
                return added;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while inserting Song: " + ex.Message);
            }
            return added;
        }

        public bool UpdateRolas (Songs rola)
        {
            bool added = false;
            try
            {
                string query = "UPDATE rolas set id_performer = @id_performer, id_album = @id_album, path = @path, title = @title, track = @track, year = @year, genre = @genre where id_rola = @id_rola";
                
                using(SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id_rola", rola.IdSong);
                    command.Parameters.AddWithValue("@id_performer", rola.IdPerformer);
                    command.Parameters.AddWithValue("@id_album", rola.IdAlbum);
                    command.Parameters.AddWithValue("@path", rola.Path);
                    command.Parameters.AddWithValue("@title", rola.Title);
                    command.Parameters.AddWithValue("@track", rola.Track);
                    command.Parameters.AddWithValue("@year", rola.Year);
                    command.Parameters.AddWithValue("@genre", rola.Genre);
                    int rowsAffected = command.ExecuteNonQuery();

                    if(rowsAffected > 0){
                        added = true;
                    }
        
                }
                Console.WriteLine("Song updated");
                return added;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while updated Song: " + ex.Message);
            }
            return added;
        }

        public bool RemoveRolas (Songs rola)
        {
            bool added = false;
            try
            {
                string query = "DELETE from rolas where id_rola = @id_rola";
                
                using(SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id_rola", rola.IdSong);
                    int rowsAffected = command.ExecuteNonQuery();

                    if(rowsAffected > 0){
                        added = true;
                    }
        
                }
                Console.WriteLine("Song removed");
                return added;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while removed Song: " + ex.Message);
            }
            return added;
        }

        public List<Songs> ListSongs()
        {
            List<Songs> songs = new List<Songs>();
            string query = "SELECT * FROM rolas";
            
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int idsong = reader.GetInt32(0);
                        int idPerformer = reader.GetInt32(1);
                        int idAlbum = reader.GetInt32(2);
                        string path = reader.GetString(3);
                        string title = reader.GetString(4);
                        int track = reader.GetInt32(5);
                        int year = reader.GetInt32(6);
                        string genre = reader.GetString(7); 
                        
                        Songs rolas = new Songs(idsong, idPerformer,idAlbum,path,title,track,year,genre);
                        songs.Add(rolas);
                    }
                }
            }
            return songs;
        }
        //make, update, remove, retreive IN_GROUP 
        public bool MakeInGroup (InGroup inG)
        {
            bool added = false;
            try
            {
                string query = "INSERT INTO in_groups (id_person, id_group)" +
                                "VALUES (@id_person, @id_group)";
                
                using(SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id_person", inG.IdPerson);
                    command.Parameters.AddWithValue("@id_group", inG.IdGroup);
                    int rowsAffected = command.ExecuteNonQuery();

                    if(rowsAffected > 0){
                        added = true;
                    }
        
                }
                Console.WriteLine("Added to a group");
                return added;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while adding to group: " + ex.Message);
            }
            return added;
        }

        //make, update, remove, retreive ALBUMS
        public bool MakeAlbums (Albums album)
        {
            bool added = false;
            try
            {
                string query = "INSERT OR IGNORE INTO albums (path, name, year)" +
                                "VALUES (@path, @name, @year)";
                
                using(SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@path", album.Path);
                    command.Parameters.AddWithValue("@name", album.Name);
                    command.Parameters.AddWithValue("@year", album.Year);
                    int rowsAffected = command.ExecuteNonQuery();

                    if(rowsAffected > 0){
                        added = true;
                    }
        
                }
                Console.WriteLine("Album added");
                return added;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while inserting Album: " + ex.Message);
            }
            return added;
        }

        public bool UpdateAlbums (Albums album)
        {
            bool added = false;
            try
            {
                string query = "UPDATE albums set path = @path, name = @name, year = @year where id_album = @id_album";
                
                using(SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id_album", album.IdAlbum);
                    command.Parameters.AddWithValue("@path", album.Path);
                    command.Parameters.AddWithValue("@name", album.Name);
                    command.Parameters.AddWithValue("@year", album.Year);
                    int rowsAffected = command.ExecuteNonQuery();

                    if(rowsAffected > 0){
                        added = true;
                    }
        
                }
                Console.WriteLine("Album removed");
                return added;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while updating Album: " + ex.Message);
            }
            return added;
        }

        public bool RemoveAlbums (Albums album)
        {
            bool added = false;
            try
            {
                string query = "Delete from albums where id_album = @id_album";
                
                using(SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id_album", album.IdAlbum);
                    int rowsAffected = command.ExecuteNonQuery();

                    if(rowsAffected > 0){
                        added = true;
                    }
        
                }
                Console.WriteLine("Album removed");
                return added;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while removing Album: " + ex.Message);
            }
            return added;
        }

        public int GetAlbumId(string albumName, int year, string filePath)
        {
            int id_album = -1;
            try
            {
            string query = "SELECT id_album FROM albums WHERE path = @path LIMIT 1";
            
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@path", filePath);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        id_album = reader.GetInt32(0);
                    }
                }
            }
            if(id_album == -1)
                {
                    using (SQLiteTransaction transaction = connection.BeginTransaction())
                    {
                        Albums albumsObj = new Albums(filePath, albumName, year);
                        bool success = MakeAlbums(albumsObj); 

                        if (success)
                        {
                            query = "SELECT last_insert_rowid()";
                            using (SQLiteCommand command = new SQLiteCommand(query, connection))
                            {
                                id_album = Convert.ToInt32(command.ExecuteScalar());
                            }
                        }

                        transaction.Commit();
                    }  
                }
            }
            catch(Exception ex)
            {
               Console.WriteLine($"Error occurred while fetching album '{albumName}' (year {year}): {ex.Message}"); 
            }
            return id_album;
        }

        public List<Albums> ListAlbums()
        {
            List<Albums> songs = new List<Albums>();
            string query = "SELECT * FROM albums";
            
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int idAlbum = reader.GetInt32(0);
                        string path = reader.GetString(1);
                        string name = reader.GetString(2);
                        int year = reader.GetInt32(3);

                        
                        Albums al = new Albums(idAlbum,path,name,year);
                        songs.Add(al);
                    }
                }
            }
            return songs;
        }

        public Albums RetreiveAlbum(int id)
        {
            Albums? album = null;
            string query = "SELECT * FROM albums WHERE id_album = @id_album";
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id_album", id);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string path = reader.GetString(1);
                        string name = reader.GetString(2);
                        int year = reader.GetInt32(3);
                        album = new Albums(id, path,name,year);
                    }
                }
            }
            return album;
        }


        //Disconnection db method        
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
