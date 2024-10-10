using System;
using Gtk;
using System.IO;

namespace MusicApp 
{
    public class Program
    {
        public static Controller methods = new Controller();
        public static void Main(string[] args)
        {
  
            Application.Init();

            // var cssProvider = new CssProvider();
            // cssProvider.LoadFromPath("/home/dannaabigailolivosnoriega/ReproductorMusical/Music/MusicApp/style.css");
            // StyleContext.AddProviderForScreen(Gdk.Screen.Default, cssProvider, StyleProviderPriority.User);
            
            // Main window
            Window window = new Window("CapyMusica");
            window.SetDefaultSize(1000, 800);
            window.SetPosition(WindowPosition.Center);
            window.Destroyed += (sender, e) => Application.Quit();

            // Main container 
            Box mainContainer = new Box(Orientation.Vertical, 5);
            window.Add(mainContainer);

            // Top section 
            Box topSection = new Box(Orientation.Horizontal, 5);
            mainContainer.PackStart(topSection, false, false, 0);

            // Create the image widget
            Image songImage = new Image();
            Gdk.Pixbuf pixbuf = new Gdk.Pixbuf("/home/dannaabigailolivosnoriega/ReproductorMusical/Music/MusicApp/AlbumCovers/perritos.jpg");
            Gdk.Pixbuf scaledPixbuf = pixbuf.ScaleSimple(250, 250, Gdk.InterpType.Bilinear);
            songImage.Pixbuf = scaledPixbuf;
            topSection.PackStart(songImage, false, false, 0);


            // Song Info section (Vertical Box for song title and details)
            Box songInfoBox = new Box(Orientation.Vertical, 5);
            topSection.PackStart(songInfoBox, false, false, 0);

            Label titleLabel = new Label("Titulo:");
            songInfoBox.PackStart(titleLabel, false, false, 0);
            Label artistLabel = new Label("Album:");
            songInfoBox.PackStart(artistLabel, false, false, 0);
            Label albumLabel = new Label("Artista:");
            songInfoBox.PackStart(albumLabel, false, false, 0);
            Label yearLabel = new Label("Año: ");
            songInfoBox.PackStart(yearLabel, false, false, 0);
            Label genreLabel = new Label("Género: ");
            songInfoBox.PackStart(genreLabel, false, false, 0);

            // Edit Button
            Button editButton = new Button("\u270E");
            songInfoBox.PackStart(editButton, false, false, 0);
            editButton.Activated += (sender, e) =>
            {

            };

            //Button for changing paths
            Button pathButton = new Button("Cambiar directorio");
            songInfoBox.PackStart(pathButton, false, false, 0);
            pathButton.Clicked += (sender, e) =>
            {
                Dialog dialog = new Dialog(
                    "Directorio Musica",
                    window,
                    DialogFlags.Modal,
                    ButtonsType.None
                );
                dialog.SetDefaultSize(1000, 10);

                Entry entry = new Entry();

                Box hbox = new Box(Orientation.Horizontal,10);
                hbox.PackStart(entry,true,true,10);

                Button sendButton = new Button ("\u27A4");
                hbox.PackStart(sendButton,false,false,10);
                dialog.ContentArea.PackStart(hbox,true,true,10);

                dialog.ShowAll();
                sendButton.Clicked +=(sender,e)=>
                {
                    string userInput = entry.Text;
                    dialog.Respond(ResponseType.Ok);
                };
                if (dialog.Run() == (int)ResponseType.Ok)
                {
                    string userInput = entry.Text;
                    if(methods.ChangePath(userInput) == true)
                    {
                        MessageDialog okay = new MessageDialog(
                            window,
                            DialogFlags.Modal,
                            MessageType.Info,
                            ButtonsType.Ok,
                            "Haz cambiado de directorio"
                        );
                        okay.Run();
                        okay.Destroy();
                    }
                    else
                    {
                        MessageDialog error = new MessageDialog(
                            window,
                            DialogFlags.Modal,
                            MessageType.Error,
                            ButtonsType.Ok,
                            "El directorio que ingresaste no existe"
                        );
                        error.Run();
                        error.Destroy();
                    }
                }
                dialog.Destroy();
            };

            //Button for mining
            Button mineButton = new Button("Minar");
            songInfoBox.PackStart(mineButton,false,false,0);
            mineButton.Clicked += (sender, e) =>
            {
                methods.StartMining();   
            };

            // Search Section (Search Entry and Button)
            Entry searchEntry = new Entry { PlaceholderText = "Search" };
            Button searchButton = new Button("\u26B2");
            searchEntry.Activated += (sender,e) =>
            {

            };

            Box searchBox = new Box(Orientation.Horizontal, 5);
            searchBox.PackStart(searchEntry, true, true, 0);
            searchBox.PackStart(searchButton, false, false, 0);

            mainContainer.PackStart(searchBox, false, false, 0);

            // Search Results section
            Label resultsLabel = new Label("Resultados de busqueda");
            mainContainer.PackStart(resultsLabel, false, false, 0);

            //SongList             
            ScrolledWindow scrolledWindow = new ScrolledWindow();
            TreeView treeView = new TreeView(); 
            ListStore songList = new ListStore(typeof(string), typeof(string), typeof(string));
            
            var songData = methods.ShowSongList();
            PopulateSongList(songData,treeView,songList);

            AddTreeViewColumns(treeView);
            scrolledWindow.Add(treeView);
            mainContainer.PackStart(scrolledWindow, true, true, 5);

            //select row action
            treeView.Selection.Changed += (sender, e) =>
            {
                TreeIter iter;
                ITreeModel model; 
                if (treeView.Selection.GetSelected(out model, out iter))
                {
                    // extract data from selected row
                    string title = (string)model.GetValue(iter, 0);
                    string performer = (string)model.GetValue(iter, 1);
                    string album = (string)model.GetValue(iter, 2);

                    //get each song(object) when selected
                    var(idP,idA,pathS,nameS, yearS, trackS, genreS) = methods.GetSongInfo(title);
                    titleLabel.Text = $"Title: {title}";
                    artistLabel.Text =  $"Performer: {performer}";
                    albumLabel.Text =  $" Album: {album}";
                    yearLabel.Text =  $" Year: {yearS} ";
                    genreLabel.Text =  $" Genre: {genreS}";
                }
            };
            

            // Slider (Volume or Progress)
            Scale progressSlider = new Scale(Orientation.Horizontal, 0, 100, 1);
            progressSlider.Value = 100;
            mainContainer.PackStart(progressSlider, false, false, 0);

            // Playback Control Buttons (Horizontal Box)
            Box controlBox = new Box(Orientation.Horizontal, 5);
            mainContainer.PackStart(controlBox, false, false, 0); 
            Button buttonplay = new Button("\u25B6"); //pausa \u23F8 "\u2190", "\u2192"
            controlBox.PackStart(buttonplay,true,true,0); 
            controlBox.Halign = Align.Center;
            // buttonplay.Clicked += (sender, e) =>
            // {
            //     //para que se reproduzca la cancion 
            // };          

            // Show the entire window
            window.ShowAll();

            Application.Run();

        }

