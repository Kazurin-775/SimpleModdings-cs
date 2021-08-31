using System;
using System.Linq;

namespace SimpleModder.Patches
{
    public class BytesPatch : Patch
    {
        private readonly BinarySearchPattern _original;
        private readonly BinaryReplacePattern _replaced;
        private readonly int _occurrences = 1;

        public BytesPatch(RawPatch raw)
        {
            if (raw.Kind != "bytes")
                throw new ArgumentException();
            _original = new BinarySearchPattern(raw.Original);
            _replaced = new BinaryReplacePattern(raw.Replaced);
            Comments = raw.Comments;
        }

        public override byte[] RunOn(byte[] data)
        {
            Logger.Log($"  应用补丁：{Comments}");
            var matches = _original.Matches(data);
            var matchesToString = string.Join("，", matches.Select(x => "0x" + x.ToString("X")));
            Logger.Log($"    找到 {matches.Count} 处匹配：{matchesToString}");
            if (matches.Count == _occurrences)
            {
                matches.ForEach(i => _replaced.WriteAt(data, i));
            }
            else
            {
                Logger.Log("    【警告】匹配数量过多或过少，将跳过该补丁");
            }

            return data;
        }
    }
}
