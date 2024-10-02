using System;
using Gtk;
using System.IO;

namespace Database{
    class Program //aca el usuario va a definir el directorio en donde minar
    {
        public static void Main(string[] args)
        {
        
            Console.WriteLine("Creando la ventana...");
            Window window = new Window("Mi ventana GTK#");

            Console.WriteLine("Configurando la ventana...");
            window.SetDefaultSize(400, 300);
            window.SetPosition(WindowPosition.Center);

            Console.WriteLine("Mostrando la ventana...");
            window.ShowAll();

            Console.WriteLine("Iniciando el ciclo de eventos...");
            Application.Run();

        }
    }
}

