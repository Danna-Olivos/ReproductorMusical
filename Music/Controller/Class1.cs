using System;
using Database;
using MusicApp;

namespace Controller;

public class Controller
{
    private string configPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".config", "CapyMusic", "config.txt");


    //en el constructor hay que asignar el currentPath = 

    //Metodo 
    //obtener el directorio del .config
    //verifica si existe el archivo
    //verifica si esta la carpeta de la app, sino existe lo crea
    //Directory.CreateDirectory = Path.GetDirectory(configPath)
    //verificar si el txt existe
    //File.ReadAllText(configPath).Trim()
    //return path
    //si no existe el path se obtiene el default i.e el music
    //si esta vacia la de music -> crear directorio 
    //File.WriteAllText(configPath, defaultPath)
    //devolver el default = music

}
