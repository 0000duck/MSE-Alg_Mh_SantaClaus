using System.Collections.Generic;
using System.Linq;

namespace SCLib
{
    public class Tour
    {
        public static List<Gift> Gifts = new List<Gift>();

        public readonly List<int> Indices;
        private readonly List<int> _reverse;

        public double Cost { get; private set; }

        public Tour(IEnumerable<int> indices)
        {
            Indices = indices.ToList();
            _reverse = new List<int>(Indices.Count);
            Cost = ComputeCost(Indices);
        }

        /**
         * Swap 2 element in the tour, reverse list if reverse tour has lower cost
         * Updates cost
         */
        public void Swap(int a, int b)
        {
            var temp = Indices[a];
            Indices[a] = Indices[b];
            Indices[b] = temp;
            var forwardCost = ComputeCost(Indices);

            _reverse.Clear();
            _reverse.AddRange(Indices);
            _reverse.Reverse();

            var backwardCost = ComputeCost(_reverse);
            if (backwardCost < forwardCost)
            {
                Cost = backwardCost;
                Indices.Reverse();
            }
            else
            {
                Cost = forwardCost;
            }
        }

        private static double ComputeCost(List<int> indices)
        {
            return Utils.CalcPenalty(indices, Gifts);
        }
    }
}
