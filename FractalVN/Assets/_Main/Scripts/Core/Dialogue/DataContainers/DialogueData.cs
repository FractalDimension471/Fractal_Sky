using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace DIALOGUE
{
    /// <summary>
    /// 对话数据
    /// </summary>
    public class DialogueData
    {

        #region 属性/Property
        public bool HasDialogue => Segments.Count > 0;
        public string RawData { get; }
        //定义段落标识符的范式（正则表达式）
        private static string ID_SegmentIdentifierPattern { get; } = @"\{[ca]\}|\{w[ca]\s\d*\.?\d*\}";
        //extra:@用于标识纯字符的字符串，$用于标识引入变量的字符串
        //定义对话段落结构体用于存储对话
        public struct DialogueSegment
        {
            public string Dialogue { get; set; }

            public enum StartSignalTypes { NONE, C, A, WC, WA }
            //定义标识开始信号
            public StartSignalTypes StartSignal { get; set; }
            //定义开始信号延迟操作
            public float SignalDelay { get; set; }
            //定义是否存在信号并对添加和更新两种文本构建方式进行识别
            public bool AppendText => StartSignal == StartSignalTypes.A || StartSignal == StartSignalTypes.WA;
        }
        //定义段落集合用于存储对话段落
        public List<DialogueSegment> Segments { get; }
        #endregion
        #region 方法/Method
        /// <summary>
        /// 构建对话数据
        /// </summary>
        /// <param name="rawDialogue"></param>
        public DialogueData(string rawDialogue)
        {
            RawData = rawDialogue;
            Segments = RipSegments(rawDialogue);
        }
        //解析对话段落（匹配）
        private List<DialogueSegment> RipSegments(string rawDialogue)
        {
            List<DialogueSegment> segments = new();
            //使用标识符范式匹配段落
            MatchCollection matches = Regex.Matches(rawDialogue, ID_SegmentIdentifierPattern);
            DialogueSegment segment = new()
            {
                Dialogue = matches.Count == 0 ? rawDialogue : rawDialogue[..matches[0].Index],
                StartSignal = DialogueSegment.StartSignalTypes.NONE,
                SignalDelay = 0
            };
            segments.Add(segment);
            //尾部索引
            int lastIndex;
            //判别是否存在标识符
            if (matches.Count == 0)
            {
                return segments;
            }
            else
            {
                lastIndex = matches[0].Index;
            }
            //开始匹配
            for (int t = 0; t < matches.Count; t++)
            {
                Match match = matches[t];
                segment = new DialogueSegment();

                //获取段落起始信号
                string signalMatch = match.Value;
                signalMatch = signalMatch.Substring(1, match.Length - 2);
                //使用空格作为分割
                string[] signalSplit = signalMatch.Split(' ');
                //使用空格分割标识符后，解析分割后的标识符并强制转换为段落的起始信号
                segment.StartSignal = (DialogueSegment.StartSignalTypes)Enum.Parse(typeof(DialogueSegment.StartSignalTypes), signalSplit[0].ToUpper());

                //获取信号延迟
                if (signalSplit.Length > 1)
                {
                    //从第二个空格开始解析
                    if (float.TryParse(signalSplit[1], out float delay))
                    {
                        segment.SignalDelay = delay;
                    }
                    else
                    {
                        Debug.LogWarning($"Cannot parse '{signalSplit[1]}'");
                    }
                }
                //获取段落的对话
                int nextIndex = t + 1 < matches.Count ? matches[t + 1].Index : rawDialogue.Length;
                segment.Dialogue = rawDialogue[(lastIndex + match.Length)..nextIndex];
                lastIndex = nextIndex;
                segments.Add(segment);
            }
            return segments;

        }
        #endregion

    }
}