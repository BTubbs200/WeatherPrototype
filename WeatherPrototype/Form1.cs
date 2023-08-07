using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Drawing;
using ImageMagick;

namespace WeatherPrototype
{
    public partial class Form1 : Form
    {
        public static class Globals
        {
            private static string imgFilePath = string.Empty;
            public static string imgFilePathValue
            {
                get { return imgFilePath; }
                set { imgFilePath = value; }
            }
        }

        public Form1()
        {
            InitializeComponent();
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
                    debugToolStripMenuItem.Enabled = true;
                    demoButton.Enabled = true;
                    statusLabel.Text = "File loaded successfully!";
                    demoButton.Text = "Process Loop";
                }
                else
                {
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
    }
}