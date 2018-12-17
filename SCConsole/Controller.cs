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

            Tour.Gifts.AddRange(gifts);
            var tours = new List<Tour>();

            // optimize solution
            Console.WriteLine("Optimize solution");
            tours.AsParallel().Select(HillClimber.Run);
            
        }

        private static float ToNormalRange(float x, float min, float max) => (x - min) / (max - min) * 2 - 1;
    }
}
