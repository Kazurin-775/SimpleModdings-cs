using System.Collections.Generic;
using System.Linq;
using SimpleModder.Patches;

namespace SimpleModder
{
    public class PatchScript
    {
        public readonly string Name;
        public readonly string DefaultPath;
        public readonly string Comments;

        private readonly Dictionary<string, List<Patch>> _patches = new Dictionary<string, List<Patch>>();

        public PatchScript(RawPatchScript raw)
        {
            Name = raw.Name;
            DefaultPath = raw.DefaultPath;
            Comments = raw.Comments;

            foreach (var patch in raw.Patches)
            {
                _patches[patch.Key] = patch.Value.Select(Patch.Compile).ToList();
            }
        }
    }
}
