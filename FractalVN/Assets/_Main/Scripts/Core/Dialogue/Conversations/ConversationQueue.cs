using DIALOGUE.LogicalLine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DIALOGUE
{
    public class ConversationQueue
    {
        #region  Ù–‘/Property
        public Queue<Conversation> CurrentQueue { get; private set; }
        public Conversation Top => CurrentQueue.Peek();
        public bool IsEmpty => CurrentQueue.Count == 0;
        #endregion
        #region ∑Ω∑®/Method
        public ConversationQueue()
        {
            CurrentQueue = new();
        }
        public void Clear() => CurrentQueue.Clear();
        public void Enqueue(Conversation conversation) => CurrentQueue.Enqueue(conversation);
        public void Dequeue()
        {
            if(CurrentQueue.Count > 0)
            {
                CurrentQueue.Dequeue();
            }
        }
        public void EnqueuePriority(Conversation conversation)
        {
            Queue<Conversation> tmpQueue = new();
            tmpQueue.Enqueue(conversation);
            while (CurrentQueue.Count > 0)
            {
                tmpQueue.Enqueue(CurrentQueue.Dequeue());
            }
            CurrentQueue = tmpQueue;
        }
        public Conversation[] GetQueue() => CurrentQueue.ToArray();
        #endregion
    }
}