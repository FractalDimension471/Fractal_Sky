using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace DIALOGUE
{
    /// <summary>
    /// �Ի�����
    /// </summary>
    public class DialogueData
    {

        #region ����/Property
        public bool HasDialogue => Segments.Count > 0;
        public string RawData { get; }
        //��������ʶ���ķ�ʽ��������ʽ��
        private static string ID_SegmentIdentifierPattern { get; } = @"\{[ca]\}|\{w[ca]\s\d*\.?\d*\}";
        //extra:@���ڱ�ʶ���ַ����ַ�����$���ڱ�ʶ����������ַ���
        //����Ի�����ṹ�����ڴ洢�Ի�
        public struct DialogueSegment
        {
            public string Dialogue { get; set; }

            public enum StartSignalTypes { NONE, C, A, WC, WA }
            //�����ʶ��ʼ�ź�
            public StartSignalTypes StartSignal { get; set; }
            //���忪ʼ�ź��ӳٲ���
            public float SignalDelay { get; set; }
            //�����Ƿ�����źŲ�����Ӻ͸��������ı�������ʽ����ʶ��
            public bool AppendText => StartSignal == StartSignalTypes.A || StartSignal == StartSignalTypes.WA;
        }
        //������伯�����ڴ洢�Ի�����
        public List<DialogueSegment> Segments { get; }
        #endregion
        #region ����/Method
        /// <summary>
        /// �����Ի�����
        /// </summary>
        /// <param name="rawDialogue"></param>
        public DialogueData(string rawDialogue)
        {
            RawData = rawDialogue;
            Segments = RipSegments(rawDialogue);
        }
        //�����Ի����䣨ƥ�䣩
        private List<DialogueSegment> RipSegments(string rawDialogue)
        {
            List<DialogueSegment> segments = new();
            //ʹ�ñ�ʶ����ʽƥ�����
            MatchCollection matches = Regex.Matches(rawDialogue, ID_SegmentIdentifierPattern);
            DialogueSegment segment = new()
            {
                Dialogue = matches.Count == 0 ? rawDialogue : rawDialogue[..matches[0].Index],
                StartSignal = DialogueSegment.StartSignalTypes.NONE,
                SignalDelay = 0
            };
            segments.Add(segment);
            //β������
            int lastIndex;
            //�б��Ƿ���ڱ�ʶ��
            if (matches.Count == 0)
            {
                return segments;
            }
            else
            {
                lastIndex = matches[0].Index;
            }
            //��ʼƥ��
            for (int t = 0; t < matches.Count; t++)
            {
                Match match = matches[t];
                segment = new DialogueSegment();

                //��ȡ������ʼ�ź�
                string signalMatch = match.Value;
                signalMatch = signalMatch.Substring(1, match.Length - 2);
                //ʹ�ÿո���Ϊ�ָ�
                string[] signalSplit = signalMatch.Split(' ');
                //ʹ�ÿո�ָ��ʶ���󣬽����ָ��ı�ʶ����ǿ��ת��Ϊ�������ʼ�ź�
                segment.StartSignal = (DialogueSegment.StartSignalTypes)Enum.Parse(typeof(DialogueSegment.StartSignalTypes), signalSplit[0].ToUpper());

                //��ȡ�ź��ӳ�
                if (signalSplit.Length > 1)
                {
                    //�ӵڶ����ո�ʼ����
                    if (float.TryParse(signalSplit[1], out float delay))
                    {
                        segment.SignalDelay = delay;
                    }
                    else
                    {
                        Debug.LogWarning($"Cannot parse '{signalSplit[1]}'");
                    }
                }
                //��ȡ����ĶԻ�
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