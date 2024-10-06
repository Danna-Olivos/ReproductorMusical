using System;
using Gtk;
using System.IO;

namespace MusicApp 
{
    class Program
    {

    // private static void ApplyCss(Widget widget)
    // {
    //     // Create a CssProvider and load CSS rules
    //     CssProvider cssProvider = new CssProvider();
    //     cssProvider.LoadFromData(
    //         @"
    //         button {
    //             background-color: #3498db;
    //             color: white;
    //             font-size: 16px;
    //             border-radius: 10px;
    //             padding: 10px;
    //         }
    //         button:hover {
    //             background-color: #2980b9;
    //         }
    //         "
    //     );

    //     //Get the widget's StyleContext and apply the CSSProvider
    //     StyleContext styleContext = widget.StyleContext;
    //     styleContext.AddProvider(cssProvider, Gtk.StyleProviderPriority.User);
    // }
        public static void Main(string[] args)
        {
  
            Application.Init();

            // Main window
            Window window = new Window("CapyMusica");
            window.SetDefaultSize(600, 400);
            window.SetPosition(WindowPosition.Center);
            window.Destroyed += (sender, e) => Application.Quit();

              var cssProvider = new Gtk.CssProvider();
            cssProvider.LoadFromPath("/home/dannaabigailolivosnoriega/ReproductorMusical/Music/MusicApp/style.css");
            Gtk.StyleContext.AddProviderForScreen(Gdk.Screen.Default, cssProvider, Gtk.StyleProviderPriority.User);

            //ApplyCss(window);
            // Main container (Vertical Box)
            Box mainContainer = new Box(Orientation.Vertical, 5);
            window.Add(mainContainer);

            // Top section (Horizontal Box)
            Box topSection = new Box(Orientation.Horizontal, 5);
            mainContainer.PackStart(topSection, false, false, 0);

            // Create the image widget
            var songImage = new Gtk.Image();

            // Load the image from file
            Gdk.Pixbuf pixbuf = new Gdk.Pixbuf("/home/dannaabigailolivosnoriega/ReproductorMusical/Music/MusicApp/AlbumCovers/perritos.jpg");

            // Scale the image to a fixed size (e.g., 150x150)
            Gdk.Pixbuf scaledPixbuf = pixbuf.ScaleSimple(180, 180, Gdk.InterpType.Bilinear);

            // Set the scaled image
            songImage.Pixbuf = scaledPixbuf;
            topSection.PackStart(songImage, false, false, 0);


            // Song Info section (Vertical Box for song title and details)
            Box songInfoBox = new Box(Orientation.Vertical, 5);
            topSection.PackStart(songInfoBox, false, false, 0);

            Label titleLabel = new Label("-Titulo ...");
            songInfoBox.PackStart(titleLabel, false, false, 0);
            Label artistLabel = new Label("-...");
            songInfoBox.PackStart(artistLabel, false, false, 0);
            Label albumLabel = new Label("-...");
            songInfoBox.PackStart(albumLabel, false, false, 0);

            // Edit Button
            Button editButton = new Button("Edit");
            songInfoBox.PackStart(editButton, false, false, 0);
            //ApplyCss(editButton);

            // Search Section (Search Entry and Button)
            Entry searchEntry = new Entry { PlaceholderText = "Search" };
            Button searchButton = new Button();
            searchButton.Add(new Image(Stock.Find, IconSize.Menu));  // Search icon

            Box searchBox = new Box(Orientation.Horizontal, 5);
            searchBox.PackStart(searchEntry, true, true, 0);
            searchBox.PackStart(searchButton, false, false, 0);

            mainContainer.PackStart(searchBox, false, false, 0);

            // Search Results section
            Label resultsLabel = new Label("Resultados de busqueda");
            mainContainer.PackStart(resultsLabel, false, false, 0);

            // Song List (ListBox or TreeView)
            ListBox songList = new ListBox();
            for (int i = 1; i <= 4; i++)
            {
                ListBoxRow row = new ListBoxRow();
                Label songLabel = new Label($"song {i}");
                row.Add(songLabel);
                songList.Add(row);
            }
            songList.SelectRow((ListBoxRow)songList.Children[2]); // Select the third song by default
            mainContainer.PackStart(songList, true, true, 0);

            // Slider (Volume or Progress)
            Scale progressSlider = new Scale(Orientation.Horizontal, 0, 100, 1);
            progressSlider.Value = 50;
            mainContainer.PackStart(progressSlider, false, false, 0);

            // Playback Control Buttons (Horizontal Box)
            Box controlBox = new Box(Orientation.Horizontal, 5);
            string[] buttonLabels = { "<<", "<", "||", ">", ">>" };
            foreach (var label in buttonLabels)
            {
                Button controlButton = new Button(label);
                controlBox.PackStart(controlButton, false, false, 0);
            }
            mainContainer.PackStart(controlBox, false, false, 0);

            // Show the entire window
            window.ShowAll();

            Application.Run();

        }

    } 
}

