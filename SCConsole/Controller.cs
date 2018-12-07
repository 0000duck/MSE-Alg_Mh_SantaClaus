using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SCLib;
using SCVis;

namespace SCConsole
{
    public class Controller
    {
        private readonly string _path;
        private Gift[] _gifts;

        public Controller()
        {
            var config = JObject.Parse(File.ReadAllText(@"config.json"));
            _path = (string)config["datafile"];
            var window = new GLWindow(800, 450, "Santa Claus");
            window.Loaded += (sender, args) => Load((GLWindow) sender);
            window.Run();
        }

        public void Load(GLWindow window)
        {
            Console.WriteLine($"Loading data from {_path}");
            _gifts = IOHandler.Load(_path);
            // preprocessing
            Console.WriteLine("Preprocessing data");
            var minX = double.MaxValue;
            var minY = double.MaxValue;
            var maxX = double.MinValue;
            var maxY = double.MinValue;
            foreach (var gift in _gifts)
            {
                var lat = gift.Latitute;
                var lot = gift.Longitude;
                minX = Math.Min(minX, lot);
                maxX = Math.Max(maxX, lot);
                minY = Math.Min(minY, lat);
                maxY = Math.Max(maxY, lat);
            }

            Console.WriteLine("Showing data");
            window.SetInstance(_gifts, (float) minX, (float) maxX, (float) minY, (float) maxY);
        }
    }
}
