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
        public List<(string Title, string Performer, string Album)> ShowSongList()
        {

            List<(string Title, string Performer, string Album)> songList = new List<(string, string, string)>();
            List<Songs> availableSongs = db.ListSongs();

            foreach (Songs song in availableSongs)
            {
                string performerName = GetSongPerformer(song.IdPerformer);
                string albumName = GetSongAlbum(song.IdAlbum);
                songList.Add((song.Title, performerName, albumName));
            }
            return songList;
        }


        private string GetSongPerformer(int id)
        {
            Performer? performer = db.RetreivePerformer(id);
            return performer.Name;
        }

        private string GetSongAlbum(int id)
        {
            Albums? album = db.RetreiveAlbum(id);
            return album.Name;
        }

        //Retreive info from each song to display when selected
        public (int idP, int idA, string path,string title, int year, int track, string genre, Songs s) GetSongInfo(string songName)
        {
            int songID = db.GetSongId(songName);
            Songs? song = db.RetreiveRola(songID);

            if(song == null) throw new Exception ($"Song with name{songName} not found");
            int idPerson = song.IdPerformer;
            int idAlbum = song.IdAlbum;
            string pathS = song.Path;
            string titleS = song.Title;
            int yearS = song.Year;
            int trackS = song.Track;
            string genreS = song.Genre;

            return (idPerson, idAlbum, pathS, titleS, yearS, trackS, genreS, song);
        }

        //change info from a song (changes database and metadata)
        public void EditSong(Songs song) //when song is selected by user, get object, get object info, rewrite SAME object
        {
            db.UpdateRolas(song);
            miner.RewriteDataS(song);
        }

        //Retreive album info 
        public void GetAlbumInfo(Albums album)
        {

        }

        //change album info(changes database and metadata)
        public void EditAlbum(Albums album)
        {
            db.UpdateAlbums(album);
            miner.RewriteDataA(album);
        }

        //also should be able to edit all the shit from the other objects
        //make group 
        //define as group or as person
        //consultas :=C

        


    }
}