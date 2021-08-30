using System;

namespace SimpleModder.Patches
{
    public class BytesPatch : Patch
    {
        private readonly BinarySearchPattern _original;
        private readonly BinaryReplacePattern _replaced;

        public BytesPatch(RawPatch raw)
        {
            if (raw.Kind != "bytes")
                throw new ArgumentException();
            _original = new BinarySearchPattern(raw.Original);
            _replaced = new BinaryReplacePattern(raw.Replaced);
            Comments = raw.Comments;
        }
    }
}
