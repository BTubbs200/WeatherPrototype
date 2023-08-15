using ImageMagick;
using Microsoft.VisualBasic;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Windows.Markup;

namespace WeatherPrototype
{
    public partial class Form1 : Form
    {
        public static class Globals
        {
            private static PointF usrGeoCoords = new PointF();
            private static string imgFilePath = string.Empty;
            private static ListBox debugOutputListBox;
            private static MagickImageCollection globalCollection = new MagickImageCollection();

            public static PointF GetGeoCoords
            {
                get { return usrGeoCoords; }
            }

            public static void SetGeoCoords(float lat, float lon)
            {
                usrGeoCoords.X = lat;
                usrGeoCoords.Y = lon;
            }

            public static string imgFilePathValue
            {
                get { return imgFilePath; }
                set { imgFilePath = value; }
            }

            public static void SetListBox(ListBox listBox)
            {
                debugOutputListBox = listBox;
            }

            public static void SetListBoxItems(string debugInfo)
            {
                debugOutputListBox.Items.Add(debugInfo);
            }

            public static void SetCollection(MagickImageCollection processedFrames)
            {
                globalCollection = processedFrames;
            }

            public static MagickImageCollection GetCollection
            {
                get { return globalCollection; }
            }
        }

        public Form1()
        {
            InitializeComponent();
            Globals.SetListBox(debugOutputListBox);
            StartupCoordInputBox();
        }

        static void StartupCoordInputBox()
        {
            Boolean coordCheck = false;
            string regularPattern = @"^-?\d{2}\.\d{1,15}, -?\d{2}\.\d{1,15}$"; // [-]XX.X(1-15 digits), [-]XX.X(1-15 digits)

            while (!coordCheck)
            {
                string usrCoords = Interaction.InputBox("Welcome! Please enter your WGS84 coordinates to begin." , "Enter Coordinates", "");

                // Ensures the program quits if the cancel button is pressed.
                if (usrCoords.Trim() == "") Environment.Exit(0);

                Regex rg = new Regex(regularPattern);

                if (!rg.IsMatch(usrCoords))
                {
                    MessageBox.Show("Invalid coordinates.\n\nPlease enter valid coordinates in\n\"[-]XX.X(1-15 digits), [-]XX.X(1-15 digits)\"\nformat and try again\n\nX  = number from 1-9\n[ ] = optional\n( ) = mandatory");
                }
                else
                {
                    string[] usrCoordSplit = usrCoords.Split(',');
                    float lat = float.Parse(usrCoordSplit[0].Trim());
                    float lon = float.Parse(usrCoordSplit[1].Trim());

                    Globals.SetGeoCoords(lat, lon);
                    Globals.SetListBoxItems($"Successfully received WGS84 coordinates as {usrCoords}.");
                    coordCheck = true;
                }
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "C:\\";
                openFileDialog.Filter = "GIF Files (*.gif)|*.gif";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Globals.imgFilePathValue = openFileDialog.FileName;
                    displayPicturebox.Image = Image.FromFile(openFileDialog.FileName);
                    demoButton.Enabled = true;
                    Globals.SetListBoxItems($"Loaded {openFileDialog.FileName}.");
                    statusLabel.Text = "File loaded successfully!";
                    demoButton.Text = "Process Loop";
                }
                else
                {
                    Globals.SetListBoxItems($"Ran into problem loading {openFileDialog.FileName}.");
                    statusLabel.Text = "Failed to load file. Check if file is valid!";
                }
            }
        }

        private void demoButton_Click(object sender, EventArgs e)
        {
            GifProcessing gp = new GifProcessing();

            Globals.SetCollection(gp.Execute(Globals.imgFilePathValue, null));

            var localCollection = Globals.GetCollection;
            var memStream = new MemoryStream();

            localCollection.Write(memStream, MagickFormat.Gif);

            memStream.Position = 0;

            var bitmap = Image.FromStream(memStream);

            displayPicturebox.Image = bitmap;

            // Since MagickImageCollection is a reference, anytime .Dispose() is called,
            // globalCollection and any variables referencing it get disposed as well.
            // see Issue #1 on GitHub.
            localCollection.Dispose();

            /// Does bitmap get disposed properly without explicitly disposing?
            //bitmap.Dispose();

            demoButton.Enabled = false;
            saveOutputtedLoopToolStripMenuItem.Enabled = true;
            demoButton.Text = "Success!";
            statusLabel.Text = $"Success!";
        }

        private void showOutputToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!debugOutputListBox.Visible)
            {
                showOutputToolStripMenuItem.Text = "Hide Program Log";
                debugOutputListBox.Visible = true;
            }
            else
            {
                showOutputToolStripMenuItem.Text = "Show Program Log";
                debugOutputListBox.Visible = false;
            }
        }

        private void saveOutputtedLoopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var localCollection = Globals.GetCollection;
            SaveFileDialog savePrompt = new SaveFileDialog();
            savePrompt.Filter = "GIF File|*.gif";
            savePrompt.Title = "Save Loop";

            if (savePrompt.ShowDialog() == DialogResult.OK)
            {
                string collectionFilePath = savePrompt.FileName;

                using (var fileStream = new FileStream(collectionFilePath, FileMode.Create))
                {
                    localCollection.Write(fileStream, MagickFormat.Gif);
                }
                Globals.SetListBoxItems($"Successfully saved loop to {collectionFilePath}.");
                statusLabel.Text = "Successfully saved loop!";
            }
            else
            {
                Globals.SetListBoxItems($"Encountered error while saving: savePrompt.ShowDialog() != DialogResult.OK");
                statusLabel.Text = "Failed to save file. Please check if name and directory are valid!";
            }
        }
    }
}