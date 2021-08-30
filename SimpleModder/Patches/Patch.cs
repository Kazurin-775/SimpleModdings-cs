using System;

namespace SimpleModder.Patches
{
    public abstract class Patch
    {
        public string Comments { get; protected set; }

        public static Patch Compile(RawPatch raw)
        {
            switch (raw.Kind)
            {
                case "bytes":
                    return new BytesPatch(raw);
                default:
                    throw new ArgumentException($"无效补丁类型：{raw.Kind}");
            }
        }
    }
}
