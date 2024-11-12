using COMMANDS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DIALOGUE
{
    /// <summary>
    /// �Ի���
    /// </summary>
    public class DialogueLine
    {

        #region ����/Property
        //��������ֶ�
        public SpeakerData SpeakerData { get; }
        public DialogueData DialogueData { get; }
        public CommandData CommandsData { get; }
        //����ȷ���Ƿ����speaker��dialogue��commands
        public bool HasSpeaker => SpeakerData != null;//speaker != string.Empty;
        public bool HasDialogue => DialogueData != null;//dialogue != string.Empty;
        public bool HasCommands => CommandsData != null;//commands != string.Empty;
        public string RawData { get; } = string.Empty;
        #endregion
        #region ����/Method
        /// <summary>
        /// �����Ի���
        /// </summary>
        /// <param name="speaker"></param>
        /// <param name="dialogue"></param>
        /// <param name="commands"></param>
        public DialogueLine(string rawLine,string speaker, string dialogue, string commands)
        {
            RawData = rawLine;
            SpeakerData = (string.IsNullOrWhiteSpace(speaker) ? null : new SpeakerData(speaker));
            DialogueData = (string.IsNullOrWhiteSpace(dialogue) ? null : new DialogueData(dialogue));
            CommandsData = (string.IsNullOrWhiteSpace(commands) ? null : new CommandData(commands));
        }
        #endregion
    }
}

