using CHARACTERS;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
namespace DIALOGUE
{
    /// <summary>
    /// �Ի�ϵͳ
    /// </summary>
    public class DialogueSystem : MonoBehaviour
    {
        #region ����/Property
        [SerializeField] private float _TransitionSpeed = 1;
        public float TransitionSpeed => _TransitionSpeed;
        [SerializeField] private float _TextSpeed = 1;
        public float TextSpeed => _TextSpeed;
        private readonly bool initialized = false;
        public bool IsRunningConversation => ConversationManager.IsRunning;
        public bool IsVisible => CgController.IsVisible;

        public static DialogueSystem Instance { get; private set; }

        [SerializeField]
        private DialogueSystemConfigurationSO _config;
        public DialogueSystemConfigurationSO Config => _config;

        [SerializeField]
        private CanvasGroup mainCanvas;
        public ConversationManager ConversationManager { get; private set; }
        [field: SerializeField]
        private TextArchitect TextArchitect { get; set; }
        private CanvasGroupController CgController { get; set; }
        public AutoReader Reader { get; private set; }

        [SerializeField]
        private DialogueContainer _DialogueContainer;
        public DialogueContainer DialogueContainer => _DialogueContainer;
        [SerializeField]
        private NameContainer _NameContainer;
        public NameContainer NameContainer => _NameContainer;
        [SerializeField]
        private DialogueContinuePrompt _DialoguePrompt;
        public DialogueContinuePrompt DialoguePrompt => _DialoguePrompt;
        [field: SerializeField]
        public TextMeshProUGUI StatusText { get; set; }
        public delegate void DialogueSystemEvent();
        public event DialogueSystemEvent PromptNext;
        public event DialogueSystemEvent PromptClear;


        #endregion
        #region ����/Method

        //��Ŀ����ʱ��ȷ����ǰ��Ϸ����Ϊ��
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                Initialize();
            }
            else
            {
                DestroyImmediate(gameObject);
            }
        }
        private void Initialize()
        {
            if (initialized)
            {
                return;
            }
            DialogueContainer.Initialize();
            TextArchitect = new(DialogueContainer.DialogueText);
            ConversationManager = new(TextArchitect);
            CgController = new(this, mainCanvas);
            //��ʼ��Ĭ��ֵ�趨
            DialogueContainer.SetDialogueColor(Config.defaultTextColor);
            DialogueContainer.SetDialogueFont(Config.defaultFont);
            NameContainer.SetNameColor(Config.defaultTextColor);
            NameContainer.SetNameFont(Config.defaultFont);

            if (TryGetComponent(out AutoReader reader))
            {
                Reader = reader;
                Reader.Initialize(ConversationManager);
            }
        }
        /// <summary>
        /// ������һ��
        /// </summary>
        public void OnUserPromptNext()
        {
            PromptNext?.Invoke();
            if (Reader != null && Reader.IsRunning && Reader.Auto)
            {
                Reader.OnAutoButtomClicked();
            }
        }
        public void OnSystemPromptNext()
        {
            PromptNext?.Invoke();
        }
        public void OnSystemPromptClear()
        {
            PromptClear?.Invoke();
        }
        public void OnStarViewingHistory()
        {
            DialoguePrompt.Hide();
            ConversationManager.AllowUserPrompt = false;
            if (Reader.IsRunning)
            {
                Reader.Disable();
            }
        }
        public void OnStopViewingHistory()
        {
            DialoguePrompt.Show();
            ConversationManager.AllowUserPrompt = true;
        }

        /// <summary>
        /// ͨ������Ӧ��˵������Ϣ
        /// </summary>
        /// <param name="speakerName"></param>
        public void ApplySpeakerDataByName(string speakerName)
        {
            //��ȡ��ɫ���õ���������Ϣ
            Character character = CharacterManager.Instance.GetCharacter(speakerName);
            CharacterConfigData configData = character != null ? character.ConfigData : CharacterManager.Instance.GetCharacterConfigData(speakerName);//ȡ��Ĭ������
            ApplySpeakerData(configData);//����
        }
        /// <summary>
        /// Ӧ��˵������Ϣ
        /// </summary>
        /// <param name="configData"></param>
        public void ApplySpeakerData(CharacterConfigData configData)
        {

            //�������ֺͶԻ�������
            NameContainer.SetNameColor(configData.NameColor);
            NameContainer.SetNameFont(configData.NameFont);
            NameContainer.SetNameFontSize(configData.NameFontSize);
            DialogueContainer.SetDialogueColor(configData.DialogueColor);
            DialogueContainer.SetDialogueFont(configData.DialogueFont);
            DialogueContainer.SetDialogueFontSize(configData.DialogueFontSize * Config.dialogueFontSizeScale);
        }
        /// <summary>
        /// ��ʾ˵��������
        /// </summary>
        /// <param name="speakerName"></param>
        public void ShowSpeakerName(string speakerName = "")
        {
            if (speakerName != string.Empty)
            {
                NameContainer.SetVisibility(true, speakerName);
            }
            else
            {
                HideSpeakerName();
            }
        }
        /// <summary>
        /// ����˵��������
        /// </summary>
        public void HideSpeakerName() => NameContainer.SetVisibility(true, " ");
        /// <summary>
        /// ˵����˵��
        /// </summary>
        /// <param name="speaker"></param>
        /// <param name="dialogue"></param>
        /// <returns></returns>
        public Coroutine Say(string speaker, string dialogue)
        {
            List<string> conversation = new() { $"{speaker}@{dialogue}@" };//����һ�����ƹ��캯���������·���
            return Say(conversation);
        }
        /// <summary>
        /// ˵����˵��
        /// </summary>
        /// <param name="conversation"></param>
        /// <returns></returns>
        public Coroutine Say(List<string> lines, string filePath = "")
        {
            Conversation conversation = new(lines, file: filePath);
            return ConversationManager.StartConversation(conversation);
        }
        public Coroutine Say(Conversation conversation)
        {
            return ConversationManager.StartConversation(conversation);
        }
        public IEnumerator SetDialogueBoxVisibility()
        {
            if (!DialogueContainer.IsVisible)
            {
                yield return DialogueContainer.Show();
            }
            else
            {
                yield return DialogueContainer.Hide();
            }
            yield return FunctionPanel.Instance.SetPanelStatus();
        }
        public Coroutine Show(float speedMultiplier = 1f, bool immediate = false) => CgController.Show(speedMultiplier, immediate);
        public Coroutine Hide(float speedMultiplier = 1f, bool immediate = false) => CgController.Hide(speedMultiplier, immediate);
        public void ChangeAutoModeStatus()
        {
            Reader.OnAutoButtomClicked();
        }
        public void ChangeSkipModeSatatus()
        {
            Reader.OnSkipButtomClicked();
        }
        #endregion
    }
}

