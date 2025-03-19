using System.Collections;
using static DIALOGUE.LogicalLine.LogicLineUtilities.Conditions;
using static DIALOGUE.LogicalLine.LogicLineUtilities.Encapsulation;

namespace DIALOGUE.LogicalLine
{
    public class ConditionLogic : ILogicalLine
    {
        #region 属性/Property
        private static string ID_IF { get; } = "if";
        private static string ID_ELSE { get; } = "else";
        private static char ID_ConditionStart { get; } = '(';
        private static char ID_ConditionEnd { get; } = ')';
        public string KeyWord => ID_IF;
        #endregion
        #region 方法/Method
        public bool Matches(DialogueLine dialogueLine)
        {
            return dialogueLine.RawData.Trim().StartsWith(KeyWord);
        }
        public IEnumerator Excute(DialogueLine dialogueLine)
        {
            string rawCondition = ExtractCondition(dialogueLine.RawData.Trim());
            bool conditionResult = EvaluateCondition(rawCondition);

            Conversation currentConversation = DialogueSystem.Instance.ConversationManager.TopConversation;
            int currentProgress = DialogueSystem.Instance.ConversationManager.TopProgress;

            EncapsulatedData ifData = RipEncapsulatedData(currentConversation, currentProgress, parentStartIndex: currentConversation.FileStartIndex);
            EncapsulatedData elseData = new();

            if (ifData.EndingIndex - ifData.StartingIndex + 1 < currentConversation.Count) //检测if内部语句的范围是否覆盖全部对话(即是否有else)
            {
                string nextLine = currentConversation.DialogueLines[ifData.EndingIndex - ifData.StartingIndex + 1].Trim();
                if (nextLine == ID_ELSE)
                {
                    elseData = RipEncapsulatedData(currentConversation, ifData.EndingIndex - ifData.StartingIndex + 1, parentStartIndex: currentConversation.FileStartIndex);
                }
            }
            if (elseData.IsNull)
            {
                currentConversation.Progress = ifData.EndingIndex;
            }
            else
            {
                currentConversation.Progress = elseData.EndingIndex;
            }


            EncapsulatedData conditionData = conditionResult ? ifData : elseData;

            if (!conditionData.IsNull && conditionData.DialogueLines.Count > 0)
            {
                conditionData.StartingIndex += 2;
                conditionData.EndingIndex -= 1;
                Conversation newConversation = new(conditionData.DialogueLines, file: currentConversation.File, fileStartIndex: conditionData.StartingIndex, fileEndIndex: conditionData.EndingIndex);
                DialogueSystem.Instance.ConversationManager.EnqueuePriority(newConversation);
            }
            yield return null;
        }


        private string ExtractCondition(string line)
        {
            int startIndex = line.IndexOf(ID_ConditionStart) + 1;
            int endIndex = line.IndexOf(ID_ConditionEnd);
            return line[startIndex..endIndex].Trim();
        }
        #endregion
    }
}