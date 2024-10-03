using System;
using System.IO;
using TagLib;

//this class retrieves information 

namespace Database
{
    public class Minero 
    {
        public required string  Path{get;set;}
        public List<Songs> songs = new();
        DataBase db = DataBase.Instance;

        public void Mine(string path)
        {
            var musicFiles = Directory.GetFiles(path, "*.mp3", SearchOption.AllDirectories);
            foreach (string filePath in musicFiles)
            {
                try
                {
                    
                    GetData(filePath);
                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing file {filePath}: {ex.Message}");
                }
            }
        }

        private (string p, string ti, string a, uint y, string g, uint tr) GetData(string filePath)
        {
            var file = TagLib.File.Create(filePath);//path from an especific song

            string performers = file.Tag.FirstPerformer ?? "Unknown"; // TPE1
            string title = file.Tag.Title ?? "Unknown";                 // TIT2
            string album = file.Tag.Album ?? "Unknown";                // TALB
            uint year = file.Tag.Year  != 0 ? file.Tag.Year : (uint)DateTime.Now.Year;   //TDRC                                // TDRC
            string genre = file.Tag.FirstGenre ?? "Unknown";           // TCON
            uint track = file.Tag.Track != 0 ? file.Tag.Track : 1;  //TRCK

            return(performers,title,album,year,genre,track);
        }

    }
}