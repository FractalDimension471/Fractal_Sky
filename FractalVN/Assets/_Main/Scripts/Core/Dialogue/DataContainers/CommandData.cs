using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace COMMANDS
{
    /// <summary>
    /// ָ������
    /// </summary>
    public class CommandData
    {
        #region ����/Property
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

        #region ����/Method
        /// <summary>
        /// ����ָ������
        /// </summary>
        /// <param name="rawCommands"></param>
        public CommandData(string rawCommands)
        {
            Commands = RipCommands(rawCommands);
        }
        private List<Command> RipCommands(string rawCommands)
        {
            //ȥ��ָ��հײ��ֲ��Զ��ŷָ�
            string[] data = rawCommands.Split(ID_CommandSpliter, System.StringSplitOptions.RemoveEmptyEntries);
            List<Command> result = new();
            foreach (string cmd in data)
            {
                Command command = new();
                //ʹ����������ƥ��
                int cmdIndex = cmd.IndexOf(ID_ParameterStarter);
                //��ȡָ����
                command.Name = cmd[..cmdIndex].Trim();
                //�ж��Ƿ�ȴ�
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
        //ָ���пո�ָ����
        private string[] GetArgs(string args)
        {
            //��������б�
            List<string> argList = new();
            //ʹ���ַ���������������ǰ����
            StringBuilder currentArg = new();
            //ȷ�ϲ����Ƿ�����
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
            //���ĩβ�������޿ո�ָ
            if (currentArg.Length > 0)
            {
                argList.Add(currentArg.ToString());
            }
            return argList.ToArray();
        }
        #endregion
    }
}