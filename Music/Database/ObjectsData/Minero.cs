using System;
using System.Data.SQLite;
using System.IO;
using TagLib;

//this class retrieves information 

namespace Database
{
    public class Minero 
    {
        public string path;
        public DataBase data;
        public List<Songs> songs = new List<Songs>();

    }
}