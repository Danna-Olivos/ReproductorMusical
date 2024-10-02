namespace Database
{
    public class Songs
    {
        public int IdSong{get;set;} //IPK
        public int IdPerformer{get;set;}
        public int IdAlbum{get;set;}
        public string Path{get;set;}
        public string Title{get;set;}
        public int Track{get;set;}
        public int Year{get;set;}
        public string Genre{get;set;}

        Songs(int id_performer, int id_album, string path, string title, int track, int year, string genre)
        {
            IdPerformer = id_performer;
            IdAlbum = id_album;
            Path = path;
            Title = title;
            Track = track;
            Year = year;
            Genre = genre; 
        }
    }
}