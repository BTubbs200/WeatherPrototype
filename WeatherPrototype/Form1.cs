using ImageMagick;

namespace WeatherPrototype
{
    public partial class Form1 : Form
    {
        public static class Globals
        {
            private static string imgFilePath = string.Empty;

            private static ListBox debugOutputListBox;

            public static string imgFilePathValue
            {
                get { return imgFilePath; }
                set { imgFilePath = value; }
            }

            public static void SetListBox(ListBox listBox)
            {
                debugOutputListBox = listBox;
            }

            public static void AddListBoxItems(string debugInfo)
            {
                debugOutputListBox.Items.Add(debugInfo);
            }
        }

        public Form1()
        {
            InitializeComponent();
            Globals.SetListBox(debugOutputListBox);
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
                    Globals.AddListBoxItems($"Succesfully loaded {openFileDialog.FileName}.");
                    statusLabel.Text = "File loaded successfully!";
                    demoButton.Text = "Process Loop";
                }
                else
                {
                    Globals.AddListBoxItems($"Ran into problem loading {openFileDialog.FileName}.");
                    statusLabel.Text = "Failed to load file. Check if file is valid!";
                }
            }
        }

        private void demoButton_Click(object sender, EventArgs e)
        {
            GifProcessing gp = new GifProcessing();
            MagickImageCollection collection = gp.Execute(Globals.imgFilePathValue, null);

            var memStream = new MemoryStream();

            collection.Write(memStream, MagickFormat.Gif);

            memStream.Position = 0;

            var bitmap = Image.FromStream(memStream);

            displayPicturebox.Image = bitmap;

            collection.Dispose();

            /// Does bitmap get disposed properly without explicitly disposing?
            //bitmap.Dispose();

            demoButton.Enabled = false;
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
    }
}