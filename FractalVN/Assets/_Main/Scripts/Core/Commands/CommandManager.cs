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
    /// ָ�������
    /// </summary>
    public class CommandManager : MonoBehaviour
    {
        #region ����/Property
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
        #region ����/Method
        private void Awake()
        {
            if (Instance == null)
            {
                //ʵ����
                Instance = this;
                //����ָ�
                Assembly assembly = Assembly.GetExecutingAssembly();
                //��ȡ��չ����
                Type[] extensionTypes = assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(DatabaseExtention))).ToArray();
                //��ȡ���ݿ���չ������
                foreach (Type extension in extensionTypes)
                {
                    //ʹ�÷����ȡ����DataBaseExtension����ķ�����ִ��
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
        ///ִ�з�����ʹ��params�ؼ��ֿ��Խ������Զ�����
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
            //ͨ����������ȡָ��
            Delegate command = Database.GetCommand(commandName.ToLower());
            if (command == null)
            {
                return null;
            }

            //ʹ��Э��ʵ��
            return StartProcess(commandName, command, args);
        }
        /// <summary>
        /// �ж�ָ���������
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

            //�������װΪЭ������
            commandProcess.RunningProcess = new(this, CO_Command);
            return commandProcess.RunningProcess;
        }
        private CoroutineWrapper ExecuteSubCommand(string commandName, string[] args)
        {
            //�ָ���Ϊ����
            string[] parts = commandName.Split(ID_SubCommand);
            //databaseName��"<��ɫ����>"
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
            //ͨ��ָ��(Text)
            Delegate command;
            if (commandDatabase.HasCommand(commandName))
            {
                command = commandDatabase.GetCommand(commandName);
                return StartProcess(commandName, command, args);
            }
            CharacterConfigData characterConfigData = CharacterManager.Instance.GetCharacterConfigData(args[0]);
            //����ɫ����
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
        /// ��ֹ��ǰ���еĽ���
        /// </summary>
        public void StopCurrentProcess()
        {
            if (TopProcess != null)
            {
                KillProcess(TopProcess);
            }
        }
        /// <summary>
        /// ��ֹ���н���
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
            //�ȴ����
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
                command.DynamicInvoke((object)args);//ע�ⷽ�������������Ϊobject
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