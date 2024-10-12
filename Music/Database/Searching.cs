#nullable disable
using System;
using System.Text.RegularExpressions;

namespace Database
{
    public class Searching()
    {
        DataBase db = DataBase.Instance;
        public List<Songs> HandleSearch(string query)
        {
            // Initialize an empty list to hold found songs
            List<Songs> foundSongs = new List<Songs>();

            if (string.IsNullOrWhiteSpace(query))
            {
                Console.WriteLine("Empty search query, please provide valid input.");
                return foundSongs;  // Return an empty list
            }

            // Define the regex patterns
            string songPattern = @"^[A-Za-z0-9'!?,\s]+$";
            string performerPattern = @"^[A-Za-z\s]+$";
            string albumPattern = @"^[A-Za-z0-9\s()]+$";

            // Check if the query matches the song pattern
            if (Regex.IsMatch(query, songPattern, RegexOptions.IgnoreCase))
            {
                Console.WriteLine("Search for a song");
                foundSongs = SearchSong(query);  // Get songs by title
            }
            // Check if the query matches the performer pattern
            else if (Regex.IsMatch(query, performerPattern, RegexOptions.IgnoreCase))
            {
                Console.WriteLine("Search for a performer");
                foundSongs = SearchSongPerformer(query);  // Get songs by performer
            }
            // Check if the query matches the album pattern
            else if (Regex.IsMatch(query, albumPattern, RegexOptions.IgnoreCase))
            {
                Console.WriteLine("Search for an album");
                foundSongs = SearchSongAlbum(query);  // Get songs by album
            }
            else
            {
                Console.WriteLine("Invalid search query, please try again.");
            }

            // Return the list of found songs
            return foundSongs;
        }


        private List<Songs> SearchSong(string songTitle)
        {
            List<Songs> foundSongs = db.GetSongsByTitle(songTitle);
            Console.WriteLine($"Searching for song: {songTitle}");
            return foundSongs;
        }

        private List<Songs> SearchSongPerformer(string performerName)
        {
            int IDp = db.GetPerformerId(performerName);
            List<Songs> foundSongs = db.GetSongsByPerformer(IDp);
            Console.WriteLine($"Searching for performer: {performerName}");
            return foundSongs;
        }

       private List<Songs> SearchSongAlbum(string albumName)
        {
            List<int> IDa = db.GetAlbumsId(albumName);
            List<Songs> foundSongs = new List<Songs>();

            foreach (int id in IDa)
            {
                List<Songs> songsFromAlbums = db.GetSongsByAlbum(id);
                foundSongs.AddRange(songsFromAlbums);
            }
            
            Console.WriteLine($"Searching for album: {albumName}. Found {foundSongs.Count} songs.");
            return foundSongs;
        }


        public bool IsQueryValid(string query)
        {
            string songPattern = @"^[A-Za-z0-9'!?,\s]+$";
            string performerPattern = @"^[A-Z][a-zA-Z\s]+$";
            string albumPattern = @"^[A-Za-z0-9\s()]+$";

            if (Regex.IsMatch(query, songPattern) || 
                Regex.IsMatch(query, performerPattern) || 
                Regex.IsMatch(query, albumPattern))
            {
                return true;
            }
            return false;

        }
    }
}