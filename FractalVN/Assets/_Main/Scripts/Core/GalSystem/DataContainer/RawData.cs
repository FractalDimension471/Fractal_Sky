using DIALOGUE;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GALGAME
{
    [Serializable]
    public class RawData
    {
        public List<string> Conversation = new();
        public int Progress;
    }
}