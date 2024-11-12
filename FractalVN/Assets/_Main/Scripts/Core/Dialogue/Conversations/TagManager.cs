using DIALOGUE.LogicalLine;
using GALGAME;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;
public class TagManager
{
    #region  Ù–‘
    private static Dictionary<string, Func<string>> Tags { get; } = new()
    {
        {"<mainChar>", () => GalSaveFile.ActiveFile.PlayerName},
        {"<time>", () => DateTime.Now.ToString("HH:mm")},
        {"<input>",() => InputPanel.Instance.LastInput}
    };
    private static Regex TagRegex { get; } = new("<\\w+>");
    #endregion
    #region ∑Ω∑®
    public static string Inject(string text, bool injectTags = true, bool injectVariables = true)
    {
        if(injectTags)
        {
            text = InjectTags(text);
        }
        if (injectVariables)
        {
            text = InjectVariables(text);
        }
        return text;
    }
    private static string InjectTags(string text)
    {
        if (TagRegex.IsMatch(text))
        {
            foreach (Match match in TagRegex.Matches(text))
            {
                if (Tags.TryGetValue(match.Value, out var tagValueRequest))
                {
                    text = text.Replace(match.Value, tagValueRequest());
                }
            }
        }
        return text;
    }
    private static string InjectVariables(string value)
    {
        var matches = Regex.Matches(value, VariableStore.R_Variables);
        var matchList = matches.Cast<Match>().ToList();

        for (int i = matchList.Count-1; i >=0; Interlocked.Decrement(ref i))
        {
            var match = matchList[i];
            string variableName = match.Value.TrimStart(VariableStore.ID_Variable, '!');
            bool negate = match.Value.StartsWith("!");
            bool endInIllegalCharacter = variableName.EndsWith(VariableStore.ID_RelationalSeperator);
            if (endInIllegalCharacter)
            {
                variableName = variableName[..^1];
            }
            if (!VariableStore.TryGetVariable(variableName, out var variableValue))
            {
                Debug.LogError($"Variable '{variableName}' cannot be found!");
                continue;
            }
            if(variableValue is bool boolValue && negate)
            {
                variableValue = !boolValue;
            }

            int removeLenth = match.Index + match.Length > value.Length ? value.Length - match.Index : match.Length;
            if (endInIllegalCharacter)
            {
                Interlocked.Decrement(ref removeLenth);
            }
            value = value.Remove(match.Index, removeLenth);
            value = value.Insert(match.Index, variableValue.ToString());
        }
        return value;
    }
    #endregion
}
