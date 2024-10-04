using System;
using Gtk;
using System.IO;

namespace MusicApp 
{
   class Program
    {
        public static void Main(string[] args)
        {
            // // Initialize the GTK application
            // Application.Init();

            // // Create the main window
            // Window window = new Window("Simple GTK# App");
            // window.SetDefaultSize(400, 200); // Set the default size of the window
            // window.SetPosition(WindowPosition.Center); // Center the window on the screen

            // // Connect the destroy event to close the application when the window is closed
            // window.Destroyed += (sender, e) =>
            // {
            //     Application.Quit();
            // };

            // // Create a vertical box layout container
            // Box vbox = new Box(Orientation.Vertical, 5);

            // // Create a label and add it to the vbox
            // Label label = new Label("Welcome to the Simple GTK# App!");
            // vbox.PackStart(label, false, false, 0);

            // // Create a button with a label
            // Button button = new Button("Click Me");
            // // Connect the button's clicked event to an event handler
            // button.Clicked += (sender, e) =>
            // {
            //     // Display a message when the button is clicked
            //     MessageDialog dialog = new MessageDialog(
            //         window,
            //         DialogFlags.Modal,
            //         MessageType.Info,
            //         ButtonsType.Ok,
            //         "Hello from GTK#!"
            //     );
            //     dialog.Run();
            //     dialog.Destroy(); // Close the dialog after it runs
            // };

            // // Add the button to the vbox
            // vbox.PackStart(button, false, false, 0);

            // // Add the vbox to the window
            // window.Add(vbox);

            // // Show all widgets in the window
            // window.ShowAll();

            // // Start the GTK application event loop
            // Application.Run();
         Application.Init();

            // Main window
            Window window = new Window("CapyMusica");
            window.SetDefaultSize(600, 400);
            window.SetPosition(WindowPosition.Center);
            window.Destroyed += (sender, e) => Application.Quit();

            // Main container (Vertical Box)
            Box mainContainer = new Box(Orientation.Vertical, 5);
            window.Add(mainContainer);

            // Top section (Horizontal Box)
            Box topSection = new Box(Orientation.Horizontal, 5);
            mainContainer.PackStart(topSection, false, false, 0);

            // Song Image placeholder
            Image songImage = new Image("placeholder.png");  // Load placeholder image
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

