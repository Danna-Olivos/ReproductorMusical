﻿using System;
using Gtk;
using System.IO;

namespace MusicApp 
{
    public class Program
    {
        private static Controller methods = new Controller();
        private static string? pathFromSelectedS; 
        public static void Main(string[] args)
        {
  
            Application.Init();
 
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
            Gdk.Pixbuf pixbuf = new Gdk.Pixbuf("MusicApp/AlbumCovers/default-cover.jpg");
            Gdk.Pixbuf scaledPixbuf = pixbuf.ScaleSimple(250, 250, Gdk.InterpType.Bilinear);
            songImage.Pixbuf = scaledPixbuf;
            topSection.PackStart(songImage, false, false, 0);

            // Search Section (Search Entry and Button)
            Entry searchEntry = new Entry { PlaceholderText = "Search" };

            Box searchBox = new Box(Orientation.Horizontal, 5);
            searchBox.PackStart(searchEntry, true, true, 0);

            mainContainer.PackStart(searchBox, false, false, 0);

            // Search Results section
            Label resultsLabel = new Label("Resultados de busqueda");
            mainContainer.PackStart(resultsLabel, false, false, 0);


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

            //SongList             
            ScrolledWindow scrolledWindow = new ScrolledWindow();
            TreeView treeView = new TreeView(); 
            ListStore songList = new ListStore(typeof(string), typeof(string), typeof(string), typeof(string));
            
            var songData = methods.ShowSongList();
            PopulateSongList(songData,treeView,songList);

            AddTreeViewColumns(treeView);
            scrolledWindow.Add(treeView);
            mainContainer.PackStart(scrolledWindow, true, true, 5);

            searchEntry.Activated += (sender,e) =>
            {
                string query = searchEntry.Text;
                bool isValid = methods.IsValid(query);

                if (!isValid)
                {
                    MessageDialog error = new MessageDialog(

                        window,
                        DialogFlags.Modal,
                        MessageType.Error,
                        ButtonsType.Ok,
                        "Invalid search"
                    );
                    error.Run();
                    error.Hide();
                }
                else
                {
                    var results = methods.ShowFoundSongList(methods.FindSongs(query));
                    PopulateSongList(results, treeView, songList);
                }
                
            };

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
                    string songPath = (string)model.GetValue(iter,3);

                    //get each song(object) when selected
                    var(idS, idP,idA,pathS,nameS, yearS, trackS, genreS) = methods.GetSongInfo(songPath);

                    Gdk.Pixbuf albumCoverPixbuf = methods.GetAlbumCover(songPath);
                    Gdk.Pixbuf scaledAlbumCover = albumCoverPixbuf.ScaleSimple(250, 250, Gdk.InterpType.Bilinear);
                    songImage.Pixbuf = scaledAlbumCover;

                    titleLabel.Text = $"Title: {title}";
                    artistLabel.Text =  $"Performer: {performer}";
                    albumLabel.Text =  $" Album: {album}";
                    yearLabel.Text =  $" Year: {yearS} ";
                    genreLabel.Text =  $" Genre: {genreS}";

                    pathFromSelectedS = pathS;
                }
            };

            // Slider (Progress)
            Scale progressSlider = new Scale(Orientation.Horizontal, 0, 100, 1);
            progressSlider.Value = 100;
            mainContainer.PackStart(progressSlider, false, false, 0);

            Box buttonBox = new Box(Orientation.Vertical, 5);
            topSection.PackEnd(buttonBox, false, false, 0); 

