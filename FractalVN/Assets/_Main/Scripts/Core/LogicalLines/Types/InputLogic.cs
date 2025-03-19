using System.Collections;

namespace DIALOGUE.LogicalLine
{
    public class InputLogic : ILogicalLine
    {
        public string KeyWord => "input";
        public bool Matches(DialogueLine dialogueLine)
        {
            return (dialogueLine.HasSpeaker && dialogueLine.SpeakerData.Name.ToLower() == KeyWord);
        }
        public IEnumerator Excute(DialogueLine dialogueLine)
        {
            InputPanel inputPanel = InputPanel.Instance;
            inputPanel.Show(dialogueLine.DialogueData.RawData);
            while (inputPanel.IsWaitingOnUserInput)
            {
                yield return null;
            }
        }
    }
}