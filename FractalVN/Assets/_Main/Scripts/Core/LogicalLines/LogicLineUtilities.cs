using DIALOGUE;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

namespace DIALOGUE.LogicalLine
{
    //逻辑行实用程序
    public class LogicLineUtilities
    {
        //封装
        public static class Encapsulation
        {
            #region 属性/Property
            public static char ID_EncapsulationStart { get; } = '{';
            public static char ID_EncapsulationEnd { get; } = '}';
            public struct EncapsulatedData
            {
                public List<string> DialogueLines { get; set; }
                public int StartingIndex { get; set; }
                public int EndingIndex { get; set; }
                public bool IsNull => DialogueLines == null;
            }
            #endregion
            #region 方法/Method
            public static bool IsEncapsulationStart(string line) => line.StartsWith(ID_EncapsulationStart);
            public static bool IsEncapsulationEnd(string line) => line.StartsWith(ID_EncapsulationEnd);
            public static EncapsulatedData RipEncapsulatedData(Conversation conversation, int startIndex, bool ripAllParts = false, int parentStartIndex = 0)
            {
                int encapsulationDepth = 0;

                EncapsulatedData data = new()
                {
                    DialogueLines = new(), 
                    StartingIndex = startIndex + parentStartIndex, 
                    EndingIndex = 0 
                };

                for (int i = startIndex; i < conversation.Count; Interlocked.Increment(ref i))
                {
                    string dialogueLine = conversation.DialogueLines[i].Trim();

                    if (ripAllParts || (encapsulationDepth > 0 && !IsEncapsulationEnd(dialogueLine))) 
                    {
                        data.DialogueLines.Add(dialogueLine);
                    }

                    if (IsEncapsulationStart(dialogueLine))
                    {
                        Interlocked.Increment(ref encapsulationDepth);
                        continue;
                    }
                    if (IsEncapsulationEnd(dialogueLine))
                    {
                        Interlocked.Decrement(ref encapsulationDepth);
                        if (encapsulationDepth == 0)
                        {
                            data.EndingIndex = i + parentStartIndex;
                            break;
                        }
                    }
                }
                return data;
            }
            #endregion
        }
        //正则表达式（封装和匹配用）
        public static class Expressions
        {
            #region 属性/Property
            public static string R_Arithmatic { get; } = @"([-+*/=]=?)";
            public static string R_Operators { get; } = @"^\$\w+\s*(=|\+=|-=|\*=|/=|)\s*";
            public static HashSet<string> Operators { get; } = new() { "=", "+", "-", "*", "/", "+=", "-=", "*=", "/=" };
            #endregion
            #region 属性/Property
            public static object CalculateValue(string[] parts)
            {
                List<string> operatorStrings = new();
                List<string> operandStrings = new();
                List<object> operands = new();
                for (int i = 0; i < parts.Length; i++)
                {
                    string part = parts[i].Trim();
                    if (part == string.Empty)
                    {
                        continue;
                    }
                    if (Operators.Contains(part))
                    {
                        operatorStrings.Add(part);
                    }
                    else
                    {
                        operandStrings.Add(part);
                    }
                }
                foreach(string operandString in operandStrings)
                {
                    operands.Add(ExtractValue(operandString));
                }
                CalculateValue_Add_Sub(operandStrings, operands);
                CalculateValue_Mul_Div(operandStrings, operands);

                return operands[0];
            }
            private static object ExtractValue(string value)
            {
                bool negate = false;
                if (value.StartsWith('!'))
                {
                    negate = true;
                    value = value[1..];
                }
                if (value.StartsWith(VariableStore.ID_Variable))
                {
                    string variableName = value.TrimStart(VariableStore.ID_Variable);
                    if (!VariableStore.HasVariable(variableName))
                    {
                        Debug.LogError($"Variable '{variableName}' dose not exist!");
                        return null;
                    }
                    VariableStore.TryGetVariable(variableName, out var variableValue);
                    if (variableValue is bool boolValue && negate)
                    {
                        return !boolValue;
                    }
                    return variableValue;
                }
                else if (value.StartsWith('\"') && value.EndsWith('\"'))//检测双引号?
                {
                    value = TagManager.Inject(value);
                    return value.Trim('\"');
                }
                else
                {
                    if (bool.TryParse(value, out var boolValue))
                    {
                        return negate ? !boolValue : boolValue;
                    }
                    else if (int.TryParse(value, out var integerValue))
                    {
                        return integerValue;
                    }
                    else if (float.TryParse(value, out var floatValue))
                    {
                        return floatValue;
                    }
                    else if (double.TryParse(value, out var doubleValue))
                    {
                        return doubleValue;
                    }
                    else
                    {
                        value = TagManager.Inject(value);
                        return value;
                    }
                }
            }
            private static void CalculateValue_Add_Sub(List<string> operandStrings, List<object> operands)
            {
                for (int i = 0; i < operandStrings.Count; Interlocked.Increment(ref i))
                {
                    string operandString = operandStrings[i];
                    if (operandString == "+" || operandString == "-")
                    {
                        double L_Operand = Convert.ToDouble(operands[i]);
                        double R_Operand = Convert.ToDouble(operands[i + 1]);
                        if (operandString == "+")
                        {
                            operands[i] = L_Operand + R_Operand;
                        }
                        if (operandString == "-")
                        {
                            operands[i] = L_Operand - R_Operand;
                        }
                        operands.RemoveAt(i + 1);
                        operandStrings.RemoveAt(i);
                        Interlocked.Decrement(ref i);
                    }
                }
            }
            private static void CalculateValue_Mul_Div(List<string> operandStrings, List<object> operands)
            {
                for (int i = 0; i < operandStrings.Count; Interlocked.Increment(ref i))
                {
                    string operandString = operandStrings[i];
                    if (operandString == "*" || operandString == "/")
                    {
                        double L_Operand = Convert.ToDouble(operands[i]);
                        double R_Operand = Convert.ToDouble(operands[i + 1]);
                        if(operandString == "*")
                        {
                            operands[i] = L_Operand * R_Operand;
                        }
                        if(operandString == "/")
                        {
                            if (R_Operand == 0d)
                            {
                                Debug.LogError("Variable calculation error: Cannot divide by zero!");
                                return;
                            }
                            operands[i] = L_Operand / R_Operand;
                        }
                        operands.RemoveAt(i + 1);
                        operandStrings.RemoveAt(i);
                        Interlocked.Decrement(ref i);
                    }
                }
            }
            #endregion
        }
       //条件运算符
        public static class Conditions
        {
            #region 属性/Property
            public static string R_ConditionalOperators { get; } = @"(==|!=|<=|>=|<|>|&&|\|\|)";

