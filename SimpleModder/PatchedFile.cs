using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using SimpleModder.Patches;

namespace SimpleModder
{
    public class PatchedFile
    {
        private readonly string _filename;
        private readonly PatchSet _patchset;

        public PatchedFile(string filename, List<RawPatch> raw)
        {
            _filename = filename;
            _patchset = new PatchSet(raw);
        }

        private async Task<byte[]> ReadFileContents(string programPath)
        {
            var stream = File.Open(Path.Combine(programPath, _filename), FileMode.Open, FileAccess.Read);
            var data = new byte[stream.Length];
            Trace.Assert(await stream.ReadAsync(data, 0, data.Length) == data.Length);
            stream.Close();
            return data;
        }

        public async Task DryRunOn(string programPath)
        {
            Logger.Log($"处理文件：{_filename}");
            var data = await ReadFileContents(programPath);

            _patchset.RunOn(data);
        }

        private void MakeBackupIfNeeded(string programPath)
        {
            var backupFile = Path.Combine(programPath, _filename + ".bak");
            if (File.Exists(backupFile) || Directory.Exists(backupFile))
            {
                Logger.Log($"【警告】文件 {_filename}.bak 已存在，将不创建备份");
            }
            else
            {
                Logger.Log($"创建备份文件 {_filename}.bak");
                var originalFile = Path.Combine(programPath, _filename);
                File.Move(originalFile, backupFile);
            }
        }

        public async Task RunOn(string programPath)
        {
            Logger.Log($"处理文件：{_filename}");
            var data = await ReadFileContents(programPath);
            MakeBackupIfNeeded(programPath);

            data = _patchset.RunOn(data);

            var stream = File.Open(Path.Combine(programPath, _filename), FileMode.Create, FileAccess.Write);
            await stream.WriteAsync(data, 0, data.Length);
            stream.Close();
        }
    }
}
