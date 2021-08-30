using System.Diagnostics;

namespace SimpleModder.Patches
{
    public class BinarySearchPattern
    {
        struct PatternByte
        {
            public bool Occupied;
            public byte Value;

            public PatternByte(char ch0, char ch1)
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

        private PatternByte[] _data;

        public BinarySearchPattern(string pattern)
        {
            pattern = pattern.Replace(" ", "");
            Trace.Assert(pattern.Length % 2 == 0);
            _data = new PatternByte[pattern.Length / 2];
            for (int i = 0; i < pattern.Length / 2; i++)
            {
                _data[i] = new PatternByte(pattern[i * 2], pattern[i * 2 + 1]);
            }
        }
    }
}