            private delegate bool OperatorFunction<T>(T L_data, T R_data);
            private static Dictionary<string, OperatorFunction<bool>> BoolOperators { get; } = new()
            {
                {"&&", (L_data, R_data) => L_data && R_data },
                {"||", (L_data, R_data) => L_data || R_data },
                {"==", (L_data, R_data) => L_data == R_data },
                {"!=", (L_data, R_data) => L_data != R_data }
            };
            private static Dictionary<string, OperatorFunction<int>> IntOperators { get; } = new()
            {
                {"==", (L_data, R_data) => L_data == R_data },
                {"!=", (L_data, R_data) => L_data != R_data },
                {">", (L_data, R_data) => L_data > R_data },
                {"<", (L_data, R_data) => L_data < R_data },
                {">=", (L_data, R_data) => L_data >= R_data },
                {"<=", (L_data, R_data) => L_data <= R_data },
            };
            private static Dictionary<string, OperatorFunction<float>> FloatOperators { get; } = new()
            {
                {"==", (L_data, R_data) => L_data == R_data },
                {"!=", (L_data, R_data) => L_data != R_data },
                {">", (L_data, R_data) => L_data > R_data },
                {"<", (L_data, R_data) => L_data < R_data },
                {">=", (L_data, R_data) => L_data >= R_data },
                {"<=", (L_data, R_data) => L_data <= R_data },
            };
            private static Dictionary<string, OperatorFunction<double>> DoubleOperators { get; } = new()
            {
                {"==", (L_data, R_data) => L_data == R_data },
                {"!=", (L_data, R_data) => L_data != R_data },
                {">", (L_data, R_data) => L_data > R_data },
                {"<", (L_data, R_data) => L_data < R_data },
                {">=", (L_data, R_data) => L_data >= R_data },
                {"<=", (L_data, R_data) => L_data <= R_data },
            };
            #endregion
            #region 属性/Property
            public static bool EvaluateCondition(string condition)
            {
                condition = TagManager.Inject(condition);
                string[] parts = Regex.Split(condition, R_ConditionalOperators, RegexOptions.IgnorePatternWhitespace);
                for (int i = 0; i < parts.Length; i++)
                {
                    //针对字符串类型
                    if (parts[i].Trim().StartsWith("\"")&& parts[i].Trim().EndsWith("\""))
                    {
                        parts[i] = parts[i].Trim('\"');
                    }
                }
                if(parts.Length == 1)
                {
                    if(bool.TryParse(parts[0].Trim(), out bool result))
                    {
                        return result;
                    }
                    else
                    {
                        Debug.LogError($"Cannot parse condition: {condition}");
                        return false;
                    }
                }
                else if(parts.Length == 3)
                {
                    return EvaluateExpression(parts[0].Trim(), parts[1].Trim(), parts[2].Trim());
                }
                else
                {
                    Debug.LogError($"Unsupported condition: {condition}");
                    return false;
                }
            }
            private static bool EvaluateExpression(string L_data, string op, string R_data)
            {
                if(bool.TryParse(L_data, out bool L_Bool) && bool.TryParse(R_data,out bool R_Bool))
                {
                    return BoolOperators[op](L_Bool, R_Bool);
                }
                if (int.TryParse(L_data, out int L_Int) && int.TryParse(R_data, out int R_Int))
                {
                    return IntOperators[op](L_Int, R_Int);
                }
                if (float.TryParse(L_data, out float L_Float) && float.TryParse(R_data, out float R_Float))
                {
                    return FloatOperators[op](L_Float, R_Float);
                }
                if (double.TryParse(L_data, out double L_Double) && double.TryParse(R_data, out double R_Double))
                {
                    return DoubleOperators[op](L_Double, R_Double);
                }


                return op switch
                {
                    "==" => L_data == R_data,
                    "!=" => L_data != R_data,
                    _ => throw new InvalidOperationException($"Unsupported operation: {op}"),
                };
            }
            #endregion
        }
    }
}