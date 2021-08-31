using System.Collections.Generic;
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

        private PatternByte[] _pattern;

        public BinarySearchPattern(string pattern)
        {
            pattern = pattern.Replace(" ", "");
            Trace.Assert(pattern.Length % 2 == 0);
            _pattern = new PatternByte[pattern.Length / 2];
            for (int i = 0; i < pattern.Length / 2; i++)
            {
                _pattern[i] = new PatternByte(pattern[i * 2], pattern[i * 2 + 1]);
            }
        }

        public List<int> Matches(byte[] data)
        {
            var result = new List<int>();
            // brute force
            for (int i = 0; i < data.Length - _pattern.Length; i++)
            {
                bool match = true;
                for (int j = 0; j < _pattern.Length; j++)
                {
                    if (data[i + j] != _pattern[j].Value && _pattern[j].Occupied)
                    {
                        match = false;
                        break;
                    }
                }

                if (match)
                    result.Add(i);
            }

            return result;
        }
    }
}
