using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task DryRun(string programPath)
        {
            Logger.Log("开始执行补丁【测试模式】");
            foreach (var patchedFile in _patches)
            {
                Logger.Log($"处理文件：{patchedFile.Key}");
                var stream = File.Open(Path.Combine(programPath, patchedFile.Key), FileMode.Open, FileAccess.Read);
                var data = new byte[stream.Length];
                Trace.Assert(await stream.ReadAsync(data, 0, data.Length) == data.Length);
                stream.Close();

                foreach (var patch in patchedFile.Value)
                {
                    data = patch.RunOn(data);
                }
            }
            Logger.Log("补丁执行完成");
        }
    }
}
