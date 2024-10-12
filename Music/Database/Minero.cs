#nullable disable
using System;
using System.IO;
using TagLib;
//this class retrieves information 

namespace Database
{
    public class Minero 
    {
        DataBase db = DataBase.Instance;

        public void Mine(string path)
        {
            var musicFiles = Directory.GetFiles(path, "*.mp3", SearchOption.AllDirectories);
            foreach (string filePath in musicFiles)
            {
                try
                {
                    if (!HasReadingPermissions(filePath))
                    {
                        Console.WriteLine($"Skipping file {filePath}: No reading permissions.");
                        continue; 
                    }
                    var albumDirectory = Path.GetDirectoryName(filePath);
                    var (performer,title,album,year,genre,track) = GetData(filePath); // getting data for each path in the directory
                    //making objects with extracted data for each path in the directory
                    Performer p = PopulatePerformer(performer);
                    db.MakePerformer(p);//inserting object into database
                    
                    Albums a = PopulateAlbums(albumDirectory,album,year);
                    db.MakeAlbums(a);//inserting object into database

                    var(id_performer, id_album) = GetLatterIDs(album,performer,year,albumDirectory);
                    Songs s = PopulateSongs(id_performer, id_album, filePath,title, track, year, genre);
                    db.MakeRolas(s);//inserting object into database
                    
                }
                catch (UnauthorizedAccessException)
                {
                    Console.WriteLine($"Skipping file {filePath}: Unauthorized Access.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing file {filePath}: {ex.Message}");
                }
            }
        }

        private bool HasReadingPermissions(string filePath)
        {
            try
            {
                using (FileStream fs = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read))
                {
                    return true;
                }
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
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

        private (int idP, int idA) GetLatterIDs(string albumN, string performerN,int year, string albumPath)
        {

            int idPer = db.GetPerformerId(performerN);
            int idAl = db.GetAlbumId(albumN, year, albumPath);

            return (idPer, idAl);
        }

        public Gdk.Pixbuf GetAlbumCover(string songPath)
        {
            var file = TagLib.File.Create(songPath);
            if (file.Tag.Pictures.Length > 0)
            {
                var albumCover = file.Tag.Pictures[0];
                using (var stream = new MemoryStream(albumCover.Data.Data))
                    return new Gdk.Pixbuf(stream);
            }    
            return new Gdk.Pixbuf("MusicApp/AlbumCovers/default-cover.jpg");
        }

    }
}