using System;
using System.Collections.Generic;
using System.Text;

namespace SCLib
{
    static class Utils
    {
        public static double CalcPenalty(List<int> trip, List<Gift> gifts)
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
        }
    }
}
