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
        private readonly string _pathGifts;
        private readonly string _pathSolution;

        public Controller()
        {
            var config = JObject.Parse(File.ReadAllText(@"config.json"));
            _pathGifts = (string) config["gifts"];
            _pathSolution = (string) config["solution"];
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
            var minLong = (float) -Math.PI;
            var maxLong = (float) Math.PI;
            var minLat = (float) -Math.PI / 2;
            var maxLat = (float) Math.PI / 2;
            var n = gifts.Length;
            var vertGifts = new List<Gift>(n + 1);
            vertGifts.Add(new Gift(0, 90, 0, 0));
            vertGifts.AddRange(gifts);

            var vertices = vertGifts.SelectMany(x => new[]
            {
                ToNormalRange((float) x.Longitude, minLong, maxLong),
                ToNormalRange((float) x.Latitute, minLat, maxLat)
            }).ToArray();

            // showing gifts
            Console.WriteLine("Showing data");
            window.SetVertices(vertices, n + 1);

            //window.RunBackground(() =>
            //{
                // create initial solution
                Console.WriteLine("Creating initial solution");
                var initial = Utils.GenerateClusteredSolutionByLongitude(new List<Gift>(gifts),
                    new double[] {0.02, 0.04}, new double[] {0.04, 0.06}, 980); //TODO adjust
                Console.WriteLine(Utils.CalcAllPenalty(initial));
                Console.WriteLine("Initial solution completed");
                var tours = initial.Select(list => new Tour(list)).ToList();
                window.SetTour(tours.Select(tour => tour.Gifts.Select(gift => gift.Id).ToArray()).ToList());

                // optimize solution
                Console.WriteLine("Optimize solution");
                tours = tours.AsParallel().Select(HillClimber.Run).ToList();
                Console.WriteLine("Solution completed");
                window.SetTour(tours.Select(tour => tour.Gifts.Select(gift => gift.Id).ToArray()).ToList());

                Console.WriteLine($"Total score: {tours.Sum(tour => tour.Cost)}");
                Console.WriteLine($"Saving solution in {_pathSolution}");
                IOHandler.Save(_pathSolution, n, tours);
                Console.WriteLine($"Done");
            //});
        }

        private static float ToNormalRange(float x, float min, float max) => (x - min) / (max - min) * 2 - 1;
    }
}