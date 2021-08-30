using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace SimpleModder
{
    public struct RawPatchScript
    {
        public string Name;
        public string DefaultPath;
        public string Comments;

        public Dictionary<string, List<RawPatch>> Patches;

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

    public struct RawPatch
    {
        public string Kind;
        public string Original;
        public string Replaced;
        public string Comments;
    }
}
