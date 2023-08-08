using ImageMagick;

namespace WeatherPrototype
{
    /// <summary>
    /// Getters and setters class for GifProcessing.
    /// </summary>
    public class GPGetterSetters
    {
        private Rectangle roi = new Rectangle(0, 24, 600, 501);

        private static List<Bitmap> frameList = new List<Bitmap>();

        private static MagickImageCollection collection = new MagickImageCollection();

        public Rectangle GetRoi()
        {
            return this.roi;
        }

        public static List<Bitmap> GetFrameList()
        {
            return frameList;
        }

        public static MagickImageCollection getCollection()
        {
            return collection;
        }

        public static void SetFrameList(Bitmap? bitMapImage)
        {
            // A null bitmap can only make it this far if 
            // the frame list is intentionally being cleared.
            if (bitMapImage == null)
            {
                frameList.Clear();
            }
            else if (frameList != null)
            {
                frameList.Add(bitMapImage);
            }
        }

        public static void SetCollection(MagickImage image, int frameLatency)
        {
            collection.Add(image);
            collection[collection.Count - 1].AnimationDelay = frameLatency;
        }
    }
}
