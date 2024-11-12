using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace COMMANDS
{
    public class CommandProcess
    {
        
        #region 属性/Property
        //定义全局唯一标识符用于对于每个进程的识别
        public Guid ID { get; }
        public string ProcessName {  get; }
        public string[] Args { get; }
        public Delegate Command { get; }
        public CoroutineWrapper RunningProcess { get; set; }
        public UnityEvent EndingAction { get; set; }
        #endregion
        #region 方法/Method
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