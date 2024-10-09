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
        public void startMining()
        {
            miner.Mine(path);
        }

        //Retreive info from each song to display when mined
        // public List<string> showSongList()
        // {
        //     List<string> info = new();
        //     List<Songs> aviableSongs = db.ListSongs();
        //     foreach(Songs song in aviableSongs)
        //     {
        //         string infoPerSong = $"{song.Title}";
        //     }
        // }

        //Retreive info from each song to display when selected
        public void getSongInfo(Songs song)
        {
            
        }

        //change info from a song (changes database and metadata)
        public void editSong(Songs song)
        {
            db.UpdateRolas(song);
            miner.RewriteDataS(song);
        }

        //Retreive album info 
        public void getAlbumInfo(Albums album)
        {

        }

        //change album info(changes database and metadata)
        public void editAlbum(Albums album)
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