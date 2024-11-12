using HISTORY;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HistoryLogManager : MonoBehaviour
{
    #region ÊôÐÔ/Property
    private static float C_LogStartingHight { get; } = 2f;
    private static float C_LogHightPerLine { get; } = 2f;
    private static float C_DefaultHight { get; } = 1f;
    private static float C_DefaultTextScale { get; } = 0.5f;
    private static string ID_NameText { get; } = "Name Text";
    private static string ID_DialogueText { get; } = "Dialogue Text";

    public float LogScaling { get; private set; } = 1f;
    private float TextScaling => LogScaling * 2f;

    private HistoryManager HistoryManager => HistoryManager.Instance;
    private List<HistoryLog> Logs { get; } = new();
    private CanvasGroupController CgController { get; set; }
    
    [SerializeField] private GameObject _LogPrefab;
    [SerializeField] private CanvasGroup _OverlayCanvas;
    [SerializeField] private CanvasGroup _PanelCG;
    [SerializeField] private Slider _LogScaleSlider;
    
    private GameObject LogPrefab => _LogPrefab;
    private CanvasGroup OverlayCanvas => _OverlayCanvas;
    private CanvasGroup PanelCG => _PanelCG;
    private Slider LogScaleSlider => _LogScaleSlider;    

    public bool IsOpen { get; private set; } = false;
    #endregion
    #region ·½·¨/Method
    private void Start()
    {
        CgController = new(this, PanelCG);
        Initialize();
    }
    public void Initialize()
    {
        CgController.Alpha = 0f;
        CgController.SetCanvasStatus(false);
    }
    public void Open()
    {
        OverlayCanvas.interactable = true;
        OverlayCanvas.blocksRaycasts = true;
        if (IsOpen)
        {
            return;
        }
        CgController.Show();
        CgController.SetCanvasStatus(true);
        IsOpen = true;
    }
    public void Close()
    {
        OverlayCanvas.interactable = false;
        OverlayCanvas.blocksRaycasts = false;
        if (!IsOpen)
        {
            return;
        }
        CgController.Hide();
        CgController.SetCanvasStatus(false);
        IsOpen = false;
    }
    public void AddLog(HistoryState state)
    {
        if (Logs.Count >= HistoryManager.MaxCacheCount)
        {
            DestroyImmediate(Logs[0].Root);
            Logs.RemoveAt(0);
        }
        CreateLog(state);
    }
    private void CreateLog(HistoryState state)
    {
        HistoryLog log = new();
        log.Root = Instantiate(LogPrefab, LogPrefab.transform.parent);
        log.Root.SetActive(true);
        log.NameText = log.Root.transform.Find(ID_NameText).GetComponent<TextMeshProUGUI>();
        log.DialogueText = log.Root.transform.Find(ID_DialogueText).GetComponent<TextMeshProUGUI>();
        if (state.DialgoueData.Speaker.CurrentName == string.Empty)
        {
            log.NameText.text = string.Empty;
        }
        else
        {
            log.NameText.text = state.DialgoueData.Speaker.CurrentName;
            log.NameText.font = HistoryCache.LoadFont(state.DialgoueData.Speaker.Font);
            log.NameText.color = state.DialgoueData.Speaker.TextColor;
            log.NameFontSize = C_DefaultTextScale * state.DialgoueData.Speaker.FontSize;
            log.NameText.fontSize = log.NameFontSize + TextScaling;
        }
        log.DialogueText.text = state.DialgoueData.Dialogue.CurrentText;
        log.DialogueText.font = HistoryCache.LoadFont(state.DialgoueData.Dialogue.Font);
        log.DialogueText.color = state.DialgoueData.Dialogue.TextColor;
        log.DialogueFontSize = C_DefaultTextScale * state.DialgoueData.Dialogue.FontSize;
        log.DialogueText.fontSize = log.DialogueFontSize + TextScaling;
        ApplyLogToText(log);
        Logs.Add(log);

    }
    private void ApplyLogToText(HistoryLog log)
    {
        RectTransform rect = log.DialogueText.GetComponent<RectTransform>();
        ContentSizeFitter fitter = log.DialogueText.GetComponent<ContentSizeFitter>();
        LayoutElement layout = log.Root.GetComponent<LayoutElement>();
        fitter.SetLayoutVertical();
        float rate = rect.rect.height / C_DefaultHight;
        float padding = (C_LogHightPerLine * rate) - C_LogHightPerLine;
        float size = C_LogStartingHight + padding;
        layout.preferredHeight = size + TextScaling;
        layout.preferredHeight += 2f * LogScaling;

    }
    public void SetLogScaling()
    {
        LogScaling = LogScaleSlider.value;
        foreach (var log in Logs)
        {
            log.NameText.fontSize = log.NameFontSize + TextScaling;
            log.DialogueText.fontSize = log.DialogueFontSize + TextScaling;
            ApplyLogToText(log);
        }
    }
    public void Clear()
    {
        for (int i = 0; i < Logs.Count; i++)
        {
            DestroyImmediate(Logs[i].Root);
        }
        Logs.Clear();
    }
    public void Rebuild()
    {
        foreach(var state in HistoryManager.CachedStates)
        {
            CreateLog(state);
        }
    }
    #endregion
}
