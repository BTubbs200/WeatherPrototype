using Emgu.CV;
using Emgu.CV.Structure;
using ImageMagick;
using System.Diagnostics;

namespace WeatherPrototype
{
    /// <summary>
    /// Class that analyzes composite reflectivity from a radar loop
    /// and identifies areas of rain by converting the frames to HSV and 
    /// isolating colors within a specified range; has customizable data processing methods.
    /// </summary>
    public class GifProcessing
    {
        /// <summary>
        /// Method that uses a Mercator projection formula to 
        /// convert WGS84 geocoordinates to a pixel pair on a given image.
        /// </summary>
        static int[] ConvertGeoToPixel()
        {
            var usrCoords = Form1.Globals.GetGeoCoords;
            var lat = usrCoords.X; var lon = usrCoords.Y;
            var mapWidth = GeoToPixelParameters.getMapWidth;
            var mapLonDelta = GeoToPixelParameters.getMapLonDelta;
            var mapHeight = GeoToPixelParameters.getMapHeight;
            var mapLonLeft = GeoToPixelParameters.getMapLonLeft;
            var mapLatBottom = GeoToPixelParameters.getMapLatBottom;
            var mapLatBottomRadian = mapLatBottom * (Math.PI / 180);

            var worldMapRadius = mapWidth / mapLonDelta * 360 / (2 * Math.PI);
            var mapOffsetY = (worldMapRadius / 2 * Math.Log((1 + Math.Sin(mapLatBottomRadian)) / (1 - Math.Sin(mapLatBottomRadian))));
            var equatorY = mapHeight + mapOffsetY;

            var tx = (lon - mapLonLeft) * (mapWidth / mapLonDelta);

            var latRadian = lat * (Math.PI / 180);
            var a = Math.Log(Math.Tan(latRadian / 2 + Math.PI / 4));

            var ty = equatorY - (worldMapRadius * a);

            int[] array = new int[2];

            // If outputted pixel coordinates are outside of image,
            // reduce the offending value until it is within bounds.
            while (ty > mapHeight || ty < mapLatBottom)
            {
                if (ty > mapHeight)
                {
                    ty--;
                }
                if (ty < mapLatBottom)
                {
                    ty++;
                }
            }

            while (tx > mapWidth || tx < mapLonLeft)
            {
                if (tx > mapWidth)
                {
                    tx--;
                }
                if (tx < mapLonLeft)
                {
                    tx++;
                }
            }

            array[0] = (int)Math.Round(Math.Abs(tx)); array[1] = (int)Math.Round((Math.Abs(ty)));

            return array;
        }
        static MagickImageCollection CreateGif(MagickImageCollection collection)
        {
            collection = new MagickImageCollection();
            GifProcessingGettersSetters gp = new GifProcessingGettersSetters();
            List<Bitmap> frameList = GifProcessingGettersSetters.GetFrameList();
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
                            GifProcessingGettersSetters.SetCollection(image, frameLatency);
                        }
                        else
                        {
                            throw new ArgumentNullException();
                        }
                    }
                    catch (ArgumentNullException)
                    {
                        Form1.Globals.SetListBoxItems("GifProcessing.CreateGif: encountered null bmps in frameList! Terminating...");
                        Environment.Exit(1);
                    }
                }

                // Dispose the frame list to avoid errors on the next go-around.
                Bitmap? bm = null;
                GifProcessingGettersSetters.SetFrameList(bm);

                return GifProcessingGettersSetters.getCollection();
            }
        }

        static void PrepareGif(string imgFilePath)
        {
            GifProcessingGettersSetters gp = new GifProcessingGettersSetters();
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
            GifProcessingGettersSetters gp = new GifProcessingGettersSetters();
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

            // Draw an ellipse on the map that corresponds to user's inputted coordinates.
            var pixelCoords = ConvertGeoToPixel();
            var brush = new SolidBrush(Color.Blue);
            var ellipseWidth = 10; var ellipseHeight = 10;

            graphicImage.FillEllipse(brush, (pixelCoords[0] - (ellipseWidth / 2)), (pixelCoords[1] - (ellipseHeight / 2)), ellipseWidth, ellipseHeight);

            // For each pixel in mask, if pixel is populated, 1/200 chance of text being printed at x,y on graphicImage.
            /* for (int y = 0; y < mask.Rows; y++)
             {
                 for (int x = 0; x < mask.Cols; x++)
                 {
                     if (mask.Data[y, x, 0] != 0 && rand.Next(0, 200) == 1)
                     {
                         graphicImage.DrawString("RAIN", new Font("Arial", 10, FontStyle.Bold), Brushes.Red, new Point(x, y));
                     }
                 }
             }*/

            try
            {
                if (bitMapImage != null)
                {
                    GifProcessingGettersSetters.SetFrameList(bitMapImage);
                }
                else
                {
                    stream = null;
                    croppedImage.Dispose();
                    bitMapImage.Dispose();
                    graphicImage.Dispose();
                    brush.Dispose();
                    throw new ArgumentNullException();
                }
            }
            catch (ArgumentNullException)
            {
                Form1.Globals.SetListBoxItems($"GifProcessing.FrameProcessing: gif contains null frames! Frame: {counter}");
                Environment.Exit(1);
            }

            // Clean house before next loop
            stream = null;
            croppedImage.Dispose();
            graphicImage.Dispose();
            brush.Dispose();

            /// TODO: LEARN HOW TO PROPERLY DISPOSE OF 
            /// BITMAP WITHOUT CREATING NULL FRAMES
            /// IN FRAMELIST.
            //bitMapImage.Dispose();
        }

        public MagickImageCollection Execute(string imgFilePath, MagickImageCollection collection)
        {
            var timer = new Stopwatch();

            timer.Start();
            PrepareGif(imgFilePath);
            collection = CreateGif(collection);
            timer.Stop();

            Form1.Globals.SetListBoxItems($"Successfully processed {imgFilePath} in {timer.Elapsed.Milliseconds} ms.");

            return collection;
        }
    }
}