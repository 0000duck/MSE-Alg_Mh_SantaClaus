﻿using System;
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
        private readonly string _path;

        public Controller()
        {
            var config = JObject.Parse(File.ReadAllText(@"config.json"));
            _path = (string)config["gifts"];
            var window = new GLWindow(800, 450, "Santa Claus");
            window.Loaded += (sender, args) => Load((GLWindow) sender);
            window.Run();
        }

        public void Load(GLWindow window)
        {
            Console.WriteLine($"Loading data from {_path}");
            var gifts = IOHandler.Load(_path);
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

            // create initial solution
            Console.WriteLine("Creating initial solution");
            var initial = Utils.GenerateClusteredSolutionByLongitude(new List<Gift>(gifts), 0.5, 0.2, 980); //TODO adjust
            Console.WriteLine("Initial solution completed");
            var tours = initial.Select(list => new Tour(list)).ToList();
            window.SetTour(tours.Select(tour => tour.Gifts.Select(gift => gift.Id - 1).ToArray()).ToList());

            // optimize solution
            Console.WriteLine("Optimize solution");
            tours = tours.AsParallel().Select(HillClimber.Run).AsSequential().ToList();
            Console.WriteLine("Solution completed");
            window.SetTour(tours.Select(tour => tour.Gifts.Select(gift => gift.Id - 1).ToArray()).ToList());
            Console.WriteLine($"Total score: {tours.Sum(tour => tour.Cost)}");
            for (var i = 0; i < tours.Count; i++)
            {
                var tour = tours[i];
#if DEBUG
                Console.WriteLine($"Tour {i}, score: {tour.Cost}");
                Console.WriteLine($"Gifts: {string.Join(", ", tour.Gifts.Select(gift => gift.Id))}");
#endif
            }
        }
        private static float ToNormalRange(float x, float min, float max) => (x - min) / (max - min) * 2 - 1;
    }
}
