using System;

namespace GALGAME
{
    [Serializable]
    public class CompressedData
    {
        public string FileName;
        public int StartIndex;
        public int EndIndex;
        public int Progress;
    }
}