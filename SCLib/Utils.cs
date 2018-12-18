using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SCLib
{
    public static class Utils
    {
        public static List<List<Gift>> GenerateClusteredSolutionByLongitude(List<Gift> gifts, double longDelta, double jumpingDelta, int full)
        {
            // Make groups by longitude and sort by latitude
            var clusterAndSort = gifts.AsParallel().GroupBy(g => g.Latitute < -1.0472).SelectMany(gr => gr.AsParallel().GroupBy(g => Math.Floor(g.Longitude * longDelta)).Select(group => group.AsParallel().OrderBy(g => g.Latitute)));
            // Build groups with max weight < full
            var groupsWithLimitedWeight = clusterAndSort.AsParallel().
                SelectMany(
                    groups => groups.Aggregate(
                        new List<List<Gift>>(), 
                        (acc, g) => 
                        {
                            if (acc.Count == 0 || acc.Last().Sum(gPrev => gPrev.Weight) + g.Weight > full || (acc.Last().Count > 0 && Math.Abs(acc.Last().Last().Longitude - g.Longitude) > jumpingDelta))
                                acc.Add(new List<Gift>());
                            acc.Last().Add(g);
                            return acc;
                        }
                    )
                ).ToList();
            Console.WriteLine(CalcAllPenalty(groupsWithLimitedWeight));
            Console.WriteLine(groupsWithLimitedWeight.Count);
            Console.WriteLine(groupsWithLimitedWeight.Sum(g => g.Count));
            return groupsWithLimitedWeight;
        }

        public static double CalcAllPenalty(List<List<Gift>> groupedGifts)
        {
            double wholePenalty = 0.0;
            foreach (List<Gift> tour in groupedGifts)
            {
                wholePenalty += CalcTourPenalty(tour);
            }
            return wholePenalty;
        }

        private static Gift nordpole = new Gift(100000, 90, 0, 0);

        public static double CalcTourPenalty(List<Gift> gifts)
        {
            double w = 10.0;
            foreach (Gift g in gifts)
            {
                w += g.Weight;
            }
            // Nordpole to first gift
            double res = CalcDistance(nordpole, gifts[0]) * w;
            w -= gifts[0].Weight;
            for (int i = 0; i < gifts.Count - 1; i++)
            {
                res += CalcExactDistance(gifts[i], gifts[i + 1]) * w;
                w -= gifts[i + 1].Weight;
            }
            // Back to nordpole
            res += CalcDistance(gifts.Last(), nordpole) * 10;
            return res;
        }

        public static double CalcDistance(Gift gift1, Gift gift2)
        {
            // We use Chord Length as we have less cos and sin calls.
            double dx = gift1.X - gift2.X;
            double dy = gift1.Y - gift2.Y;
            double dz = gift1.Z - gift2.Z;
            double d = dx * dx + dy * dy + dz * dz;
            double r = Math.Sqrt(d);
            return 2 * Math.Asin(r);
        }

        private static double CalcExactDistance(Gift gift1, Gift gift2)
        {
            var R = 6371.0088; // In kilometers
            var lat1 = gift1.Latitute;
            var lat2 = gift2.Latitute;
            var lon1 = gift1.Longitude;
            var lon2 = gift2.Longitude;
            var diffLa = lat2 - lat1;
            var diffLo = lon2 - lon1;

            var a = Math.Sin(diffLa / 2) * Math.Sin(diffLa / 2) + Math.Sin(diffLo / 2) * Math.Sin(diffLo / 2) * Math.Cos(lat1) * Math.Cos(lat2);
            return R * 2 * Math.Asin(Math.Sqrt(a));
        }
    }
    
    /*public static double CalcPenalty(List<int> trip, List<Gift> gifts)
    {
        if (trip.Count == 0) return 0.0;
        double weightSum = 0.0;
        foreach (int g in trip)
        {
            weightSum += gifts[g].Weight;
        }
        return CalcPenalty(trip, gifts, weightSum);
    }

    public static double CalcPenalty(List<int> trip, List<Gift> gifts, double weightSum)
    {
        if (trip.Count == 0) return 0.0;
        double res = 0.0;
        double w = weightSum + 10.0;
        for (int i = 0; i < trip.Count - 1; i++)
        {
            res += CalcDistance(gifts, trip[i], trip[i + 1]) * w;
            w -= gifts[trip[i + 1]].Weight;
        }
        return res;
    }

    public static double CalcDistance(List<Gift> gifts, int gift1, int gift2)
    {
        // We use Chord Length as we have less cos and sin calls.
        double dx = gifts[gift1].X - gifts[gift2].X;
        double dy = gifts[gift1].Y - gifts[gift2].Y;
        double dz = gifts[gift1].Z - gifts[gift2].Z;
        double d = dx * dx + dy * dy + dz * dz;
        double r = Math.Sqrt(d);
        // We lose to much precision. We normalize the values to still be accurate.
        if (r < 0.02) return r * (2 + (2.0 / 6) * d);
        return 2 * Math.Asin(r);
    }*/
}
