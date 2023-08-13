namespace WeatherPrototype
{
    public static class GeoToPixelParameters
    {
        private static readonly double mapWidth = 600D;
        private static readonly double mapHeight = 501D;
        private static readonly double mapLonLeft = -97.90D;
        private static readonly double mapLonRight = -82.04D;
        private static readonly double mapLonDelta = mapLonRight - mapLonLeft;
        private static readonly double mapLatBottom = 26.95D;

        public static double getMapWidth
        {
            get { return mapWidth; }
        }

        public static double getMapHeight
        {
            get { return mapHeight; }
        }

        public static double getMapLonLeft
        {
            get { return mapLonLeft; }
        }

        public static double getMapLonDelta
        {
            get { return mapLonDelta; }
        }

        public static double getMapLatBottom
        {
            get { return mapLatBottom; }
        }
    }
}