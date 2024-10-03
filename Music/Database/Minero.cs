using System;
using System.IO;
using Pango;
using TagLib;
//this class retrieves information 

namespace Database
{
    public class Minero 
    {
        public required string  Path{get;set;}
        public List<Songs> songs = new(); // maybe delete(?)
        DataBase db = DataBase.Instance;

        public void Mine(string path)
        {
            var musicFiles = Directory.GetFiles(path, "*.mp3", SearchOption.AllDirectories);
            foreach (string filePath in musicFiles)
            {
                try
                {
                    
                    GetData(filePath); // getting data for each path in the directory
                    //making objects with extracted data for each path in the directory
                    //inserting object into database
                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing file {filePath}: {ex.Message}");
                }
            }
        }

        private (string p, string ti, string a, int y, string g, int tr) GetData(string filePath)
        {
            var file = TagLib.File.Create(filePath);//path from an especific song

            string performer = file.Tag.FirstPerformer ?? "Unknown"; // TPE1
            string title = file.Tag.Title ?? "Unknown";                 // TIT2
            string album = file.Tag.Album ?? "Unknown";                // TALB
            uint year = file.Tag.Year  != 0 ? file.Tag.Year : (uint)DateTime.Now.Year;   //TDRC                                
            string genre = file.Tag.FirstGenre ?? "Unknown";           // TCON
            uint track = file.Tag.Track != 0 ? file.Tag.Track : 1;  //TRCK

            return(performer,title,album,(int)year,genre,(int)track);
        }

        private Performer PopulatePerformer(string performer)
        {
            Performer performerObj = new Performer(performer, Type.ArtistType.Unknown);
            return performerObj;
        }

        private Albums PopulateAlbums(string filepath, string album, int year)
        {
            Albums albumsObj = new Albums(filepath, album, year); 
            return albumsObj;
        }

        private Songs PopulateSongs(int id_performer, int id_album, string path, string title, int track, int year, string genre)
        {
            Songs songsObj = new Songs(id_performer, id_album, path, title, track, year, genre);
            return songsObj;
        }

    }
}