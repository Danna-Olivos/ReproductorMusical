﻿using System;
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
            songInfoBox.PackStart(editButton, true, true, 0);
            editButton.Clicked += (sender, e) =>
            {
                Dialog dialog = new Dialog(
                    "Opciones de edicion",
                    window,
                    DialogFlags.Modal,
                    ButtonsType.None
                );
                dialog.SetDefaultSize(200, 80);
                Box editOptionsBox = new Box(Orientation.Horizontal, 5);
    
                Button editSongButton = new Button("Edit Song");
                editOptionsBox.PackStart(editSongButton, false, false, 5);

                Button editAlbumButton = new Button("Edit Album");
                editOptionsBox.PackStart(editAlbumButton, false, false, 5); 

                Button makePerButton = new Button("Add new artist");
                editOptionsBox.PackStart(makePerButton, false, false, 5);  

                Button makeGroButton = new Button("Add new group");
                editOptionsBox.PackStart(makeGroButton, false, false, 5); 

                Button makeINGroButton = new Button("Add artist to group");
                editOptionsBox.PackStart(makeINGroButton, false, false, 5);            
        
                dialog.ContentArea.PackStart(editOptionsBox,true,true,5);
                dialog.ShowAll();

                editSongButton.Clicked += (s, ev) =>
                {
                    ShowEditSongForm();
                };

                editAlbumButton.Clicked += (s, ev) =>
                {
                    ShowEditAlbumForm();
                };

                makePerButton.Clicked += (s, ev) =>
                {
                    ShowMakeA();
                };

                makeGroButton.Clicked += (s, ev) =>
                {
                    ShowMakeG();
                };

                makeINGroButton.Clicked += (s, ev) =>
                {
                    ShowAddToGroup();
                };
            };

            //Button for changing paths
            Button pathButton = new Button("Cambiar directorio");
            songInfoBox.PackStart(pathButton, true, true, 0);
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
            songInfoBox.PackStart(mineButton,true,true,0);
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
                    var(idP,idA,pathS,nameS, yearS, trackS, genreS, songObj) = methods.GetSongInfo(title);
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

        
        //EVENT HANDLING
        private static void ShowEditAlbumForm()
        {
            throw new NotImplementedException();
        }

        private static void ShowEditSongForm()
        {
            Dialog editSongDialog = new Dialog("Edit this Song", null, DialogFlags.Modal);
    
            Entry titleEntry = new Entry { PlaceholderText = "New Song Title" };
            editSongDialog.ContentArea.PackStart(titleEntry, true, true, 10);

            Entry performerEntry = new Entry { PlaceholderText = "New Performer" }; // si no existe el nombre hay que crear y asignar 
            editSongDialog.ContentArea.PackStart(performerEntry, true, true, 10);
            
            Entry albumEntry = new Entry { PlaceholderText = "New Album" }; // si no existe crear
            editSongDialog.ContentArea.PackStart(albumEntry, true, true, 10);

            Entry genreEntry = new Entry { PlaceholderText = "New Genre" };
            editSongDialog.ContentArea.PackStart(genreEntry, true, true, 10);

            Entry yearEntry = new Entry { PlaceholderText = "New Song Year" };
            editSongDialog.ContentArea.PackStart(yearEntry, true, true, 10);

            editSongDialog.AddButton("OK", ResponseType.Ok);
            editSongDialog.AddButton("Cancel", ResponseType.Cancel);

            editSongDialog.ShowAll();

            if (editSongDialog.Run() == (int)ResponseType.Ok)
            {
                string newTitle = titleEntry.Text; //hay que encontrar el id
                string newPerformer = performerEntry.Text; //hay que encontrar el id
                string newAlbum = albumEntry.Text;
                string newGenre = genreEntry.Text;
                string newYear = yearEntry.Text;

            }

            editSongDialog.Destroy();
        }

        private static void ShowAddToGroup()
        {
            throw new NotImplementedException();
        }

        private static void ShowMakeG()
        {
            throw new NotImplementedException();
        }

        private static void ShowMakeA()
        {
            throw new NotImplementedException();
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

