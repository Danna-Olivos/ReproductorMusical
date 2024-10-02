namespace Database
{
    public class Albums
    {
        public int IdAlbum{get;set;} //IPK
        public string Path{get;set;}
        public string Name{get;set;}
        public int Year{get;set;}

        Albums(string path, string name, int year)
        {
            Path = path;
            Name = name;
            Year = year;
        }
    }
}