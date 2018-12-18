using System;
using System.Collections.Generic;
using SCLib;

namespace SCConsole
{
    class TourEventArgs : EventArgs
    {
        public readonly List<Tour> Tours;

        public TourEventArgs(List<Tour> tours)
        {
            Tours = tours;
        }
    }
}