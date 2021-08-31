using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SimpleModder.Patches;

namespace SimpleModder
{
    public class PatchedFile
    {
        private readonly string _filename;
        private readonly List<Patch> _patches;

        public PatchedFile(string filename, List<RawPatch> raw)
        {
            _filename = filename;
            _patches = raw.Select(Patch.Compile).ToList();
        }

        public async Task DryRunOn(string programPath)
        {
            Logger.Log($"处理文件：{_filename}");
            var stream = File.Open(Path.Combine(programPath, _filename), FileMode.Open, FileAccess.Read);
            var data = new byte[stream.Length];
            Trace.Assert(await stream.ReadAsync(data, 0, data.Length) == data.Length);
            stream.Close();

            foreach (var patch in _patches)
            {
                data = patch.RunOn(data);
            }
        }
    }
}
