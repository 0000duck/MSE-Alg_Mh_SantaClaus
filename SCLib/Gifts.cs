using System;
using System.Collections.Generic;
using System.Linq;

namespace SCLib
{
    public class Gifts
    {
        private readonly Gift[] _gifts;

        public Gifts(IEnumerable<Gift> gifts)
        {
            _gifts = gifts.ToArray();
            BuildKdTree();
        }

        public Gift this[int id] => _gifts[id];

        private void BuildKdTree()
        {

        }

        public IEnumerable<Gift> Search(double longitude, double latitude, double radius)
        {
            throw new NotImplementedException();
        }
    }
}
