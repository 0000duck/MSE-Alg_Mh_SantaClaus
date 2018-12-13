using System;
using System.Collections.Generic;
using System.Linq;

namespace SCLib
{
    public class Sleigh
    {
        private const double BaseWeight = 10.0;
        private const double CargoCapacity = 1000.0;

        private double _cargoWeight;
        public double Weight => BaseWeight + _cargoWeight;

        private readonly ISet<Gift> _gifts;
        public Gift[] Gifts => _gifts.ToArray();

        public Sleigh()
        {
            _gifts = new HashSet<Gift>();
        }

        public void AddGifts(IEnumerable<Gift> gifts)
        {
            var addWeight = gifts.Sum(gift => gift.Weight);
            if (_cargoWeight + addWeight > CargoCapacity) throw new Exception("Too much weight");
            _cargoWeight += addWeight;
            _gifts.UnionWith(gifts);
        }

        public void RemoveGift(Gift gift)
        {
            if (!_gifts.Contains(gift)) throw new Exception("Gift is not on sleigh");
            _gifts.Remove(gift);
            _cargoWeight -= gift.Weight;
        }
    }
}
