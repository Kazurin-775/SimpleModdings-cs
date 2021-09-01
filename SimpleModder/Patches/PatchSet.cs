using System.Collections.Generic;
using System.Linq;

namespace SimpleModder.Patches
{
    public class PatchSet : Patch
    {
        private readonly List<Patch> _patches;

        public PatchSet(List<RawPatch> raw, Dictionary<string, PatchSet> patchsets)
        {
            _patches = raw.Select(patch => Patch.Compile(patch, patchsets)).ToList();
        }

        public override byte[] RunOn(byte[] data)
        {
            foreach (var patch in _patches)
            {
                data = patch.RunOn(data);
            }

            return data;
        }
    }
}
