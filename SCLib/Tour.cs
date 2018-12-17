using System.Collections.Generic;
using System.Linq;

namespace SCLib
{
    public class Tour
    {
        public readonly List<Gift> Gifts;
        private readonly List<Gift> _reverse;

        public double Cost { get; private set; }

        public Tour(IEnumerable<Gift> gifts)
        {
            Gifts = gifts.ToList();
            _reverse = new List<Gift>(Gifts.Count);
            Cost = Utils.CalcTourPenalty(Gifts);
        }

        private Tour(IEnumerable<Gift> gifts, double cost)
        {
            Gifts = gifts.ToList();
            Cost = cost;
        }

        public Tour Clone() => new Tour(Gifts, Cost);

        /**
         * Swap 2 element in the tour, reverse list if reverse tour has lower cost
         * Updates cost
         */
        public void Swap(int a, int b)
        {
            var temp = Gifts[a];
            Gifts[a] = Gifts[b];
            Gifts[b] = temp;
            var forwardCost = Utils.CalcTourPenalty(Gifts);

            _reverse.Clear();
            _reverse.AddRange(Gifts);
            _reverse.Reverse();

            var backwardCost = Utils.CalcTourPenalty(_reverse);
            if (backwardCost < forwardCost)
            {
                Cost = backwardCost;
                Gifts.Reverse();
            }
            else
            {
                Cost = forwardCost;
            }
        }
    }
}
