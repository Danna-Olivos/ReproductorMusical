using System;
using Gtk;
using System.IO;

namespace Database{
    class Program //aca el usuario va a definir el directorio en donde minar
    {
        public static void Main(string[] args)
        {
            string path = "/home/dannaabigailolivosnoriega/Documentos/Bruno Mars";
            Minero miner = new Minero(); 
            miner.Mine(path);
        }
    }
}

