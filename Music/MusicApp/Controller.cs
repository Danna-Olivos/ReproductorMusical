using Database;
using TagLib.Matroska;

namespace MusicApp
{
    public class Controller
    {
        private DataBase db = DataBase.Instance;
        private Minero miner;
        private string configPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".config", "CapyMusic", "config.txt");
        private string path{get;set;}
 
        public Controller()
        {
            path = ConfigPath();
            miner = new Minero();
        }

        //obtener direccion del config
        private string ConfigPath()
        {
            string configDirectory = Path.GetDirectoryName(configPath);
            if (!Directory.Exists(configDirectory)) //verificar que existe la carpeta de la app
            {
                Directory.CreateDirectory(configDirectory);//si no existe, la crea
                Console.WriteLine($"Directory '{configDirectory}' created.");
            }
            if (System.IO.File.Exists(configPath))
            {
                string pathFromFile = System.IO.File.ReadAllText(configPath).Trim();
                if(Directory.Exists(pathFromFile)) return pathFromFile;
            }
            string defaultPath = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            if(string.IsNullOrWhiteSpace(defaultPath))
                defaultPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Music");
            if(!Directory.Exists(defaultPath)) 
                Directory.CreateDirectory(defaultPath);
            System.IO.File.WriteAllText(configPath, defaultPath);//new file
            return defaultPath;
        }

        //change path 
        public bool ChangePath(string newPath)
        {
            if(!Directory.Exists(newPath))
            {
                return false; //mandar mensaje de no mames no existe
            }
            path = newPath;
            System.IO.File.WriteAllText(configPath, path);
            return true;
        }
        
        //Mina desde el path asignado
        public void StartMining()
        {
            miner.Mine(path);
        }

        //Retreive info from each song to display when mined
        public List<(string Title, string Performer, string Album, string Path)> ShowSongList()
        {
 
            List<(string Title, string Performer, string Album, string Path)> songList = new List<(string, string, string, string)>();
            List<Songs> availableSongs = db.ListSongs();

            foreach (Songs song in availableSongs)
            {
                string performerName = GetSongPerformer(song.IdPerformer);
                string albumName = GetSongAlbum(song.IdAlbum);
                songList.Add((song.Title, performerName, albumName,song.Path));
            }
            return songList;
        }


        public string GetSongPerformer(int id)
        {
            Performer? performer = db.RetreivePerformer(id);
            return performer.Name;
        }

        public string GetSongAlbum(int id)
        {
            Albums? album = db.RetreiveAlbum(id);
            return album.Name;
        }

        //Retreive info from each song to display when selected
        public (int idS, int idP, int idA, string path,string title, int year, int track, string genre) GetSongInfo(string songPath)
        {
            int songID = db.GetSongId(songPath);
            Songs? song = db.RetreiveRola(songID);

            if(song == null) throw new Exception ($"Song with name{songPath} not found");
            int idPerson = song.IdPerformer;
            int idAlbum = song.IdAlbum;
            string pathS = song.Path;
            string titleS = song.Title;
            int yearS = song.Year;
            int trackS = song.Track;
            string genreS = song.Genre;

            return (songID, idPerson, idAlbum, pathS, titleS, yearS, trackS, genreS);
        }

        //change info from a song (changes database and metadata)
        public void EditSong(string songPath, int songID,string newTitle, string newGenre, string newTrack, string performerName, string newYear, string albumName)
        {
            Songs? songToUpdate = db.RetreiveRola(songID);

            if (songToUpdate == null) return;
            
            int performerID = db.GetPerformerId(performerName); // si no existe, el metodo ya se encarga de crear
            Performer? newPerformer = db.RetreivePerformer(performerID); 

            songToUpdate.IdPerformer = newPerformer.IdPerformer;

            int albumID = db.GetAlbumId(albumName, int.Parse(newYear), path); // si no existe, el metodo ya se encarga de crear
            Albums? newAlbum = db.RetreiveAlbum(albumID);

            songToUpdate.IdAlbum = newAlbum.IdAlbum;

            if (!string.IsNullOrEmpty(newTitle)) songToUpdate.Title = newTitle;
            if (!string.IsNullOrEmpty(newGenre)) songToUpdate.Genre = newGenre;
            if (!string.IsNullOrEmpty(newTrack)) songToUpdate.Track = int.Parse(newTrack);
            if (!string.IsNullOrEmpty(newYear)) songToUpdate.Year = int.Parse(newYear);

            db.UpdateRolas(songToUpdate); //update in database

            UpdateMetadata(songToUpdate); //update mp3
        }

        //Retreive album info 
        public void GetAlbumInfo(string albumName)
        {

        }

        //change album info(changes database and metadata)
        public void EditAlbum(string albumName, int year, string filePath)
        {
            int albumID = db.GetAlbumId(albumName, year, filePath);
            Albums? album = db.RetreiveAlbum(albumID);

            db.UpdateAlbums(album);
       
        }

        private void UpdateMetadata(Songs rola)
        {
            var file = TagLib.File.Create(rola.Path);
            file.Tag.Title = rola.Title;
            file.Tag.Performers = new[] { GetSongPerformer(rola.IdPerformer) };
            file.Tag.Album = GetSongAlbum(rola.IdAlbum);
            file.Tag.Year = (uint)rola.Year;
            file.Tag.Track = (uint)rola.Track;
            file.Tag.Genres = new[] { rola.Genre};
            file.Save();
        }

        //also should be able to edit all the shit from the other objects
        //make group 
        //define as group or as person
        //consultas :=C

        


    }
}