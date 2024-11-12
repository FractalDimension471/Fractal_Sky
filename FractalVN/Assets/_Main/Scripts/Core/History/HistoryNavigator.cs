using DIALOGUE;
using HISTORY;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HistoryNavigator : MonoBehaviour
{
    #region ÊôÐÔ/Property
    [field: SerializeField]
    private int Progress { get; set; }
    [field: SerializeField]
    private bool IsViewingHistory { get; set; }
    private TextMeshProUGUI StatusText => DialogueSystem.StatusText;

    private HistoryManager HistoryManager => HistoryManager.Instance;
    private List<HistoryState> States => HistoryManager.CachedStates;

    private HistoryState CachedState { get; set; }
    private bool IsOnCachedState { get; set; } = false;
    private bool CanNavigate => !DialogueSystem.Instance.ConversationManager.IsOnLogicalLine;
    private DialogueSystem DialogueSystem => DialogueSystem.Instance;
    #endregion
    #region ·½·¨/Method
    public void GoForward()
    {
        if (!IsViewingHistory || !CanNavigate)
        {
            return;
        }
        HistoryState state = null;
        if (Progress < States.Count - 1)
        {
            Progress++;
            state = States[Progress];
        }
        else
        {
            IsOnCachedState = true;
            state = CachedState;
        }

        state.Load();
        if(IsOnCachedState)
        {
            IsViewingHistory = false;
            DialogueSystem.PromptNext -= GoForward;
            StatusText.text = "";
            DialogueSystem.OnStopViewingHistory();
        }
        else
        {
            UpdateStatusText();
        }
    }
    public void GoBack()
    {
        if (States.Count == 0 || (IsViewingHistory && Progress == 0) || !CanNavigate) 
        {
            return;
        }
        Progress = IsViewingHistory ? Progress - 1 : States.Count - 1;

        if (!IsViewingHistory)
        {
            IsViewingHistory = true;
            IsOnCachedState = false;

            CachedState = HistoryState.Capture();
            DialogueSystem.PromptNext += GoForward;
            DialogueSystem.OnStarViewingHistory();
        }
        HistoryState state = States[Progress];
        state.Load();
        UpdateStatusText();
    }

    private void UpdateStatusText()
    {
        StatusText.text = $"History: {States.Count - Progress}/{States.Count}";
    }
    #endregion
}
