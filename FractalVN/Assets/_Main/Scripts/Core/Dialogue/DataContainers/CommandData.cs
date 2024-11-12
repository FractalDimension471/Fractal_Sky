using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace COMMANDS
{
    /// <summary>
    /// 指令数据
    /// </summary>
    public class CommandData
    {
        #region 属性/Property
        public struct Command
        {
            public string Name { get; set; }
            public string[] Arguments {  get; set; }
            public bool WaitForCompletion {  get; set; }
        }
        public List<Command> Commands { get; }

        private static char ID_CommandSpliter { get; } = ',';
        private static char ID_ParameterStarter { get; } = '('; 
        private static char ID_ParameterNameContainer { get; } = '"';
        private static char ID_ParameterSpliter { get; } = ' ';
        private static string ID_WaitCommandExcute { get; } = "[wait]";
        #endregion

        #region 方法/Method
        /// <summary>
        /// 构建指令数据
        /// </summary>
        /// <param name="rawCommands"></param>
        public CommandData(string rawCommands)
        {
            Commands = RipCommands(rawCommands);
        }
        private List<Command> RipCommands(string rawCommands)
        {
            //去除指令空白部分并以逗号分割
            string[] data = rawCommands.Split(ID_CommandSpliter, System.StringSplitOptions.RemoveEmptyEntries);
            List<Command> result = new();
            foreach (string cmd in data)
            {
                Command command = new();
                //使用索引进行匹配
                int cmdIndex = cmd.IndexOf(ID_ParameterStarter);
                //获取指令名
                command.Name = cmd[..cmdIndex].Trim();
                //判断是否等待
                if (command.Name.ToLower().StartsWith(ID_WaitCommandExcute))
                {
                    command.Name = command.Name[ID_WaitCommandExcute.Length..];
                    command.WaitForCompletion = true;
                }
                else
                {
                    command.WaitForCompletion = false;
                }

                command.Arguments = GetArgs(cmd.Substring(cmdIndex + 1, cmd.Length - cmdIndex - 2));
                result.Add(command);
            }
            return result;
        }
        //指令中空格分割参数
        private string[] GetArgs(string args)
        {
            //定义参数列表
            List<string> argList = new();
            //使用字符串构建器构建当前参数
            StringBuilder currentArg = new();
            //确认参数是否引用
            bool inQuotes = false;

            for (int t = 0; t < args.Length; t++)
            {
                if (args[t] == ID_ParameterNameContainer)
                {
                    inQuotes = !inQuotes;
                    continue;
                }
                if (!inQuotes && args[t] == ID_ParameterSpliter)
                {
                    argList.Add(currentArg.ToString());
                    currentArg.Clear();
                    continue;
                }
                currentArg.Append(args[t]);
            }
            //检测末尾参数（无空格分割）
            if (currentArg.Length > 0)
            {
                argList.Add(currentArg.ToString());
            }
            return argList.ToArray();
        }
        #endregion
    }
}