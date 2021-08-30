using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SimpleModder
{
    public static class PatchesList
    {
        public static List<string> Obtain()
        {
            if (!Directory.Exists("patches"))
                return new List<string>();
            return Directory.EnumerateFiles("patches")
                .Where(File.Exists)
                .Select(Path.GetFileName)
                .ToList();
        }
    }
}
