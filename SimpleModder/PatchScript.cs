using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleModder
{
    public class PatchScript
    {
        public readonly string Name;
        public readonly string DefaultPath;
        public readonly string Comments;

        private readonly Dictionary<string, PatchedFile> _patches = new Dictionary<string, PatchedFile>();

        public PatchScript(RawPatchScript raw)
        {
            Name = raw.Name;
            DefaultPath = raw.DefaultPath;
            Comments = raw.Comments;

            foreach (var patch in raw.Patches)
            {
                _patches[patch.Key] = new PatchedFile(patch.Key, patch.Value);
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
    }
}
