using DIALOGUE;
using HISTORY;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HistoryNavigator))]
[RequireComponent(typeof(HistoryLogManager))]
public class HistoryManager : MonoBehaviour
{
    #region ÊôÐÔ/Property
    public static HistoryManager Instance { get; private set; }
    [SerializeField]
    private int _MaxCacheCount = 100;
    public int MaxCacheCount => _MaxCacheCount;
    [field:SerializeField]
    public List<HistoryState> CachedStates {  get; internal set; }
    private HistoryNavigator Navigator { get; set; }
    public HistoryLogManager LogManager { get; private set; }
    #endregion
    #region ·½·¨/Method
    private void Awake()
    {
        Instance = this;
        Navigator = GetComponent<HistoryNavigator>();
        LogManager = GetComponent<HistoryLogManager>();
    }
    // Start is called before the first frame update
    void Start()
    {
        CachedStates = new();
        DialogueSystem.Instance.PromptClear += SaveState;
    }
    
    public void SaveState()
    {
        HistoryState state = HistoryState.Capture();
        if (state != null)
        {
            CachedStates.Add(state);
            LogManager.AddLog(state);
        }
        if(CachedStates.Count > MaxCacheCount)
        {
            CachedStates.RemoveAt(0);
        }
    }
    public void LoadState(HistoryState state)
    {
        state.Load();
    }
    public void GoForward() => Navigator.GoForward();
    public void GoBack() => Navigator.GoBack();
    public void SetLogStatus()
    {
        if (!LogManager.IsOpen)
        {
            LogManager.Open();
        }
        else
        {
            LogManager.Close();
        }
    }
    #endregion
}
