using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using UnityEngine;
using static DIALOGUE.LogicalLine.LogicLineUtilities.Expressions;

namespace DIALOGUE.LogicalLine
{
    public class OperatorLogic : ILogicalLine
    {
        public string KeyWord => throw new System.NotImplementedException();
        public bool Matches(DialogueLine line)
        {
            Match match = Regex.Match(line.RawData, R_Operators, RegexOptions.IgnorePatternWhitespace);
            return match.Success;
        }
        public IEnumerator Excute(DialogueLine dialogueLine)
        {
            string rawData = dialogueLine.RawData.Trim();
            string[] parts = Regex.Split(rawData, R_Arithmatic, RegexOptions.IgnorePatternWhitespace);
            if (parts.Length < 3)
            {
                Debug.LogError($"Invalid command: {rawData}");
                yield break;
            }
            string variableName = parts[0].Trim().TrimStart(VariableStore.ID_Variable);
            string op = parts[1].Trim();
            string[] remainingParts = new string[parts.Length - 2];

            Array.Copy(parts, 2, remainingParts, 0, parts.Length - 2);

            object value = CalculateValue(remainingParts);
            if (value == null)
            {
                yield break;
            }
            ProcessOperator(variableName, op, value);
        }
        private void ProcessOperator(string variableName,string op, object value)
        {
            if (VariableStore.TryGetVariable(variableName, out var currentValue))
            {
                ProcessOperatorOnVariable(variableName, op, value, currentValue);
            }
            else if (op == "=") 
            {
                VariableStore.TryCreateVariable(variableName, value);
            }
        }
        private void ProcessOperatorOnVariable(string variableName, string op, object value, object currentValue)
        {
            switch(op)
            {
                case "=":
                    VariableStore.TrySetVariable(variableName, value);
                    break;
                case "+=":
                    VariableStore.TrySetVariable(variableName, ConcatenateOrAdd(value, currentValue));
                    break;
                case "-=":
                    VariableStore.TrySetVariable(variableName, Convert.ToDouble(currentValue) - Convert.ToDouble(value));
                    break;
                case "*=":
                    VariableStore.TrySetVariable(variableName, Convert.ToDouble(currentValue) * Convert.ToDouble(value));
                    break;
                case "/=":
                    VariableStore.TrySetVariable(variableName, OnDividedZero(value, currentValue));
                    break;
                default:
                    Debug.LogError($"Invalid operator:{op}");
                    break;

            }
        }
        private object ConcatenateOrAdd(object value,object currentValue)
        {
            if(value is string)
            {
                return currentValue.ToString() + value;
            }
            return Convert.ToDouble(currentValue)+ Convert.ToDouble(value);
        }
        private object OnDividedZero(object value, object currentValue)
        {
            double l = Convert.ToDouble(currentValue);
            double r = Convert.ToDouble(value);
            if (r == 0d)
            {
                Debug.LogError("Variable calculation error: Cannot divide by zero!");
                return currentValue;
            }
            return l / r;
        }
        
    }
}