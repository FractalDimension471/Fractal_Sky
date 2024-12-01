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
        #region ����/Property
        private DialogueSystem DialogueSystem => DialogueSystem.Instance;
        private List<ILogicalLine> LogicalLines { get; } = new();
        #endregion
        #region ����/Method
        public LogicalLineManager() => LoadLogicLines();
        public bool TryGetLogic(DialogueLine dialogueLine, out Coroutine logic)
        {
            foreach (var logicalLine in LogicalLines)
            {   
                //�ж��Ƿ�ƥ�䣬ƥ����ִ��
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
            //��ȡ����ʵ��ILogicalLine�ӿڵķǽӿ����ͣ�ȷ����ʵ�ֹ��ܣ�
            Assembly assembly = Assembly.GetExecutingAssembly();
            Type[] lineTypes = assembly.GetTypes().Where(t => typeof(ILogicalLine).IsAssignableFrom(t) && !t.IsInterface).ToArray();

            foreach (var lineType in lineTypes)
            {
                //ʵ�����߼�������
                ILogicalLine logicalLine=(ILogicalLine)Activator.CreateInstance(lineType);
                LogicalLines.Add(logicalLine);
            }
        }
        #endregion
    }
}