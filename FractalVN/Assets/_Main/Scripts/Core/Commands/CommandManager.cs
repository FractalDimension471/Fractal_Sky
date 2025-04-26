using CHARACTERS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

namespace COMMANDS
{
    /// <summary>
    /// 指令管理器
    /// </summary>
    public class CommandManager : MonoBehaviour
    {
        #region 属性/Property
        public static CommandManager Instance { get; private set; }
        private CommandDatabase Database { get; } = new();
        private Dictionary<string, CommandDatabase> SubDatabases { get; } = new();
        private List<CommandProcess> ActiveProcesses { get; } = new();
        private CommandProcess TopProcess => ActiveProcesses.Last();
        private static char ID_SubCommand { get; } = '.';
        public static string ID_CharacterDB_Generic { get; } = "characters";
        public static string ID_CharacterDB_Sprite { get; } = "characterID_sprite";
        //public const string ID_CharacterDB_Live2D = "characterID_live2d";
        //public const string ID_CharacterDB_Model3D = "characterID_model3d";
        #endregion
        #region 方法/Method
        private void Awake()
        {
            if (Instance == null)
            {
                //实例化
                Instance = this;
                //定义指令集
                Assembly assembly = Assembly.GetExecutingAssembly();
                //获取扩展类型
                Type[] extensionTypes = assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(DatabaseExtention))).ToArray();
                //获取数据库扩展的类型
                foreach (Type extension in extensionTypes)
                {
                    //使用反射获取所有DataBaseExtension里面的方法并执行
                    MethodInfo extendMethod = extension.GetMethod("Extend");
                    extendMethod.Invoke(Instance, new object[] { Database });
                }
            }
            else
            {
                DestroyImmediate(gameObject);
            }
        }
        /// <summary>
        ///执行方法，使用params关键字可以将参数自动整合
        /// </summary>
        /// <param name="commandName"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public CoroutineWrapper Execute(string commandName, params string[] args)
        {
            if (commandName.Contains(ID_SubCommand))
            {
                return ExecuteSubCommand(commandName, args);
            }
            //通过命令名获取指令
            Delegate command = Database.GetCommand(commandName.ToLower());
            if (command == null)
            {
                return null;
            }

            //使用协程实现
            return StartProcess(commandName, command, args);
        }
        /// <summary>
        /// 中断指定命令程序
        /// </summary>
        /// <param name="commandProcess"></param>
        public void KillProcess(CommandProcess commandProcess)
        {
            ActiveProcesses.Remove(commandProcess);
            if (commandProcess.RunningProcess != null && !commandProcess.RunningProcess.IsDone)
            {
                commandProcess.RunningProcess.Stop();
            }
            commandProcess.EndingAction?.Invoke();
        }
        private CoroutineWrapper StartProcess(string commandName, Delegate command, string[] args)
        {
            Guid processID = Guid.NewGuid();
            CommandProcess commandProcess = new(processID, commandName, command, args, null, null);
            ActiveProcesses.Add(commandProcess);
            Coroutine CO_Command = StartCoroutine(RunningProcess(commandProcess));

            //将命令封装为协程运行
            commandProcess.RunningProcess = new(this, CO_Command);
            return commandProcess.RunningProcess;
        }
        private CoroutineWrapper ExecuteSubCommand(string commandName, string[] args)
        {
            //分割作为键名
            string[] parts = commandName.Split(ID_SubCommand);
            //databaseName是"<角色名称>"
            string databaseName = string.Join(ID_SubCommand, parts.Take(parts.Length - 1));
            string subCommandName = parts.Last().ToLower();

            var characterName = CharacterManager.Instance.GetNameFromAlias(databaseName);

            if (SubDatabases.ContainsKey(characterName))
            {
                Delegate command = SubDatabases[characterName].GetCommand(subCommandName);
                if (command != null)
                {
                    return StartProcess(commandName, command, args);
                }
                else
                {
                    Debug.LogError($"There is no command called '{commandName}' in database '{characterName}'");
                    return null;
                }
            }

            if (CharacterManager.Instance.HasCharacter(characterName))
            {
                List<string> newArgs = new(args);
                newArgs.Insert(0, characterName);
                args = newArgs.ToArray();
                return ExecuteCharacterCommand(subCommandName, args);
            }
            Debug.LogError($"There is no command called '{commandName}' in database '{characterName}', '{subCommandName}' cannot be run.");
            return null;
        }
        private CoroutineWrapper ExecuteCharacterCommand(string commandName, params string[] args)
        {
            CommandDatabase commandDatabase = SubDatabases[ID_CharacterDB_Generic];
            //通用指令(Text)
            Delegate command;
            if (commandDatabase.HasCommand(commandName))
            {
                command = commandDatabase.GetCommand(commandName);
                return StartProcess(commandName, command, args);
            }
            CharacterConfigData characterConfigData = CharacterManager.Instance.GetCharacterConfigData(args[0]);
            //按角色类型
            switch (characterConfigData.CharacterType)
            {
                case Character.CharacterType.Sprite:
                    //case Character.CharacterType.SpriteSheet:
                    commandDatabase = SubDatabases[ID_CharacterDB_Sprite];
                    break;
                    //case Character.CharacterType.Live2D:
                    //case Character.CharacterType.Model3D:
            }
            command = commandDatabase.GetCommand(commandName);
            if (command != null)
            {
                return StartProcess(commandName, command, args);
            }
            Debug.LogError($"CommandManager cannot execute command '{commandName}' on character '{args[0]}'.{Environment.NewLine}The character name or command may be invalid.");
            return null;
        }
        /// <summary>
        /// 终止当前运行的进程
        /// </summary>
        public void StopCurrentProcess()
        {
            if (TopProcess != null)
            {
                KillProcess(TopProcess);
            }
        }
        /// <summary>
        /// 终止所有进程
        /// </summary>
        public void StopAllProcesses()
        {
            foreach (CommandProcess commandProcess in ActiveProcesses)
            {
                if (commandProcess.RunningProcess != null && !commandProcess.RunningProcess.IsDone)
                {
                    commandProcess.RunningProcess.Stop();
                }
                commandProcess.EndingAction?.Invoke();
            }
            ActiveProcesses.Clear();
        }
        private IEnumerator RunningProcess(CommandProcess commandProcess)
        {
            //等待完成
            yield return WaitingForProcessToComplete(commandProcess.Command, commandProcess.Args);

            KillProcess(commandProcess);
        }
        private IEnumerator WaitingForProcessToComplete(Delegate command, string[] args)
        {
            if (command is Action)
            {
                command.DynamicInvoke();
            }
            else if (command is Action<string>)
            {
                command.DynamicInvoke(args[0]);
            }
            else if (command is Action<string[]>)
            {
                command.DynamicInvoke((object)args);//注意方法所需参数类型为object
            }
            else if (command is Func<IEnumerator> funcNP)
            {
                yield return funcNP();
            }
            else if (command is Func<string, IEnumerator> funcSP)
            {
                yield return funcSP(args[0]);
            }
            else if (command is Func<string[], IEnumerator> funcMP)
            {
                yield return funcMP(args);
            }
        }
        public void AddEndingActionToCurrentProcess(UnityAction action)
        {
            CommandProcess commandProcess = TopProcess;
            if (commandProcess == null)
            {
                return;
            }
            commandProcess.EndingAction = new UnityEvent();
            commandProcess.EndingAction.AddListener(action);
        }
        public CommandDatabase CreateSubDatabase(string name)
        {
            name = name.ToLower();
            if (SubDatabases.TryGetValue(name, out CommandDatabase commandDatabase))
            {
                Debug.LogWarning($"A database named '{name}' already exists!");
                return commandDatabase;
            }
            CommandDatabase newDatabase = new();
            SubDatabases.Add(name, newDatabase);
            return newDatabase;
        }
        #endregion
    }
}