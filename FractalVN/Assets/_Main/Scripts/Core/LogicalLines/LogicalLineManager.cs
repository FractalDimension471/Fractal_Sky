using System;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.VisualScripting;

namespace DIALOGUE.LogicalLine
{
    public class LogicalLineManager
    {
        #region 属性/Property
        private DialogueSystem DialogueSystem => DialogueSystem.Instance;
        private List<ILogicalLine> LogicalLines { get; } = new();
        #endregion
        #region 方法/Method
        public LogicalLineManager() => LoadLogicLines();
        public bool TryGetLogic(DialogueLine dialogueLine, out Coroutine logic)
        {
            foreach (var logicalLine in LogicalLines)
            {   
                //判断是否匹配，匹配则执行
                if (logicalLine.Matches(dialogueLine))
                {
                    logic = DialogueSystem.StartCoroutine(logicalLine.Excute(dialogueLine));
                    return true;
                }
            }
            logic = null;
            return false;
        }
        private void LoadLogicLines()
        {
            //获取所有实现ILogicalLine接口的非接口类型（确保能实现功能）
            Assembly assembly = Assembly.GetExecutingAssembly();
            Type[] lineTypes = assembly.GetTypes().Where(t => typeof(ILogicalLine).IsAssignableFrom(t) && !t.IsInterface).ToArray();

            foreach (var lineType in lineTypes)
            {
                //实例化逻辑行类型
                ILogicalLine logicalLine=(ILogicalLine)Activator.CreateInstance(lineType);
                LogicalLines.Add(logicalLine);
            }
        }
        #endregion
    }
}