using System;
using System.Collections.Generic;

namespace SimpleModder.Patches
{
    public abstract class Patch
    {
        public string Comments { get; protected set; }

        public abstract byte[] RunOn(byte[] data);

        public static Patch Compile(RawPatch raw, Dictionary<string, PatchSet> patchsets)
        {
            switch (raw.Kind)
            {
                case "bytes":
                    return new BytesPatch(raw);
                case "patchset":
                    return patchsets[raw.Name];
                default:
                    throw new ArgumentException($"无效补丁类型：{raw.Kind}");
            }
        }
    }
}
