using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace COMMANDS
{
    /// <summary>
    /// ָ�����ݿ�
    /// </summary>
    public class CommandDatabase
    {
        #region ����/Property
        //�����ֵ��������ʽ����Ӧί��
        private Dictionary<string, Delegate> Database { get; } = new();
        public bool HasCommand(string commandName) => Database.ContainsKey(commandName.ToLower());
        #endregion
        #region ����/Method
        /// <summary>
        /// ���ָ��
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
        /// ��ȡָ��
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