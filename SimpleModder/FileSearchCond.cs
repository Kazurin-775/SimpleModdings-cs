using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace SimpleModder
{
    public class FileSearchCond
    {
        private readonly string _filename;
        private readonly Regex _regex;

        public FileSearchCond(string filename, RawSearchCond raw)
        {
            _filename = filename;
            _regex = new Regex(raw.Regex);
        }

        public string Search(string programPath)
        {
            Logger.Log("  正在搜寻该文件");
            string found = null;
            foreach (var filePath in EnumerateFilesInDir(programPath))
            {
                if (!_regex.IsMatch(filePath))
                    continue;
                Logger.Log($"  找到文件：{filePath}");
                if (found != null)
                {
                    Logger.Log($"  【错误】匹配 {_filename} 的文件数量过多");
                    return null;
                }
                else
                {
                    found = filePath;
                }
            }

            if (found == null)
                Logger.Log($"  【错误】找不到文件 {_filename}");
            return found;
        }

        private static IEnumerable<string> EnumerateFilesInDir(string current)
        {
            foreach (var file in Directory.GetFiles(current))
                yield return file;
            foreach (var dir in Directory.GetDirectories(current))
            {
                foreach (var s in EnumerateFilesInDir(dir))
                    yield return s;
            }
        }
    }
}
