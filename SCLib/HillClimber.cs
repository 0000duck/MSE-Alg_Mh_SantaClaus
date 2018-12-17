using System;

namespace SCLib
{
    public class HillClimber
    {
        private const int T = 100000;

        public static Tour Run(Tour tour)
        {
            var n = tour.Indices.Count;
            var rnd = new Random(17);
            var t = 0.0;
            var solution = tour.Clone();

            while (t < T)
            {
                var a = rnd.Next(n);
                var b = rnd.Next(n);
                tour.Swap(a, b);
                if (tour.Cost < solution.Cost)
                {
                    solution = tour.Clone();
                    t = 0;
                }
                else
                {
                    t++;
                }
            }

            return solution;
        }
    }
}
