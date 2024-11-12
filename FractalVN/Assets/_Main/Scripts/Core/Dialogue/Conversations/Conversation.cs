using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace DIALOGUE
{
    public class Conversation
    {
        #region  Ù–‘/Property
        public List<string> DialogueLines { get; }
        public int Progress { get; internal set; }
        public int FileStartIndex {  get; private set; }
        public int FileEndIndex {  get; private set; }
        public int Count => DialogueLines.Count;
        public string CurrentDialogueLine => DialogueLines[Progress];
        public string File { get; private set; }
        public bool HasReachedEnd => Progress >= DialogueLines.Count;

        #endregion
        #region ∑Ω∑®/Method
        public Conversation(List<string> lines, int progress = 0, string file = "", int fileStartIndex = -1, int fileEndIndex = -1)
        {
            DialogueLines = lines;
            Progress = progress;
            File = file;
            if (fileStartIndex == -1)
            {
                fileStartIndex = 0;
            }
            if (fileEndIndex == -1) 
            {
                fileEndIndex = lines.Count - 1;
            }
            FileStartIndex = fileStartIndex;
            FileEndIndex = fileEndIndex;
        }
        public void IncrementProgress()
        {
            int progress = Progress;
            Interlocked.Increment(ref progress);
            Progress = progress;
        }
        #endregion
    }
}