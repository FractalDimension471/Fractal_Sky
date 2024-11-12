using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using COMMANDS;
using CHARACTERS;
using DIALOGUE.LogicalLine;
namespace DIALOGUE
{
    /// <summary>
    /// 对话管理器
    /// </summary>
    public class ConversationManager
    {
        
        #region 属性/Property
        public DialogueSystem DialogueSystem => DialogueSystem.Instance;
        public TextArchitect TextArchitect { get; }
        
        public Conversation TopConversation => ConversationQueue.IsEmpty ? null : ConversationQueue.Top;
        public int TopProgress => ConversationQueue.IsEmpty ? -1 : ConversationQueue.Top.Progress;
        
        public bool IsRunning => Process != null;
        public bool IsOnLogicalLine { get; private set; } = false;
        public bool IsWaitingSegmentTimer { get; private set; } = false;
        public bool AllowUserPrompt { get; set; } = true;
        public bool IsCharacterSpeaking { get; set; } = true;

        private bool UserPrompt { get; set; } = false;
        private LogicalLineManager LogicalLineManager => new();
        private ConversationQueue ConversationQueue { get; } = new();
        private Coroutine Process { get; set; } = null;

        #endregion
        #region 方法/Method
        /// <summary>
        /// 构建对话管理器实例
        /// </summary>
        /// <param name="textArchitect"></param>
        public ConversationManager(TextArchitect textArchitect)
        {
            TextArchitect = textArchitect;
            DialogueSystem.PromptNext += OnUserPromptNext;
        }
        private void OnUserPromptNext()
        {
            if(AllowUserPrompt)
            {
                UserPrompt = true;
            }
        }
        /// <summary>
        /// 开始对话
        /// </summary>
        /// <param name="conversation"></param>
        /// <returns></returns>
        public Coroutine StartConversation(Conversation conversation)
        {
            StopConversation();
            ConversationQueue.Clear();

            if (!ConversationQueue.CurrentQueue.Contains(conversation))
            {
                Enqueue(conversation);
            }
            //RunningConversation作为协程运行
            Process = DialogueSystem.StartCoroutine(RunningConversation()); 
            return Process;
        }
        /// <summary>
        /// 停止对话
        /// </summary>
        public void StopConversation()
        {
            if(!IsRunning)
            {
                return;
            }
            DialogueSystem.StopCoroutine(Process);
            Process = null;
        }
        /// <summary>
        /// 运行对话
        /// </summary>
        /// <param name="conversation"></param>
        /// <returns></returns>
        IEnumerator RunningConversation()
        {
            while(!ConversationQueue.IsEmpty)
            {
                Conversation currentConversation = TopConversation;
                if (currentConversation.HasReachedEnd)
                {
                    ConversationQueue.Dequeue();
                    continue;
                }
                string rawLine = currentConversation.CurrentDialogueLine;
                if (string.IsNullOrWhiteSpace(rawLine)) 
                {
                    TryAdvanceConversation(currentConversation);
                    continue;
                }
                DialogueLine line = DialogueParser.Parse(rawLine);

                if (LogicalLineManager.TryGetLogic(line, out Coroutine logic))
                {
                    IsOnLogicalLine = true;
                    yield return logic;
                }
                else
                {
                    if (line.HasDialogue)
                    {
                        yield return RunDialogueLine(line);
                    }
                    if (line.HasCommands)
                    {
                        yield return RunCommandsLine(line);
                    }
                    if (line.HasDialogue)
                    {
                        yield return WaitForUserInput();
                        CommandManager.Instance.StopAllProcesses();
                        DialogueSystem.OnSystemPromptClear();
                    }
                }
                TryAdvanceConversation(currentConversation);
                IsOnLogicalLine = false;
            }
            Process = null;
        }
        
