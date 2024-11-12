using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading;
using System.Linq;
using DIALOGUE.LogicalLine;
public class ChoicePanel : MonoBehaviour
{
    #region 属性/Property
    public static ChoicePanel Instance { get; private set; }

    private static float C_MinButtonWidth { get; } = 768.89f;
    private static float C_MaxButtonWidth { get; } = 2000f;
    private static float C_PaddingButtonWidth { get; } = 10f;
    private static float C_ButtonHeightPerLine { get; } = 76.89f;
    //private const float C_ButtonHeightPadding = 20f;
    private CanvasGroupController CgController { get; set; }

    [SerializeField] private CanvasGroup _PanelCG;
    [SerializeField] private VerticalLayoutGroup _ButtonLayoutGroup;
    [SerializeField] private GameObject _ChoiceButtonPrefab;
    [SerializeField] private TextMeshProUGUI _TitleText;
    
    public ChoicePanelDecision LastDecision { get; private set; } = null;
    private struct ChoiceButton
    {
        public Button Button {  get; set; }
        public TextMeshProUGUI Title {  get; set; }
        public LayoutElement Layout {  get; set; }
    }
    private List<ChoiceButton> ChoiceButtons { get; set; }
    public bool IsWaitingOnUserMakingChoice { get; private set; } = false;
    public bool WaitingCountDown { get; internal set; } = false;
    
    #endregion
    #region 方法/Method
    public class ChoicePanelDecision
    {
        public string question = string.Empty;
        public int answerIndex = -1;
        public string[] choices = new string[0];
        public ChoicePanelDecision(string question, string[] choices)
        {
            this.question = question;
            this.choices = choices;
        }
    }
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        ChoiceButtons = new();
        CgController = new(this, _PanelCG);
        Initialize();
    }
    public void Initialize()
    {
        CgController.Alpha = 0f;
        CgController.SetCanvasStatus(false);
    }
    public void Show(string question, string[] choices)
    {
        _TitleText.text = question;
        LastDecision = new(question, choices);
        
        CgController.Show();
        CgController.SetCanvasStatus(true);
        IsWaitingOnUserMakingChoice = true;
        StartCoroutine(GeneratingChoices(choices));
    }

    public void Hide()
    {
        CgController.Hide();
        CgController.SetCanvasStatus(false);
    }
    
    private void OnAcceptedChoice(int index)
    {
        ChoiceLogic.IsWaitingCountDown = false;
        if (index < 0 || index > LastDecision.choices.Length - 1)
        {
            return;
        }
        LastDecision.answerIndex = index;
        IsWaitingOnUserMakingChoice = false;
        Hide();
    }
    public void OnChoiceCountDown()
    {
        int index = LastDecision.choices.Length - 1;
        LastDecision.answerIndex = index;
        IsWaitingOnUserMakingChoice = false;
        Hide();
    }
    public IEnumerator GeneratingChoices(string[] choices)
    {
        int counter = choices.Length;
        if (WaitingCountDown)
        {
            Interlocked.Decrement(ref counter);
            WaitingCountDown = false;
        }
        float maxWidth = 0;
        for (int i = 0; i < counter; Interlocked.Increment(ref i)) 
        {
            ChoiceButton choiceButton;
            if (i < ChoiceButtons.Count)
            {
                choiceButton = ChoiceButtons[i];
            }
            else
            {
                GameObject newButtonObject = Instantiate(_ChoiceButtonPrefab, _ButtonLayoutGroup.transform);
                newButtonObject.name = $"Choice {i}";

                Button newButton = newButtonObject.GetComponent<Button>();
                TextMeshProUGUI newTitle = newButtonObject.GetComponentInChildren<TextMeshProUGUI>();
                LayoutElement newLayout = newButtonObject.GetComponent<LayoutElement>();
                choiceButton = new ChoiceButton { Button = newButton, Layout = newLayout, Title = newTitle };
                ChoiceButtons.Add(choiceButton);
            }
            choiceButton.Button.onClick.RemoveAllListeners();
            int choiceIndex = i;//闭包用的，不能简化
            choiceButton.Button.onClick.AddListener(() => OnAcceptedChoice(choiceIndex));
            choiceButton.Title.text = choices[i];
            float buttonWidth = Mathf.Clamp(C_PaddingButtonWidth + choiceButton.Title.preferredWidth, C_MinButtonWidth, C_MaxButtonWidth);
            maxWidth = Mathf.Max(maxWidth, buttonWidth);
        }
        foreach (var choiceButton in ChoiceButtons)
        {
            choiceButton.Layout.preferredWidth = maxWidth;
        }
        for(int i = 0; i < ChoiceButtons.Count; i++)
        {
            bool show = i < choices.Length;
            ChoiceButtons[i].Button.gameObject.SetActive(show);
        }
        yield return new WaitForEndOfFrame();

        foreach (var choiceButton in ChoiceButtons)
        {
            choiceButton.Title.ForceMeshUpdate();
            int lines = choiceButton.Title.textInfo.lineCount;
            choiceButton.Layout.preferredHeight = lines * C_ButtonHeightPerLine;
        }
        
    }
    #endregion
}
