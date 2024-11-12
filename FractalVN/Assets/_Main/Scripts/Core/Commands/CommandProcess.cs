using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace COMMANDS
{
    public class CommandProcess
    {
        
        #region ����/Property
        //����ȫ��Ψһ��ʶ�����ڶ���ÿ�����̵�ʶ��
        public Guid ID { get; }
        public string ProcessName {  get; }
        public string[] Args { get; }
        public Delegate Command { get; }
        public CoroutineWrapper RunningProcess { get; set; }
        public UnityEvent EndingAction { get; set; }
        #endregion
        #region ����/Method
        public CommandProcess(Guid id, string processName, Delegate command, string[] args, CoroutineWrapper runningProcess, UnityEvent endingAction = null)
        {
            ID = id;
            ProcessName = processName;
            Args = args;
            Command = command;
            RunningProcess = runningProcess;
            EndingAction = endingAction;
        }
        #endregion
    }
}