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
            string newQ = SelectFromQuery(query);

            List<Songs> foundSongs = new List<Songs>();

            if (string.IsNullOrWhiteSpace(query))
            {
                Console.WriteLine("Empty search query, please provide valid input.");
                return foundSongs;
            }

            string songPattern = @"^t:\s*[A-Za-z0-9'!?,\s]+$";
            string performerPattern = @"^p:\s*[A-Za-z\s]+$";
            string albumPattern = @"^a:\s*[A-Za-z0-9\s()]+$";

            if (Regex.IsMatch(query, songPattern, RegexOptions.IgnoreCase))
            {
                Console.WriteLine("Search for a song");
                foundSongs = SearchSong(newQ);  
            }

            else if (Regex.IsMatch(query, performerPattern, RegexOptions.IgnoreCase))
            {
                Console.WriteLine("Search for a performer");
                foundSongs = SearchSongPerformer(newQ);  
            }
            else if (Regex.IsMatch(query, albumPattern, RegexOptions.IgnoreCase))
            {
                Console.WriteLine("Search for an album");
                foundSongs = SearchSongAlbum(newQ);
            }
            else
            {
                Console.WriteLine("Invalid search query, please try again.");
            }

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

        private string SelectFromQuery(string query)
        {
            int colonIndex = query.IndexOf(':');
            if (colonIndex >= 0 && colonIndex < query.Length - 1)
            {
                return query.Substring(colonIndex + 1).Trim();
            }
            
            return string.Empty;

        }


        public bool IsQueryValid(string query)
        {
            string songPattern = @"^t:\s*[A-Za-z0-9'!?,\s]+$";
            string performerPattern = @"^p:\s*[A-Za-z\s]+$";
            string albumPattern = @"^a:\s*[A-Za-z0-9\s()]+$";

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