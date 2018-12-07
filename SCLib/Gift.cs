namespace SCLib
{
    public struct Gift
    {
        public readonly uint Id;
        public readonly double Latitute;
        public readonly double Longitude;
        public readonly double Weight;

        public Gift(uint id, double latitute, double longitude, double weight)
        {
            Id = id;
            Latitute = latitute;
            Longitude = longitude;
            Weight = weight;
        }
    }
}
