using Emgu.CV;
using Emgu.CV.Structure;
using WeatherPrototype;
using ImageMagick;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace WeatherPrototype
{
    /// <summary>
    /// Class that analyzes composite reflectivity from a radar loop
    /// and identifies areas of rain by converting the frames to HSV, 
    /// isolating colors within a specified range, and printing the text 
    /// "RAIN" on a each frame where areas of rain are identified.
    /// Processed frames are returned as a MagickImageCollecition GIF.
    /// </summary>
    public class GifProcessing
    {
        static MagickImageCollection CreateGif(MagickImageCollection collection)
        {
            collection = new MagickImageCollection();
            GPGetterSetters gp = new GPGetterSetters();
            List<Bitmap> frameList = GPGetterSetters.GetFrameList();
            int frameLatency = 30;

            // Loop through each frame in frameList and append it to a new GIF.
            using (collection)
            {
                foreach (var frame in frameList)
                {
                    try
                    {
                        if (frame != null)
                        {
                            byte[] byteArr;
                            using (MemoryStream stream = new MemoryStream())
                            {
                                frame.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                                byteArr = stream.ToArray();
                            }

                            MagickImage image = new MagickImage(byteArr);
                            GPGetterSetters.SetCollection(image, frameLatency);
                        }
                        else
                        {
                            throw new ArgumentNullException();
                        }
                    }
                    catch (ArgumentNullException)
                    {
                        Console.WriteLine("Error: CreateGif() encountered null bmps in frameList! Terminating...");
                        Environment.Exit(1);
                    }
                }

                // Dispose the frame list to avoid errors on the next go-around.
                Bitmap? bm = null;
                GPGetterSetters.SetFrameList(bm);

                return GPGetterSetters.getCollection();
            }
        }

        static void PrepareGif(string imgFilePath)
        {
            GPGetterSetters gp = new GPGetterSetters();
            VideoCapture gif = new VideoCapture(imgFilePath, VideoCapture.API.Any);
            int counter = 1;

            // Loop through each frame in inputted GIF and send it to FrameProcessing() as BGR image.
            while (true)
            {
                Mat frame = gif.QueryFrame();

                if (frame == null) break;

                Image<Bgr, byte> displayImg = frame.ToImage<Bgr, byte>();

                FrameProcessing(displayImg, counter);
                displayImg.Dispose();

                counter++;
            }

            gif.Dispose();
        }

        static void FrameProcessing(Image<Bgr, byte> myImage, int counter)
        {
            GPGetterSetters gp = new GPGetterSetters();
            var roi = gp.GetRoi();

            // Take image, crop according to roi; create mask that identifies colors within a specified HSV range.
            Image<Hsv, byte> myImageHsv = myImage.Convert<Hsv, byte>();
            Image<Hsv, byte> croppedImage = myImageHsv.Copy(roi);
            var stream = new MemoryStream(myImage.ToJpegData(100));

            myImageHsv.Dispose();
            myImage.Dispose();

            Hsv lowerBound = new Hsv(20, 67, 136);
            Hsv upperBound = new Hsv(180, 255, 255);
            Image<Gray, byte> mask = croppedImage.InRange(lowerBound, upperBound);

            ////////// MASK DEBUG ///////////////
            /*CvInvoke.Imshow("test", mask);
            CvInvoke.WaitKey(0);*/
            ///////////////////////////////////

            // Initialize a Bgr Graphics bitmap identical to MyImage to be used for better processing capabilities.
            var bitMapImage = new Bitmap(stream, true);
            bitMapImage = bitMapImage.Clone(roi, bitMapImage.PixelFormat);
            var graphicImage = Graphics.FromImage(bitMapImage);
            Random rand = new Random();

            // For each pixel in mask, if pixel is populated, 1/1000 chance of text being printed on at x,y on graphicImage.
            for (int y = 0; y < mask.Rows; y++)
            {
                for (int x = 0; x < mask.Cols; x++)
                {
                    if (mask.Data[y, x, 0] != 0 && rand.Next(0, 200) == 1)
                    {
                        graphicImage.DrawString("RAIN", new Font("Arial", 10, FontStyle.Bold), Brushes.Red, new Point(x, y));
                    }
                }
            }

            try
            {
                if (bitMapImage != null)
                {
                    GPGetterSetters.SetFrameList(bitMapImage);
                }
                else
                {
                    stream = null;
                    croppedImage.Dispose();
                    bitMapImage.Dispose();
                    graphicImage.Dispose();
                    throw new ArgumentNullException();
                }
            }
            catch (ArgumentNullException)
            {
                Console.WriteLine($"Error: gif contains null frames! Frame: {counter}");
                Environment.Exit(1);
            }

            // Clean house before next loop
            stream = null;
            croppedImage.Dispose();
            graphicImage.Dispose();

            /// TODO: LEARN HOW TO PROPERLY DISPOSE OF 
            /// BITMAP WITHOUT CREATING NULL FRAMES
            /// IN FRAMELIST.
            //bitMapImage.Dispose();
        }

        public MagickImageCollection Execute(string imgFilePath, MagickImageCollection collection)
        { 
            PrepareGif(imgFilePath);
            collection = CreateGif(collection);

            return collection;
        }
    }
}