        /// <summary>
        /// 运行行对话
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        IEnumerator RunDialogueLine(DialogueLine line)
        {
            //确认说话者
            if (line.HasSpeaker) 
            {
                if (line.SpeakerData.Name.ToLower() == "narrator")
                {
                    IsCharacterSpeaking = false;
                    DialogueSystem.HideSpeakerName();
                }
                else
                {
                    IsCharacterSpeaking = true;
                    HandleSpeakerLogic(line.SpeakerData);
                }
            }
            //开始新对话行则打开对话框
            if (!DialogueSystem.DialogueContainer.IsVisible)
            {
                yield return DialogueSystem.SetDialogueBoxVisibility();
            }
            //构建对话
            yield return BuildLineSegments(line.DialogueData);
            //等待用户相应
            //yield return WaitForUserInput();
        }
        /// <summary>
        /// 运行指令行
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        IEnumerator RunCommandsLine(DialogueLine line)
        {
            List<CommandData.Command> commands = line.CommandsData.Commands;
            foreach(CommandData.Command command in commands)
            {
                if (command.WaitForCompletion || command.Name == "wait") 
                {
                    CoroutineWrapper cw= CommandManager.Instance.Execute(command.Name, command.Arguments);
                    //用于用户介入
                    while (!cw.isDone)
                    {
                        if (UserPrompt)
                        {
                            CommandManager.Instance.StopCurrentProcess();
                            UserPrompt = false;
                        }
                        yield return null;
                    }
                    yield return cw;
                }
                else
                {
                    CommandManager.Instance.Execute(command.Name, command.Arguments);
                }
                
            }
            //Debug.Log(line.commandsData);
            yield return null;
        }
        /// <summary>
        /// 构建对话
        /// </summary>
        /// <param name="dialogue"></param>
        /// <param name="append"></param>
        /// <returns></returns>
        IEnumerator BuildDialogue(string dialogue, bool append = false)
        {
            dialogue = TagManager.Inject(dialogue);
            //判断为添加或者更新
            if (!append)
            {
                
                TextArchitect.Build(dialogue);
            }
            else
            {
                TextArchitect.Append(dialogue);
            }
            
            //直到对话构建结束再停止协程
            if (TextArchitect.IsBuilding)
            {
                if (UserPrompt)
                {
                    UserPrompt = false;
                }
                yield return null;
            }
        }
        /// <summary>
        /// 构建对话段落
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        IEnumerator BuildLineSegments(DialogueData line)
        {
            for(int t = 0; t < line.Segments.Count; t++)
            {
                DialogueData.DialogueSegment segment = line.Segments[t];
                yield return WaitForSegmentSignal(segment);
                yield return BuildDialogue(segment.Dialogue,segment.AppendText);
            }
        }
        /// <summary>
        /// 等待信号
        /// </summary>
        /// <param name="segment"></param>
        /// <returns></returns>
        IEnumerator WaitForSegmentSignal(DialogueData.DialogueSegment segment)
        {
            switch(segment.StartSignal)
            {
                case DialogueData.DialogueSegment.StartSignalTypes.C:
                    yield return WaitForUserInput();
                    DialogueSystem.OnSystemPromptClear();
                    break;
                case DialogueData.DialogueSegment.StartSignalTypes.A:
                    yield return WaitForUserInput();
                    break;
                case DialogueData.DialogueSegment.StartSignalTypes.WC:

                    IsWaitingSegmentTimer = true;
                    yield return new WaitForSeconds(segment.SignalDelay);
                    IsWaitingSegmentTimer = false;
                    DialogueSystem.OnSystemPromptClear();
                    break;
                case DialogueData.DialogueSegment.StartSignalTypes.WA:
                    IsWaitingSegmentTimer = true;
                    yield return new WaitForSeconds(segment.SignalDelay);
                    IsWaitingSegmentTimer = false;
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// 等待用户输入
        /// </summary>
        /// <returns></returns>
        IEnumerator WaitForUserInput()
        {
            DialogueSystem.DialoguePrompt.Show();
            while(!UserPrompt)
            {
                yield return null;
            }
            DialogueSystem.DialoguePrompt.Hide();
            UserPrompt = false;
        }
        private void HandleSpeakerLogic(SpeakerData data)
        {
            bool characterCreated = (data.MakeCharacterEnter || data.IsCastingExpressions || data.IsCastingPosition);
            Character character = CharacterManager.Instance.GetCharacter(data.Name,characterCreated);
            if (data.MakeCharacterEnter && !character.IsShowing) 
            {
                character.Show();
            }
            //向UI界面添加说话者名字
            DialogueSystem.ShowSpeakerName(TagManager.Inject(data.DisplayName));
            //应用角色名所对应的设置信息
            DialogueSystem.Instance.ApplySpeakerDataByName(data.Name);
            //设置角色位置
            if (data.IsCastingPosition)
            {
                character.SetPosition(data.CastPosition);
            }

            if (data.IsCastingExpressions)
            {
                foreach(var (layer, expression) in data.CastExpression)
                {
                    character.OnReceiveCastingExpression(layer, expression);
                }
            }
        }
        public void Enqueue(Conversation conversation) => ConversationQueue.Enqueue(conversation);
        public void Dequeue() => ConversationQueue.Dequeue();
        public void EnqueuePriority(Conversation conversation) => ConversationQueue.EnqueuePriority(conversation);
        /// <summary>
        /// 该方法仍然可能存在bug，导致Progress属性超出界限，常见于异常跳过对话行，2024/10/9
        /// </summary>
        /// <param name="conversation"></param>
        private void TryAdvanceConversation(Conversation conversation)
        {
            if (conversation != ConversationQueue.Top)
            {
                return;
            }
            TopConversation.IncrementProgress();
            if (TopConversation.HasReachedEnd)
            {
                ConversationQueue.Dequeue();
            }
        }
        public Conversation[] GetConversations() => ConversationQueue.GetQueue();
        #endregion
    }
}

