using System.Collections.Generic;
using System.Threading.Tasks;
using SimpleModder.Patches;

namespace SimpleModder
{
    public class PatchScript
    {
        public readonly string Name;
        public readonly string DefaultPath;
        public readonly string Comments;

        private readonly Dictionary<string, PatchedFile> _patches = new();
        internal readonly Dictionary<string, PatchSet> Patchsets = new();
        internal readonly Dictionary<string, FileSearchCond> Search = new();

        public PatchScript(RawPatchScript raw)
        {
            Name = raw.Name;
            DefaultPath = raw.DefaultPath;
            Comments = raw.Comments;

            if (raw.Patchsets != null)
            {
                foreach (var patchset in raw.Patchsets)
                {
                    Patchsets[patchset.Key] = new PatchSet(patchset.Value, Patchsets);
                }
            }

            if (raw.Search != null)
            {
                foreach (var search in raw.Search)
                {
                    Search[search.Key] = new FileSearchCond(search.Key, search.Value);
                }
            }

            foreach (var patch in raw.Patches)
            {
                _patches[patch.Key] = new PatchedFile(patch.Key, patch.Value, this);
            }
        }

        public async Task DryRun(string programPath)
        {
            Logger.Log("开始执行补丁【测试模式】");
            foreach (var patchedFile in _patches)
            {
                await patchedFile.Value.DryRunOn(programPath);
            }

            Logger.Log("补丁执行完成");
        }

        public async Task Run(string programPath)
        {
            Logger.Log("开始执行补丁");
            foreach (var patchedFile in _patches)
            {
                await patchedFile.Value.RunOn(programPath);
            }

            Logger.Log("补丁执行完成");
        }
    }
}
