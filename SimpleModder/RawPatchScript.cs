using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace SimpleModder
{
    public class RawPatchScript
    {
        public string Name;
        public string DefaultPath;
        public string Comments;

        public Dictionary<string, List<RawPatch>> Patches;
        public Dictionary<string, List<RawPatch>> Patchsets;
        public Dictionary<string, RawSearchCond> Search;

        private static readonly IDeserializer Deserializer = new DeserializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .Build();

        public static async Task<RawPatchScript> LoadFromFile(string path)
        {
            var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            var reader = new StreamReader(stream);
            var content = await reader.ReadToEndAsync();
            reader.Close();

            return Deserializer.Deserialize<RawPatchScript>(content);
        }
    }

    public class RawPatch
    {
        public string Kind;
        public string Original;
        public string Replaced;
        public int Occurrences = 1;
        public string Comments;
        public string Name;
    }

    public class RawSearchCond
    {
        public string Regex;
    }
}
