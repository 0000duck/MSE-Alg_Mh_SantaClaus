using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SCLib;
using SCVis;

namespace SCConsole
{
    public class Controller
    {
        private readonly string _pathGifts;
        private readonly string _pathSolution;

        private event EventHandler<TourEventArgs> _updateTour;

        public Controller()
        {
            var config = JObject.Parse(File.ReadAllText(@"config.json"));
            _pathGifts = (string)config["gifts"];
            _pathSolution = (string)config["solution"];
            var window = new GLWindow(800, 450, "Santa Claus");
            window.Loaded += (sender, args) => Load((GLWindow) sender);
            window.Run();
        }

        public void Load(GLWindow window)
        {
            Console.WriteLine($"Loading data from {_pathGifts}");
            var gifts = IOHandler.Load(_pathGifts);
            // preprocessing
            Console.WriteLine("Preprocessing data");
            var minX = double.MaxValue;
            var minY = double.MaxValue;
            var maxX = double.MinValue;
            var maxY = double.MinValue;
            foreach (var gift in gifts)
            {
                var lat = gift.Latitute;
                var lot = gift.Longitude;
                minX = Math.Min(minX, lot);
                maxX = Math.Max(maxX, lot);
                minY = Math.Min(minY, lat);
                maxY = Math.Max(maxY, lat);
            }
            var fminX = (float) minX;
            var fminY = (float) minY;
            var fmaxX = (float) maxX;
            var fmaxY = (float) maxY;
            var n = gifts.Length;
            var vertices = gifts.SelectMany(x => new[]
            {
                ToNormalRange((float) x.Longitude, fminX, fmaxX),
                ToNormalRange((float) x.Latitute, fminY, fmaxY)
            }).ToArray();

            // showing gifts
            Console.WriteLine("Showing data");
            window.SetVertices(vertices, n);

            _updateTour += (sender, args) =>
            {
                window.SetTour(args.Tours.Select(tour => tour.Gifts.Select(gift => gift.Id - 1).ToArray()).ToList());
            };

            Task.Run(() =>
            {
                // create initial solution
                Console.WriteLine("Creating initial solution");
                var initial = Utils.GenerateClusteredSolutionByLongitude(new List<Gift>(gifts), 0.5, 980); //TODO adjust
                Console.WriteLine("Initial solution completed");
                var tours = initial.Select(list => new Tour(list)).ToList();
                _updateTour?.Invoke(this, new TourEventArgs(tours));

                // optimize solution
                Console.WriteLine("Optimize solution");
                tours = tours.AsParallel().Select(HillClimber.Run).ToList();
                Console.WriteLine("Solution completed");
                _updateTour?.Invoke(this, new TourEventArgs(tours));
                Console.WriteLine($"Total score: {tours.Sum(tour => tour.Cost)}");
                Console.WriteLine($"Saving solution in {_pathSolution}");
                IOHandler.Save(_pathSolution, tours);
            });
            
        }

        private static float ToNormalRange(float x, float min, float max) => (x - min) / (max - min) * 2 - 1;
    }
}
