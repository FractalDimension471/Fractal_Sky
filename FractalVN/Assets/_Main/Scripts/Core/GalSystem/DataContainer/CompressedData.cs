using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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