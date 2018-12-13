using System;

namespace SCLib
{
    public struct Gift
    {
        public readonly uint Id;
        public readonly double Latitute;
        public readonly double Longitude;
        public readonly double Weight;
        public readonly double X, Y, Z;

        public Gift(uint id, double latitute, double longitude, double weight)
        {
            Id = id;
            // Use radians.
            Latitute = latitute * Math.PI / 180;
            Longitude = longitude * Math.PI / 180;
            Weight = weight;
            // We precalc x, y, z.
            X = 0.5 * Math.Cos(Latitute) * Math.Sin(Longitude);
            Y = 0.5 * Math.Cos(Latitute) * Math.Cos(Longitude);
            Z = 0.5 * Math.Sin(Latitute);
        }
    }
}
