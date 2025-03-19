using System;
using System.Collections.Generic;

namespace GALGAME
{
    [Serializable]
    public class RawData
    {
        public List<string> Conversation = new();
        public int Progress;
    }
}