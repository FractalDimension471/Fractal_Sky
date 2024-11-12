using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using UnityEngine;
/// <summary>
/// 说话者数据
/// </summary>
public class SpeakerData
{
    #region 属性/Property
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
    #region 方法/Method
    /// <summary>
    /// 构建说话者数据
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
        //无匹配时，直接获取角色名字
        if (matches.Count == 0)
        {
            Name = rawSpeaker;

            return;
        }
        //有匹配的时获取第一个匹配前的字符串作为名字
        int index = matches[0].Index;
        Name = rawSpeaker[..index];
        //开始匹配
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
                //定义屏幕轴
                string[] axis = castPos.Split(ID_DefineExpression, System.StringSplitOptions.RemoveEmptyEntries);

                Vector2 cachePos = CastPosition;
                if(float.TryParse(axis[0], out float x))
                {
                    cachePos.x = x;
                };//X轴
                if (axis.Length > 1)
                {
                    if(float.TryParse(axis[1], out float y))
                    {
                        cachePos.y = y;
                    };//Y轴
                }
                CastPosition = cachePos;
            }
            else if (match.Value == ID_StarterOfExpressions)
            {
                startIndex = match.Index + ID_StarterOfExpressions.Length;//debug:"\["的长度可能不为2
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
            //获得去除关键词的子句
            rawSpeaker = rawSpeaker[ID_Enter.Length..];
            MakeCharacterEnter = true;
        }
        return rawSpeaker;
    }
    #endregion
}
