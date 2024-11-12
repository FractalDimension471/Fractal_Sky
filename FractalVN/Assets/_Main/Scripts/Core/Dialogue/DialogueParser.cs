using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
namespace DIALOGUE
{
    /// <summary>
    /// 对话解析器
    /// </summary>
    public class DialogueParser
    {
        #region 属性/Property
        //定义命令（commands）的表达范式
        private static string ID_Dialogue { get; } = @"[\w\[\]]*[^\s]\(";
        private static char ID_DialogueIdentifier { get; } = '@';
        private static char ID_EscapeIdentifier { get; } = '\\';
        #endregion
        #region 方法/Method
        /// <summary>
        /// 解析对话
        /// </summary>
        /// <param name="rawline"></param>
        /// <returns></returns>
        public static DialogueLine Parse(string rawline)
        {
            Debug.Log($"Parsing line-'{rawline}'");
            (string speaker, string dialogue, string commands) = RipContent(rawline);
            Debug.Log($"speaker:'{speaker}'\ndialogue:'{dialogue}'\ncommands:'{commands}'");
            commands = TagManager.Inject(commands);//指令类型不用判断，直接检测是否有标签
            return new DialogueLine(rawline, speaker, dialogue, commands);
        }
        /// <summary>
        /// 分割对话
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
                //定义“@”作为分隔符
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
            //定义正则表达式（用于匹配）
            Regex commandRegex = new(ID_Dialogue);
            MatchCollection matches = commandRegex.Matches(rawLine);
            int commandStart = -1;
            foreach(Match match in matches)
            {
                //确保对话内的指令不会生效
                if (match.Index < dialogueStart || match.Index > dialogueEnd) 
                {
                    commandStart = match.Index;
                    break;
                }
            }
            //无speaker、dialogue
            if (commandStart != -1 && (dialogueStart == -1 && dialogueEnd == -1))
            {
                return ("", "", rawLine.Trim());
            }
            //解析命令中的各成分
            //有speaker、dialogue、commands
            if (dialogueStart != -1 && dialogueEnd != -1 && (commandStart == -1 || commandStart > dialogueEnd))
            {
                //确定speaker的位置并裁剪下来
                speaker = rawLine[..dialogueStart].Trim();
                //确定dialogue的位置并裁剪下来
                dialogue = rawLine.Substring(dialogueStart + 1, dialogueEnd - dialogueStart - 1).Replace("\\\"", "\"");//将dialogue中的 /"正确截取
                //有commands
                if (commandStart != -1)
                {
                    //确定commands的位置并裁剪下来
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

