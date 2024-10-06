using System;
using Gtk;
using System.IO;

namespace MusicApp 
{
    class Program
    {
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

            // Scale the image to a fixed size
            Gdk.Pixbuf scaledPixbuf = pixbuf.ScaleSimple(250, 250, Gdk.InterpType.Bilinear);

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

            //songList
            var scrolledWindow = new ScrolledWindow();
            var treeView = new TreeView();  // Table
            var songList = new ListStore(typeof(string),typeof(string));
            songList.AppendValues("Song 1", "Artist 1");
            songList.AppendValues("Song 2", "Artist 2");
            songList.AppendValues("Song 3", "Artist 3");

            treeView.Model = songList;

            AddTreeViewColumns(treeView);
            scrolledWindow.Add(treeView);
            mainContainer.PackStart(scrolledWindow, true, true, 5);

            // Slider (Volume or Progress)
            Scale progressSlider = new Scale(Orientation.Horizontal, 0, 100, 1);
            progressSlider.Value = 50;
            mainContainer.PackStart(progressSlider, false, false, 0);

            // Playback Control Buttons (Horizontal Box)
            Box controlBox = new Box(Orientation.Horizontal, 5);
            string[] buttonLabels = {"\u25B6"}; //pausa \u23F8 "\u2190", "\u2192"
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

        private static void AddTreeViewColumns(TreeView treeView)
        {
            // Column 1: Song Title
            TreeViewColumn titleColumn = new TreeViewColumn { Title = "Title" };
            CellRendererText titleCell = new CellRendererText();
            titleColumn.PackStart(titleCell, true);
            titleColumn.AddAttribute(titleCell, "text", 0); // The first column in the ListStore is at index 0
            treeView.AppendColumn(titleColumn);

            // Column 2: Artist
            TreeViewColumn artistColumn = new TreeViewColumn { Title = "Artist" };
            CellRendererText artistCell = new CellRendererText();
            artistColumn.PackStart(artistCell, true);
            artistColumn.AddAttribute(artistCell, "text", 1); // The second column in the ListStore is at index 1
            treeView.AppendColumn(artistColumn);
        }
    } 
}

