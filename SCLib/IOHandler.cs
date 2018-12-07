using System.Collections.Generic;
using System.IO;

namespace SCLib
{
    public static class IOHandler
    {
        public static Gift[] Load(string path)
        {
            var list = new List<Gift>(10000);
            using (var sr = new StreamReader(new BufferedStream(File.OpenRead(path))))
            {
                sr.ReadLine();
                while (!sr.EndOfStream)
                {
                    var values = sr.ReadLine().Split(',');
                    list.Add(new Gift(uint.Parse(values[0]), double.Parse(values[1]), double.Parse(values[2]), double.Parse(values[3])));
                }
            }
            return list.ToArray();
        }
    }
}