        private static void AddTreeViewColumns(TreeView treeView)
        {
            // Column 1: title
            TreeViewColumn titleColumn = new TreeViewColumn { Title = "Title" };
            CellRendererText titleCell = new CellRendererText();
            titleColumn.PackStart(titleCell, true);
            titleColumn.AddAttribute(titleCell, "text", 0); 
            treeView.AppendColumn(titleColumn);

            // Column 2: artist
            TreeViewColumn artistColumn = new TreeViewColumn { Title = "Performer" };
            CellRendererText artistCell = new CellRendererText();
            artistColumn.PackStart(artistCell, true);
            artistColumn.AddAttribute(artistCell, "text", 1); 
            treeView.AppendColumn(artistColumn);

            // Column 3: album
            TreeViewColumn albumColumn = new TreeViewColumn { Title = "Album" };
            CellRendererText albumCell = new CellRendererText();
            albumColumn.PackStart(albumCell, true);
            albumColumn.AddAttribute(albumCell, "text", 2); 
            treeView.AppendColumn(albumColumn);

        }

        private static void PopulateSongList(List<(string Title, string Performer, string Album)> songs, TreeView treeView, ListStore songList)
        {
            foreach (var song in songs)
            {
                songList.AppendValues(song.Title, song.Performer, song.Album);
            }
                
            treeView.Model = songList; 
        }

        //para regresar las listas de canciones resultados de las busquedas 
        private static ListStore HandleSearch(object sender, EventArgs e)
        {
            ListStore list = new ListStore(typeof(string), typeof(string), typeof(string));
            return list;
        }

    } 
}

