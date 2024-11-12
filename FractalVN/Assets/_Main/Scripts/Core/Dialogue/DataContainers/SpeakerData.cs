using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using UnityEngine;
/// <summary>
/// ˵��������
/// </summary>
public class SpeakerData
{
    #region ����/Property
    public bool MakeCharacterEnter { get; private set; } = false;
    public bool IsCastingName => CastName != string.Empty;
    public bool IsCastingPosition { get; private set; } = false;
    public bool IsCastingExpressions => CastExpression.Count > 0;
    public string Name { get; } = "";
    private string CastName { get; } = "";
    public string RawData { get; } = "";

    private static string ID_ToCastName { get; } = " as ";
    private static string ID_SetPosition { get; } = " at ";
    private static string ID_StarterOfExpressions { get; } = " [";
    private static string ID_Enter { get; } = "enter ";
    private static char ID_JoinExpressions { get; } = ',';
    private static char ID_DefineExpression { get; } = ':';

    public string DisplayName => (IsCastingName ? CastName : Name);
    public Vector2 CastPosition { get; private set; }
    public List<(int layer, string expression)> CastExpression { get; }
    #endregion
    #region ����/Method
    /// <summary>
    /// ����˵��������
    /// </summary>
    /// <param name="rawSpeaker"></param>
    public SpeakerData(string rawSpeaker)
    {
        RawData = rawSpeaker;
        rawSpeaker = ProcessKeyWords(rawSpeaker);
        string pattern = @$"{ID_ToCastName}|{ID_SetPosition}|{ID_StarterOfExpressions.Insert(ID_StarterOfExpressions.Length - 1, @"\")}";
        MatchCollection matches = Regex.Matches(rawSpeaker, pattern);
        CastPosition = Vector2.zero;
        CastExpression = new List<(int layer, string expression)>();
        //��ƥ��ʱ��ֱ�ӻ�ȡ��ɫ����
        if (matches.Count == 0)
        {
            Name = rawSpeaker;

            return;
        }
        //��ƥ���ʱ��ȡ��һ��ƥ��ǰ���ַ�����Ϊ����
        int index = matches[0].Index;
        Name = rawSpeaker[..index];
        //��ʼƥ��
        for (int t = 0; t < matches.Count; t++)
        {
            Match match = matches[t];
            int startIndex = 0, endIndex = 0;
            if (match.Value == ID_ToCastName)
            {
                startIndex = match.Index + ID_ToCastName.Length;
                endIndex = (t < matches.Count - 1) ? matches[t + 1].Index : rawSpeaker.Length;
                CastName = rawSpeaker[startIndex..endIndex];
            }
            else if (match.Value == ID_SetPosition)
            {
                IsCastingPosition = true;
                startIndex = match.Index + ID_SetPosition.Length;
                endIndex = (t < matches.Count - 1) ? matches[t + 1].Index : rawSpeaker.Length;

                string castPos = rawSpeaker[startIndex..endIndex];
                //������Ļ��
                string[] axis = castPos.Split(ID_DefineExpression, System.StringSplitOptions.RemoveEmptyEntries);

                Vector2 cachePos = CastPosition;
                if(float.TryParse(axis[0], out float x))
                {
                    cachePos.x = x;
                };//X��
                if (axis.Length > 1)
                {
                    if(float.TryParse(axis[1], out float y))
                    {
                        cachePos.y = y;
                    };//Y��
                }
                CastPosition = cachePos;
            }
            else if (match.Value == ID_StarterOfExpressions)
            {
                startIndex = match.Index + ID_StarterOfExpressions.Length;//debug:"\["�ĳ��ȿ��ܲ�Ϊ2
                endIndex = (t < matches.Count - 1) ? matches[t + 1].Index : rawSpeaker.Length;

                string castExp = rawSpeaker.Substring(startIndex, endIndex - (startIndex + 1));

                CastExpression = castExp.Split(ID_JoinExpressions)
                    .Select(x =>
                    {
                        var parts = x.Trim().Split(ID_DefineExpression);
                        return (int.Parse(parts[0]), parts[1]);
                    }).ToList();
            }
        }
    }
    private string ProcessKeyWords(string rawSpeaker)
    {
        if (rawSpeaker.StartsWith(ID_Enter))
        {
            //���ȥ���ؼ��ʵ��Ӿ�
            rawSpeaker = rawSpeaker[ID_Enter.Length..];
            MakeCharacterEnter = true;
        }
        return rawSpeaker;
    }
    #endregion
}
