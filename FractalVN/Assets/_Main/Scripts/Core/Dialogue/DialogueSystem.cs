using CHARACTERS;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
namespace DIALOGUE
{
    /// <summary>
    /// 对话系统
    /// </summary>
    public class DialogueSystem : MonoBehaviour
    {
        #region 属性/Property
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
        #region 方法/Method

        //项目启动时，确保当前游戏对象为空
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
            //初始化默认值设定
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
        /// 触发下一句
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
        /// 通过名称应用说话者信息
        /// </summary>
        /// <param name="speakerName"></param>
        public void ApplySpeakerDataByName(string speakerName)
        {
            //获取角色并得到其设置信息
            Character character = CharacterManager.Instance.GetCharacter(speakerName);
            CharacterConfigData configData = character != null ? character.ConfigData : CharacterManager.Instance.GetCharacterConfigData(speakerName);//取得默认设置
            ApplySpeakerData(configData);//重载
        }
        /// <summary>
        /// 应用说话者信息
        /// </summary>
        /// <param name="configData"></param>
        public void ApplySpeakerData(CharacterConfigData configData)
        {

            //设置名字和对话的属性
            NameContainer.SetNameColor(configData.NameColor);
            NameContainer.SetNameFont(configData.NameFont);
            NameContainer.SetNameFontSize(configData.NameFontSize);
            DialogueContainer.SetDialogueColor(configData.DialogueColor);
            DialogueContainer.SetDialogueFont(configData.DialogueFont);
            DialogueContainer.SetDialogueFontSize(configData.DialogueFontSize * Config.dialogueFontSizeScale);
        }
        /// <summary>
        /// 显示说话者名称
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
        /// 隐藏说话者名称
        /// </summary>
        public void HideSpeakerName() => NameContainer.SetVisibility(true, " ");
        /// <summary>
        /// 说话者说话
        /// </summary>
        /// <param name="speaker"></param>
        /// <param name="dialogue"></param>
        /// <returns></returns>
        public Coroutine Say(string speaker, string dialogue)
        {
            List<string> conversation = new() { $"{speaker}@{dialogue}@" };//定义一个类似构造函数的匿名新方法
            return Say(conversation);
        }
        /// <summary>
        /// 说话者说话
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

