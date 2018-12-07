using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SCLib;

namespace Alg_Mh_SantaClaus
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = JObject.Parse(File.ReadAllText(@"config.json"));
            var path = (string) config["datafile"];
            var gifts = IOHandler.Load(path);
            Console.WriteLine($"loaded gifts {gifts.Length}");
        }
    }
}
