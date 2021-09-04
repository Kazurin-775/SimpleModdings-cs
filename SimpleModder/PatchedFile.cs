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
        private readonly FileSearchCond _search;
        private string _realPath;

        public PatchedFile(string filename, List<RawPatch> raw, PatchScript context)
        {
            _filename = filename;
            _patchset = new PatchSet(raw, context.Patchsets);
            if (context.Search.ContainsKey(_filename))
                _search = context.Search[_filename];
        }

        private async Task<byte[]> ReadFileContents()
        {
            var stream = File.Open(_realPath, FileMode.Open, FileAccess.Read);
            var data = new byte[stream.Length];
            Trace.Assert(await stream.ReadAsync(data, 0, data.Length) == data.Length);
            stream.Close();
            return data;
        }

        private bool SearchFile(string programPath)
        {
            if (_search != null)
                _realPath = _search.Search(programPath);
            else
                _realPath = Path.Combine(programPath, _filename);
            return _realPath != null;
        }

        public async Task DryRunOn(string programPath)
        {
            Logger.Log($"处理文件：{_filename}");
            if (!SearchFile(programPath))
                return;

            var data = await ReadFileContents();

            _patchset.RunOn(data);
        }

        private void MakeBackupIfNeeded()
        {
            var backupFile = _realPath + ".bak";
            if (File.Exists(backupFile) || Directory.Exists(backupFile))
            {
                Logger.Log($"【警告】文件 {_filename}.bak 已存在，将不创建备份");
            }
            else
            {
                Logger.Log($"创建备份文件 {_filename}.bak");
                var originalFile = _realPath;
                File.Move(originalFile, backupFile);
            }
        }

        public async Task RunOn(string programPath)
        {
            Logger.Log($"处理文件：{_filename}");
            if (!SearchFile(programPath))
                return;

            var data = await ReadFileContents();
            MakeBackupIfNeeded();

            data = _patchset.RunOn(data);

            var stream = File.Open(_realPath, FileMode.Create, FileAccess.Write);
            await stream.WriteAsync(data, 0, data.Length);
            stream.Close();
        }
    }
}
