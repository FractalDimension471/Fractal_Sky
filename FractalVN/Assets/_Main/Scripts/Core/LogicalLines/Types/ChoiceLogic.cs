using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

using static DIALOGUE.LogicalLine.LogicLineUtilities.Encapsulation;

namespace DIALOGUE.LogicalLine
{
    public class ChoiceLogic : ILogicalLine
    {
        #region 属性/Property
        public string KeyWord => "choice";
        public static char ID_Choice { get; } = '-';
        public static char ID_DefaultChoice { get; } = '+';
        public static bool IsWaitingCountDown { get; internal set; } = false;
        private struct Choice
        {
            public string Title { get; set; }
            public List<string> ResultLines {  get; set; }
            public int StartIndex {  get; set; }
            public int EndIndex { get; set; }
        }
        #endregion
        #region 方法/Method
        
        public bool Matches(DialogueLine dialogueLine)
        {
            return dialogueLine.HasSpeaker && dialogueLine.SpeakerData.Name.ToLower() == KeyWord;
        }
        public IEnumerator Excute(DialogueLine dialogueLine)
        {
            ChoicePanel choicePanel = ChoicePanel.Instance;

            Conversation currentConversation = DialogueSystem.Instance.ConversationManager.TopConversation;
            int progress = DialogueSystem.Instance.ConversationManager.TopProgress;

            EncapsulatedData choiceData = RipEncapsulatedData(currentConversation, progress, true, currentConversation.FileStartIndex);
            List<Choice> choices = GetChoiceFromData(choiceData);

            string question = dialogueLine.DialogueData.RawData;
            string[] answers = choices.Select(c => c.Title).ToArray();
            if (IsWaitingCountDown)
            {
                choicePanel.WaitingCountDown = true;
            }
            choicePanel.Show(question, answers);
            while (choicePanel.IsWaitingOnUserMakingChoice)
            {
                if (!IsWaitingCountDown)
                {
                    yield return null;
                }
                else
                {
                    float elapsedTime = 0f;
                    while (elapsedTime < 5f && choicePanel.IsWaitingOnUserMakingChoice)
                    {
                        yield return null; // 每帧等待
                        elapsedTime += Time.deltaTime; // 增加已用时间 }
                    }
                    if (elapsedTime >= 5f && choicePanel.IsWaitingOnUserMakingChoice)
                    {
                        choicePanel.OnChoiceCountDown();
                    }
                }
            }
            Choice selectedChoice = choices[choicePanel.LastDecision.answerIndex];

            Conversation newConversation = new(selectedChoice.ResultLines, file: currentConversation.File, fileStartIndex: selectedChoice.StartIndex, fileEndIndex: selectedChoice.EndIndex);
            DialogueSystem.Instance.ConversationManager.TopConversation.Progress = choiceData.EndingIndex - currentConversation.FileStartIndex + 1;//加一回避选择内容最后的‘}’
            DialogueSystem.Instance.ConversationManager.EnqueuePriority(newConversation);
            
        }
        private bool IsChoiceStart(string line) => line.Trim().StartsWith(ID_Choice);
        private bool HasingDefaultChoice(string line)
        {
            if (line.Trim().StartsWith(ID_DefaultChoice))
            {
                IsWaitingCountDown = true;
                return true;
            }
            return false;
        }
        private List<Choice> GetChoiceFromData(EncapsulatedData data)
        {
            List<Choice> choices = new();
            int encapsulationDepth = 0;
            bool isFirstChoice = true;
            Choice choice = new()
            {
                Title = string.Empty,
                ResultLines = new()
            };
            int choiceIndex = 0;
            int counter;
            for (counter = 1; counter < data.DialogueLines.Count; Interlocked.Increment(ref counter))
            {
                string dialogueLine = data.DialogueLines[counter].Trim();
                if ((IsChoiceStart(dialogueLine) || HasingDefaultChoice(dialogueLine)) && encapsulationDepth == 1)
                {
                    if (!isFirstChoice)
                    {
                        choice.StartIndex = data.StartingIndex + choiceIndex + 1;
                        choice.EndIndex = data.StartingIndex + counter - 1;
                        choices.Add(choice);
                        choice = new()
                        {
                            Title = string.Empty,
                            ResultLines = new()
                        };
                    }
                    choiceIndex = counter;
                    choice.Title = dialogueLine[1..];
                    isFirstChoice = false;
                    continue;
                }
                AddDialogueLineToResult(dialogueLine, ref choice, ref encapsulationDepth);
            }

            if (!choices.Contains(choice))
            {
                choice.StartIndex = data.StartingIndex + choiceIndex + 1;
                choice.EndIndex = data.StartingIndex + counter - 2;
                choices.Add(choice);
            }
            return choices;
        }
        private void AddDialogueLineToResult(string dialogueLine, ref Choice choice, ref int encapsulationDepth)
        {
            if (IsEncapsulationStart(dialogueLine))
            {
                if (encapsulationDepth > 0)
                {
                    choice.ResultLines.Add(dialogueLine);
                }
                Interlocked.Increment(ref encapsulationDepth);
                return;
            }
            if (IsEncapsulationEnd(dialogueLine))
            {
                Interlocked.Decrement(ref encapsulationDepth);
                if (encapsulationDepth > 0)
                {
                    choice.ResultLines.Add(dialogueLine);
                }
                return;
            }
            choice.ResultLines.Add(dialogueLine);
        }

        #endregion
    }
}