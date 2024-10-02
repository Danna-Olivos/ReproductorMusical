using System;
using System.IO;
using TagLib;

//this class retrieves information 

namespace Database
{
    public class Minero 
    {
        public required string  Path{get;set;}
        DataBase db = DataBase.Instance;

        public void Mine(string path)
        {
            var musicFiles = Directory.GetFiles(path, "*.mp3", SearchOption.AllDirectories);
            foreach (var filePath in musicFiles)
            {
                try
                {
                    
                    var file = TagLib.File.Create(filePath);//path from an especific song

                    string performers = file.Tag.FirstPerformer ?? "Unknown"; // TPE1
                    string title = file.Tag.Title ?? "Unknown";                 // TIT2
                    string album = file.Tag.Album ?? "Unknown";                // TALB
                    uint year = file.Tag.Year  != 0 ? file.Tag.Year : (uint)DateTime.Now.Year;   //TDRC                                // TDRC
                    string genre = file.Tag.FirstGenre ?? "Unknown";           // TCON
                    uint track = file.Tag.Track != 0 ? file.Tag.Track : 1;  //TRCK
                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing file {filePath}: {ex.Message}");
                }
            }
        }

    }
}