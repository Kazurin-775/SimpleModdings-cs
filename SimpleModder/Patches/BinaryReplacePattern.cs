using System.Diagnostics;

namespace SimpleModder.Patches
{
    public class BinaryReplacePattern
    {
        struct PatchedByte
        {
            public bool Occupied;
            public byte Value;

            public PatchedByte(char ch0, char ch1)
            {
                if (ch0 == '?' && ch1 == '?')
                {
                    Occupied = false;
                    Value = 0;
                }
                else
                {
                    Occupied = true;
                    Value = byte.Parse($"{ch0}{ch1}", System.Globalization.NumberStyles.HexNumber);
                }
            }
        }

        private PatchedByte[] _data;

        public BinaryReplacePattern(string pattern)
        {
            pattern = pattern.Replace(" ", "");
            Trace.Assert(pattern.Length % 2 == 0);
            _data = new PatchedByte[pattern.Length / 2];
            for (int i = 0; i < pattern.Length / 2; i++)
            {
                _data[i] = new PatchedByte(pattern[i * 2], pattern[i * 2 + 1]);
            }
        }
    }
}
