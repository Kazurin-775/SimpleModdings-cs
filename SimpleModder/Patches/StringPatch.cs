using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleModder.Patches
{
    public class StringPatch : Patch
    {
        private readonly string _original;
        private readonly string _replaced;
        private readonly int _occurrences;

        public StringPatch(RawPatch raw)
        {
            if (raw.Kind != "string")
                throw new ArgumentException();
            _original = raw.Original;
            _replaced = raw.Replaced;
            _occurrences = raw.Occurrences;
            Comments = raw.Comments;
        }

        private List<int> Matches(string haystack)
        {
            var result = new List<int>();
            int lastIndex = 0;
            while (true)
            {
                int current = haystack.IndexOf(_original, lastIndex, StringComparison.Ordinal);
                if (current >= 0)
                {
                    result.Add(current);
                    lastIndex = current + _original.Length;
                }
                else
                {
                    break;
                }
            }

            return result;
        }

        public override byte[] RunOn(byte[] data)
        {
            Logger.Log($"  应用补丁：{Comments}");

            var content = Encoding.UTF8.GetString(data);
            var matches = Matches(content);
            var matchesToString = string.Join("，", matches.Select(x => "0x" + x.ToString("X")));
            Logger.Log($"    找到 {matches.Count} 处匹配：{matchesToString}");
            if (matches.Count == _occurrences)
            {
                content = content.Replace(_original, _replaced);
                return Encoding.UTF8.GetBytes(content);
            }
            else
            {
                Logger.Log("    【警告】匹配数量过多或过少，将跳过该补丁");
                return data;
            }
        }
    }
}