            // Edit Button
            Button editButton = new Button("\u270E");
            buttonBox.PackStart(editButton, true, true, 0);
            editButton.Clicked += (sender, e) =>
            {
                if (pathFromSelectedS == null)
                {
                    // Handle the case when no song is selected
                    MessageDialog dialog1 = new MessageDialog(
                        window,
                        DialogFlags.Modal,
                        MessageType.Warning,
                        ButtonsType.Ok,
                        "No song selected for editing."
                    );
                    dialog1.Run();
                    
                    dialog1.Hide();
                    return;
                }

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

                Button makePerButton = new Button("Edit Performer");
                editOptionsBox.PackStart(makePerButton, false, false, 5);             
        
                dialog.ContentArea.PackStart(editOptionsBox,true,true,5);
                dialog.ShowAll();

                editSongButton.Clicked += (s, ev) =>
                {
                    ShowEditSongForm(pathFromSelectedS);
                };

                editAlbumButton.Clicked += (s, ev) =>
                {
                    ShowEditAlbumForm(pathFromSelectedS);
                };

                makePerButton.Clicked += (s, ev) =>
                {
                    ShowEditPerformer(pathFromSelectedS);
                };

            };

            //Button for changing paths
            Button pathButton = new Button("Cambiar directorio");
            buttonBox.PackStart(pathButton, true, true, 0);
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
                        okay.Hide();
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
                        error.Hide();
                    }
                }
                dialog.Hide();
            };

            //Button for mining
            Button mineButton = new Button("Minar");
            buttonBox.PackStart(mineButton,true,true,0);
            mineButton.Clicked += (sender, e) =>
            {
                methods.StartMining(); 

                var updatedSongData = methods.ShowSongList();
                PopulateSongList(updatedSongData, treeView, songList);  

            };

            Button refreshButton = new Button("Refresh table");
            buttonBox.PackStart(refreshButton,true,true,0);
            refreshButton.Clicked += (sender, e) =>
            {
                var updatedSongData = methods.ShowSongList();
                PopulateSongList(updatedSongData, treeView, songList);  
            };

            Button makePerformerButton = new Button("Make performer");
            buttonBox.PackStart(makePerformerButton,true,true,0);
            makePerformerButton.Clicked += (sender, e) =>
            {
                Dialog dialog = new Dialog(
                    "Opciones de creacion",
                    window,
                    DialogFlags.Modal,
                    ButtonsType.None
                );
                dialog.SetDefaultSize(200, 80);
                Box editOptionsBox = new Box(Orientation.Horizontal, 5);
    
                Button MakeAButton = new Button("Define un artista");
                editOptionsBox.PackStart(MakeAButton, false, false, 5);

                Button MakeGButton = new Button("Define un grupo");
                editOptionsBox.PackStart(MakeGButton, false, false, 5);            
        
                dialog.ContentArea.PackStart(editOptionsBox,true,true,5);
                dialog.ShowAll();

                MakeAButton.Clicked += (s, ev) =>
                {
                    ShowMakeA();
                };

                MakeGButton.Clicked += (s, ev) =>
                {
                    ShowMakeG();
                };
            };

            Button makeInGroupButton = new Button("Add performer to group");
            buttonBox.PackStart( makeInGroupButton,true,true,0);
            makeInGroupButton.Clicked += (sender, e) =>
            {
                ShowAddToGroup();
            };
            

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

        private static void PerformSearching(string query)
        {
            
        }

        private static void ShowEditPerformer(string path)
        {
            var(idS, idP,idA,pathS,nameS, yearS, trackS, genreS) = methods.GetSongInfo(path);

            var (nameP, typeP)= methods.GetPerformerInfo(idP);
            Dialog editPDialog = new Dialog("Edit this Performer", null, DialogFlags.Modal);
    
            Entry titleEntry = new Entry { Text = nameP, PlaceholderText = "New Name" };
            editPDialog.ContentArea.PackStart(titleEntry, true, true, 10);

            MenuBar menuBar = new MenuBar();
            editPDialog.ContentArea.PackStart(menuBar, false, false, 0);

            MenuItem performerTypeMenu = new MenuItem("Select Performer Type");
            menuBar.Append(performerTypeMenu);

            Menu performerTypeOptions = new Menu();
            performerTypeMenu.Submenu = performerTypeOptions;

            RadioMenuItem option0 = new RadioMenuItem("Solo Artist");
            RadioMenuItem option1 = new RadioMenuItem(option0.Group,"Group");
            RadioMenuItem option2 = new RadioMenuItem(option0.Group,"Unknown");

            performerTypeOptions.Append(option0);
            performerTypeOptions.Append(option1);
            performerTypeOptions.Append(option2);

            if (typeP == 0)
            {
                option0.Active = true;
            }
            else if (typeP == 1)
            {
                option1.Active = true;  
            }
            else if(typeP == 2)
            {
                option2.Active = true; 
            }

            int selectedType = typeP; 

            option1.Activated += (sender, e) =>
            {
                selectedType = 0;
            };
            option1.Activated += (sender, e) =>
            {
                selectedType = 1;
            };
            option2.Activated += (sender, e) =>
            {
                selectedType = 2;
            };

            editPDialog.AddButton("OK", ResponseType.Ok); //should also actualize list
            editPDialog.AddButton("Cancel", ResponseType.Cancel);

            editPDialog.ShowAll();

            if (editPDialog.Run() == (int)ResponseType.Ok)
            {

                string newTitle = titleEntry.Text; 

                methods.EditPerformer(idP, newTitle,selectedType );//metodo de edicion

            }

            editPDialog.Hide();
        }

        private static void ShowEditAlbumForm(string path)
        {
            var(idS, idP,idA,pathS,nameS, yearS, trackS, genreS) = methods.GetSongInfo(path);
            var (pathA, nameA, yearA)= methods.GetAlbumInfo(idA);
            Dialog editAlbumDialog = new Dialog("Edit this Album", null, DialogFlags.Modal);
    
            Entry titleEntry = new Entry { Text = nameA, PlaceholderText = "New Album Name" };
            editAlbumDialog.ContentArea.PackStart(titleEntry, true, true, 10);

            Entry yearEntry = new Entry { Text = yearA.ToString(),PlaceholderText = "New Album Year" };
            editAlbumDialog.ContentArea.PackStart(yearEntry, true, true, 10);

            editAlbumDialog.AddButton("OK", ResponseType.Ok); //should also actualize list
            editAlbumDialog.AddButton("Cancel", ResponseType.Cancel);

            editAlbumDialog.ShowAll();

            if (editAlbumDialog.Run() == (int)ResponseType.Ok)
            {

                string newTitle = titleEntry.Text; 
                string newYear = yearEntry.Text;
               

                methods.EditAlbum(idA, newTitle, newYear);//metodo de edicion

            }

            editAlbumDialog.Hide();
        }

        private static void ShowEditSongForm(string path)
        {
            var(idS, idP,idA,pathS,nameS, yearS, trackS, genreS) = methods.GetSongInfo(path);

            Dialog editSongDialog = new Dialog("Edit this Song", null, DialogFlags.Modal);
    
            Entry titleEntry = new Entry { Text = nameS, PlaceholderText = "New Song Title" };
            editSongDialog.ContentArea.PackStart(titleEntry, true, true, 10);

            Entry performerEntry = new Entry { Text = methods.GetSongPerformer(idP),PlaceholderText = "New Performer" }; // si no existe el nombre hay que crear y asignar 
            editSongDialog.ContentArea.PackStart(performerEntry, true, true, 10);
            
            Entry albumEntry = new Entry { Text = methods.GetSongAlbum(idA),PlaceholderText = "New Album" }; // si no existe crear y asignar 
            editSongDialog.ContentArea.PackStart(albumEntry, true, true, 10);

            Entry yearEntry = new Entry { Text = yearS.ToString(),PlaceholderText = "New Song Year" };
            editSongDialog.ContentArea.PackStart(yearEntry, true, true, 10);

            Entry genreEntry = new Entry {Text = genreS, PlaceholderText = "New Genre" };
            editSongDialog.ContentArea.PackStart(genreEntry, true, true, 10);

            Entry trackEntry = new Entry { Text = trackS.ToString(),PlaceholderText = "New track" };
            editSongDialog.ContentArea.PackStart(trackEntry, true, true, 10);

            editSongDialog.AddButton("OK", ResponseType.Ok); //should also actualize list
            editSongDialog.AddButton("Cancel", ResponseType.Cancel);

            editSongDialog.ShowAll();

            if (editSongDialog.Run() == (int)ResponseType.Ok)
            {

                string newTitle = titleEntry.Text; //hay que encontrar el id
                string newGenre = genreEntry.Text;
                string newTrack = trackEntry.Text;
                string newPerformer = performerEntry.Text; //hay que encontrar el id
                string newYear = yearEntry.Text;
                string newAlbum = albumEntry.Text;
               

                methods.EditSong(idS,newTitle,newGenre,newTrack,newPerformer,newYear,newAlbum);//metodo de edicion

            }

            editSongDialog.Hide();
        }

        private static void ShowAddToGroup()
        {
            throw new NotImplementedException();
        }

        private static void ShowMakeG()
        {
            Dialog editGDialog = new Dialog("Make a Group", null, DialogFlags.Modal);
    
            Entry titleEntry = new Entry { PlaceholderText = "Group Name" };
            editGDialog.ContentArea.PackStart(titleEntry, true, true, 10);

            Entry yearSEntry = new Entry { PlaceholderText = "Fecha de creacion" };
            editGDialog.ContentArea.PackStart(yearSEntry, true, true, 10);

            Entry yearEndEntry = new Entry { PlaceholderText = "Fecha de separacion" };
            editGDialog.ContentArea.PackStart(yearEndEntry, true, true, 10);

            editGDialog.AddButton("OK", ResponseType.Ok); //should also actualize list
            editGDialog.AddButton("Cancel", ResponseType.Cancel);

            editGDialog.ShowAll();

            if (editGDialog.Run() == (int)ResponseType.Ok)
            {

                string name = titleEntry.Text; //hay que encontrar el id
                string startDate = yearSEntry.Text;
                string endDate = yearEndEntry.Text;
               
               methods.MakeGroup(name, startDate, endDate);
            }

            editGDialog.Hide();
        }

        private static void ShowMakeA()
        {
            Dialog editADialog = new Dialog("Make an Artist", null, DialogFlags.Modal);
    
            Entry nameEntry = new Entry { PlaceholderText = "Artist Name" };
            editADialog.ContentArea.PackStart(nameEntry, true, true, 10);

            Entry realNameEntry = new Entry { PlaceholderText = "Real Name" };
            editADialog.ContentArea.PackStart(realNameEntry, true, true, 10);

            Entry birthEntry = new Entry { PlaceholderText = "Fecha de nacimiento" };
            editADialog.ContentArea.PackStart(birthEntry, true, true, 10);

            Entry deathEntry = new Entry { PlaceholderText = "Fecha de fallecimiento" };
            editADialog.ContentArea.PackStart(deathEntry, true, true, 10);

            editADialog.AddButton("OK", ResponseType.Ok); //should also actualize list
            editADialog.AddButton("Cancel", ResponseType.Cancel);

            editADialog.ShowAll();

            if (editADialog.Run() == (int)ResponseType.Ok)
            {

                string name = nameEntry.Text; //hay que encontrar el id
                string realName = realNameEntry.Text;
                string birthDate = birthEntry.Text;
                string deathDate = deathEntry.Text;
               
               methods.MakeArtist(name, realName,birthDate, deathDate);
            }

            editADialog.Hide();

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

        private static void PopulateSongList(List<(string Title, string Performer, string Album,string Path)> songs, TreeView treeView, ListStore songList)
        {
            songList.Clear();
            foreach (var song in songs)
            {
                songList.AppendValues(song.Title, song.Performer, song.Album, song.Path);
            }
                
            treeView.Model = songList; 
            treeView.ShowAll();
        }

    } 
}

