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
    /// �Ի�������
    /// </summary>
    public class ConversationManager
    {
        
        #region ����/Property
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
        #region ����/Method
        /// <summary>
        /// �����Ի�������ʵ��
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
        /// ��ʼ�Ի�
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
            //RunningConversation��ΪЭ������
            Process = DialogueSystem.StartCoroutine(RunningConversation()); 
            return Process;
        }
        /// <summary>
        /// ֹͣ�Ի�
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
        /// ���жԻ�
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
        /// �����жԻ�
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        IEnumerator RunDialogueLine(DialogueLine line)
        {
            //ȷ��˵����
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
            //��ʼ�¶Ի�����򿪶Ի���
            if (!DialogueSystem.DialogueContainer.IsVisible)
            {
                yield return DialogueSystem.SetDialogueBoxVisibility();
            }
            //�����Ի�
            yield return BuildLineSegments(line.DialogueData);
            //�ȴ��û���Ӧ
            //yield return WaitForUserInput();
        }
        /// <summary>
        /// ����ָ����
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
                    //�����û�����
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
        /// �����Ի�
        /// </summary>
        /// <param name="dialogue"></param>
        /// <param name="append"></param>
        /// <returns></returns>
        IEnumerator BuildDialogue(string dialogue, bool append = false)
        {
            dialogue = TagManager.Inject(dialogue);
            //�ж�Ϊ��ӻ��߸���
            if (!append)
            {
                
                TextArchitect.Build(dialogue);
            }
            else
            {
                TextArchitect.Append(dialogue);
            }
            
            //ֱ���Ի�����������ֹͣЭ��
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
        /// �����Ի�����
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
        /// �ȴ��ź�
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
        /// �ȴ��û�����
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
            //��UI�������˵��������
            DialogueSystem.ShowSpeakerName(TagManager.Inject(data.DisplayName));
            //Ӧ�ý�ɫ������Ӧ��������Ϣ
            DialogueSystem.Instance.ApplySpeakerDataByName(data.Name);
            //���ý�ɫλ��
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
        /// �÷�����Ȼ���ܴ���bug������Progress���Գ������ޣ��������쳣�����Ի��У�2024/10/9
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

