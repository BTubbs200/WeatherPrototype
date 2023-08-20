using Emgu.CV;
using Emgu.CV.Structure;
using ImageMagick;
using System.Diagnostics;
using System.Drawing;
using System.Net.Security;
using static WeatherPrototype.Form1;

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
            GifProcessingGlobals gp = new GifProcessingGlobals();
            List<Bitmap> frameList = GifProcessingGlobals.GetFrameList();
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
                            GifProcessingGlobals.SetCollection(image, frameLatency);
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
                GifProcessingGlobals.SetFrameList(bm);

                return GifProcessingGlobals.getCollection();
            }
        }

        static void PrepareGif(string imgFilePath)
        {
            GifProcessingGlobals gp = new GifProcessingGlobals();
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

        /// <summary>
        /// Method that checks for white mask pixels (areas of rain)
        /// within two certain perimeter thresholds of user location,
        /// writes a message to output log if rain is detected in 
        /// two respective thresholds. (inner and outer west, inner and outer south, etc.)
        /// </summary>
        static void RainCheck(Image<Gray, byte> mask)
        {
            var pixelCoords = ConvertGeoToPixel();

            // Create all lines necessary to create two encapsulated quadrilaterals around user location pixel.
            var innerNorthLineWestPoint = new Point(pixelCoords[0] - 1, pixelCoords[1] - 2);
            var innerNorthLineEastPoint = new Point(pixelCoords[0] + 1, pixelCoords[1] - 2);
            var innerSouthLineWestPoint = new Point(pixelCoords[0] - 1, pixelCoords[1] + 2);
            var innerSouthLineEastPoint = new Point(pixelCoords[0] + 1, pixelCoords[1] + 2);
            var innerWestLineSouthPoint = new Point(pixelCoords[0] - 2, pixelCoords[1] + 1);
            var innerWestLineNorthPoint = new Point(pixelCoords[0] - 2, pixelCoords[1] - 1);
            var innerEastLineSouthPoint = new Point(pixelCoords[0] + 2, pixelCoords[1] + 1);
            var innerEastLineNorthPoint = new Point(pixelCoords[0] + 2, pixelCoords[1] - 1);

            var outerNorthLineWestPoint = new Point(pixelCoords[0] - 3, pixelCoords[1] - 4);
            var outerNorthLineEastPoint = new Point(pixelCoords[0] + 3, pixelCoords[1] - 4);
            var outerSouthLineWestPoint = new Point(pixelCoords[0] - 3, pixelCoords[1] + 4);
            var outerSouthLineEastPoint = new Point(pixelCoords[0] + 3, pixelCoords[1] + 4);
            var outerWestLineSouthPoint = new Point(pixelCoords[0] - 4, pixelCoords[1] + 3);
            var outerWestLineNorthPoint = new Point(pixelCoords[0] - 4, pixelCoords[1] - 3);
            var outerEastLineSouthPoint = new Point(pixelCoords[0] + 4, pixelCoords[1] + 3);
            var outerEastLineNorthPoint = new Point(pixelCoords[0] + 4, pixelCoords[1] - 3);

            Boolean breakCheck = false;

            // Loop through outer direction line, if rain detected in outer line then loop through
            // respective inner line. If rain found there, send message to output log.

            for (int x = outerNorthLineWestPoint.X; x <= outerNorthLineEastPoint.X; x++)
            {
                if (breakCheck == true) break;

                if (mask.Data[outerNorthLineWestPoint.Y, x, 0] != 0)
                {
                    for (int z = innerNorthLineWestPoint.X; z <= innerNorthLineEastPoint.X; z++)
                    {
                        if (mask.Data[innerNorthLineWestPoint.Y, z, 0] != 0)
                        {
                            Globals.SetListBoxItems("Rain approaches from the north!");
                            breakCheck = true;
                            break;
                        }
                    }
                }
            }

            breakCheck = false;

            for (int x = outerSouthLineWestPoint.X; x <= outerSouthLineEastPoint.X; x++)
            {
                if (breakCheck == true) break;

                if (mask.Data[outerNorthLineWestPoint.Y, x, 0] != 0)
                {
                    for (int z = innerSouthLineWestPoint.X; z <= innerSouthLineEastPoint.X; z++)
                    {
                        if (mask.Data[innerSouthLineWestPoint.Y, z, 0] != 0)
                        {
                            Globals.SetListBoxItems("Rain approaches from the south!");
                            breakCheck = true;
                            break;
                        }
                    }
                }
            }

            breakCheck = false;

            for (int y = outerWestLineSouthPoint.Y; y >= outerWestLineNorthPoint.Y; y--)
            {
                if (breakCheck == true) break;

                if (mask.Data[y, outerWestLineSouthPoint.X, 0] != 0)
                {
                    for (int z = innerWestLineSouthPoint.Y; z >= innerWestLineNorthPoint.Y; z--)
                    {
                        if (mask.Data[z, innerWestLineSouthPoint.X, 0] != 0)
                        {
                            Globals.SetListBoxItems("Rain approaches from the west!");
                            breakCheck = true;
                            break;
                        }
                    }
                }
            }

            breakCheck = false;

            for (int y = outerEastLineSouthPoint.Y; y >= outerEastLineNorthPoint.Y; y--)
            {
                if (breakCheck == true) break;

                if (mask.Data[y, outerEastLineSouthPoint.X, 0] != 0)
                {
                    for (int z = innerEastLineSouthPoint.Y; z >= innerEastLineNorthPoint.Y; z--)
                    {
                        if (mask.Data[z, innerEastLineSouthPoint.X, 0] != 0)
                        {
                            Globals.SetListBoxItems("Rain approaches from the east!");
                            breakCheck = true;
                            break;
                        }
                    }
                }
            }

            /// (Debugging) make threshold perimeters around user location visible.
            /*  
                var penOrange = new Pen(Color.Orange);
                var penBlue = new Pen(Color.Blue);
                var penGreen = new Pen(Color.Green);
                var penRed = new Pen(Color.Red);
                graphicImage.DrawLine(penOrange, innerNorthLineWestPoint, innerNorthLineEastPoint);
                graphicImage.DrawLine(penBlue, innerSouthLineWestPoint, innerSouthLineEastPoint);
                graphicImage.DrawLine(penGreen, innerWestLineSouthPoint, innerWestLineNorthPoint);
                graphicImage.DrawLine(penRed, innerEastLineSouthPoint, innerEastLineNorthPoint);

                graphicImage.DrawLine(penOrange, outerNorthLineWestPoint, outerNorthLineEastPoint);
                graphicImage.DrawLine(penBlue, outerSouthLineWestPoint, outerSouthLineEastPoint);
                graphicImage.DrawLine(penGreen, outerWestLineSouthPoint, outerWestLineNorthPoint);
                graphicImage.DrawLine(penRed, outerEastLineSouthPoint, outerEastLineNorthPoint);   
            */
        }

        static void FrameProcessing(Image<Bgr, byte> myImage, int counter)
        {
            GifProcessingGlobals gp = new GifProcessingGlobals();
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

            RainCheck(mask);

            // Initialize a Bgr Graphics bitmap identical to MyImage to be used for better processing capabilities.
            var bitMapImage = new Bitmap(stream, true);
            bitMapImage = bitMapImage.Clone(roi, bitMapImage.PixelFormat);
            var graphicImage = Graphics.FromImage(bitMapImage);

            var pixelCoords = ConvertGeoToPixel();
            var brush = new SolidBrush(Color.Blue);
            var ellipseWidth = 7; var ellipseHeight = 7;

            // Render user coordinates as a blue ellipse on loop. 
            graphicImage.FillEllipse(brush, (pixelCoords[0] - (ellipseWidth / 2)), (pixelCoords[1] - (ellipseHeight / 2)), ellipseWidth, ellipseHeight);

            try
            {
                if (bitMapImage != null)
                {
                    GifProcessingGlobals.SetFrameList(bitMapImage);
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
                Globals.SetListBoxItems($"GifProcessing.FrameProcessing: gif contains null frames! Frame: {counter}");
                Environment.Exit(1);
            }

            // Clean house before next loop
            stream = null;
            croppedImage.Dispose();
            graphicImage.Dispose();
            brush.Dispose();
        }

        public MagickImageCollection Execute(string imgFilePath, MagickImageCollection collection)
        {
            var timer = new Stopwatch();

            timer.Start();
            PrepareGif(imgFilePath);
            collection = CreateGif(collection);
            timer.Stop();

            Globals.SetListBoxItems($"Successfully processed {imgFilePath} in {timer.Elapsed.Milliseconds} ms.");

            return collection;
        }
    }
}