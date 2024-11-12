using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
namespace DIALOGUE
{
    /// <summary>
    /// �Ի�������
    /// </summary>
    public class DialogueParser
    {
        #region ����/Property
        //�������commands���ı�ﷶʽ
        private static string ID_Dialogue { get; } = @"[\w\[\]]*[^\s]\(";
        private static char ID_DialogueIdentifier { get; } = '@';
        private static char ID_EscapeIdentifier { get; } = '\\';
        #endregion
        #region ����/Method
        /// <summary>
        /// �����Ի�
        /// </summary>
        /// <param name="rawline"></param>
        /// <returns></returns>
        public static DialogueLine Parse(string rawline)
        {
            Debug.Log($"Parsing line-'{rawline}'");
            (string speaker, string dialogue, string commands) = RipContent(rawline);
            Debug.Log($"speaker:'{speaker}'\ndialogue:'{dialogue}'\ncommands:'{commands}'");
            commands = TagManager.Inject(commands);//ָ�����Ͳ����жϣ�ֱ�Ӽ���Ƿ��б�ǩ
            return new DialogueLine(rawline, speaker, dialogue, commands);
        }
        /// <summary>
        /// �ָ�Ի�
        /// </summary>
        /// <param name="rawLine"></param>
        /// <returns></returns>
        private static(string,string,string) RipContent(string rawLine)
        {
            string speaker="", dialogue="", commands="";
            int dialogueStart = -1;
            int dialogueEnd = -1;
            bool isEscaped = false;

            for (int t = 0; t < rawLine.Length; t++) 
            {
                char current = rawLine[t];
                if (current == ID_EscapeIdentifier)
                {
                    isEscaped = !isEscaped;
                }
                //���塰@����Ϊ�ָ���
                else if (current == ID_DialogueIdentifier && !isEscaped)
                {
                    if(dialogueStart==-1)
                    {
                        dialogueStart = t;
                    }
                    else if (dialogueEnd == -1)
                    {
                        dialogueEnd = t;
                        break;
                    }
                }
                else if(current == ID_DialogueIdentifier && isEscaped)
                {
                    isEscaped = false;
                }
                
            }
            //Debug.Log(rawline.Substring(dialogueStart + 1, (dialogueEnd - dialogueStart)-1));
            //����������ʽ������ƥ�䣩
            Regex commandRegex = new(ID_Dialogue);
            MatchCollection matches = commandRegex.Matches(rawLine);
            int commandStart = -1;
            foreach(Match match in matches)
            {
                //ȷ���Ի��ڵ�ָ�����Ч
                if (match.Index < dialogueStart || match.Index > dialogueEnd) 
                {
                    commandStart = match.Index;
                    break;
                }
            }
            //��speaker��dialogue
            if (commandStart != -1 && (dialogueStart == -1 && dialogueEnd == -1))
            {
                return ("", "", rawLine.Trim());
            }
            //���������еĸ��ɷ�
            //��speaker��dialogue��commands
            if (dialogueStart != -1 && dialogueEnd != -1 && (commandStart == -1 || commandStart > dialogueEnd))
            {
                //ȷ��speaker��λ�ò��ü�����
                speaker = rawLine[..dialogueStart].Trim();
                //ȷ��dialogue��λ�ò��ü�����
                dialogue = rawLine.Substring(dialogueStart + 1, dialogueEnd - dialogueStart - 1).Replace("\\\"", "\"");//��dialogue�е� /"��ȷ��ȡ
                //��commands
                if (commandStart != -1)
                {
                    //ȷ��commands��λ�ò��ü�����
                    commands = rawLine[commandStart..].Trim().ToLower();
                }
            }
            else if (commandStart != -1 && dialogueStart > commandStart) 
            {
                commands = rawLine;
            }
            else
            {
                dialogue = rawLine;
            }

            return (speaker, dialogue, commands);
        }
        #endregion
    }
}

