using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace COMMANDS
{
    /// <summary>
    /// 指令数据库
    /// </summary>
    public class CommandDatabase
    {
        #region 属性/Property
        //定义字典存放命令格式并对应委托
        private Dictionary<string, Delegate> Database { get; } = new();
        public bool HasCommand(string commandName) => Database.ContainsKey(commandName.ToLower());
        #endregion
        #region 方法/Method
        /// <summary>
        /// 添加指令
        /// </summary>
        /// <param name="commandName"></param>
        /// <param name="command"></param>
        public void AddCommand(string commandName, Delegate command)
        {
            if (!HasCommand(commandName))
            {
                Database.Add(commandName, command);
            }
            else
            {
                Debug.LogError($"Command already exist in the database'{commandName}'");
            }
        }
        /// <summary>
        /// 获取指令
        /// </summary>
        /// <param name="commandName"></param>
        /// <returns></returns>
        public Delegate GetCommand(string commandName)
        {
            if (!HasCommand(commandName))
            {
                Debug.LogError($"Command '{commandName}' does not exist in the database!");
                return null;
            }
            return Database[commandName];
        }
        #endregion
    }
}