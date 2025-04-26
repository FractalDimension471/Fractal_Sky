using System;
using System.Collections;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class ESSystem : MonoBehaviour
{
    public static ESSystem Instance { get; private set; }
    private AudioManager AManagaer => AudioManager.Instance;
    private ElementBlockManager EBManager => ElementBlockManager.Instance;
    private RoundEndPanel REPanel => RoundEndPanel.Instance;
    private FunctionPanel FPanel => FunctionPanel.Instance;
    private Plate ActivePlate => Plate.Instance;
    private Reactor ActiveReactor => Reactor.Instance;
    [field: SerializeField]
    public int CurrentLevel { get; internal set; }
    [field:SerializeField]
    public int CapLevel { get; internal set; }
    [field: SerializeField]
    public ElementConfigSO ElementSO { get; internal set; }
    [field:SerializeField]
    public float TransitionSpeed { get; internal set; }
    [field: SerializeField]
    public Button MagnetButton { get; internal set; }
    [field:SerializeField]
    public Button BackTrackButton { get; internal set; }
    [field:SerializeField]
    public int MagnetCount { get; internal set; }
    [field:SerializeField]
    public int BackTrackCount { get; internal set; }

    private event Action<int> StartNewLevel;
    private event Action RestartCurrentLevel;
    private event Action<FunctionPanel.PageType> OpenFunctionPanelPage;
    private event Action CloseFunctionPanelPage;
    private event Action ChangeAudioMute;
    private event Action BackTrack;
    private event Action UseMagnet;
    private event Action BackToMainMenu;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        StartNewLevel += StartingNewLevel;
        RestartCurrentLevel += ResetartingCurrentLevel;
        OpenFunctionPanelPage += OpeningFPanelPage;
        CloseFunctionPanelPage += ClosingFPanelPage;
        ChangeAudioMute += ChangingAudioMute;
        BackTrack += BackTracking;
        UseMagnet += UsingMagnet;
        BackToMainMenu += BackingToMainMenu;

        StartCoroutine(ActivePlate.InitializeNewRound(CurrentLevel));
        AManagaer.PlayMusic(FilePaths.GetPath(FilePaths.DefaultMusicPaths,"Summer Sky"));
    }
    public void OnRetryButtonClicked()
    {
        RestartCurrentLevel?.Invoke();
        REPanel.Hide();
    }
    public void OnNextButtonClicked()
    {
        CurrentLevel++;
        StartNewLevel?.Invoke(CurrentLevel);
        REPanel.Hide();
    }
    public void OnHelpButtonClicked()
    {
        OpenFunctionPanelPage?.Invoke(FunctionPanel.PageType.Help);
    }
    public void OnSettingButtonClicked()
    {
        OpenFunctionPanelPage?.Invoke(FunctionPanel.PageType.Setting);
    }
    public void OnReturnButtonClicked()
    {
        CloseFunctionPanelPage?.Invoke();
    }
    public void OnMuteToggleClicked()
    {
        ChangeAudioMute?.Invoke();
    }
    public void OnBackTrackButtonClicked()
    {
        BackTrack?.Invoke();
    }
    public void OnMagnetButtonClicked()
    {
        UseMagnet?.Invoke();
    }
    public void OnBackToMainMenuButtonClicked()
    {
        BackToMainMenu?.Invoke();
    }
    private void UsingMagnet()
    {
        if (MagnetCount == 0)
        {
            return;
        }
        MagnetCount--;
        ActivePlate.UpdateItemCount();
        EBManager.UseMagnet();
    }
    private void BackTracking()
    {
        if (BackTrackCount == 0 || EBManager.DeletedElements.Count == 0)
        {
            return;
        }
        BackTrackCount--;
        ActivePlate.UpdateItemCount();
        EBManager.BackTrack();
    }
    private void ChangingAudioMute()
    {
        float volume = AManagaer.MuteToggle.isOn ? -80f : 0f;
        AManagaer.MainMixer.audioMixer.SetFloat("MainVolume", volume);
    }
    private void ClosingFPanelPage()
    {
        FPanel.ButtonsCG.interactable = true;
        FPanel.Hide();
    }
    private void OpeningFPanelPage(FunctionPanel.PageType page)
    {
        FPanel.ButtonsCG.interactable = false;
        FPanel.Show(page);
    }
    private void StartingNewLevel(int lvl)
    {
        StopAllCoroutines();
        ResetAll();
        StartCoroutine(ActivePlate.InitializeNewRound(lvl));
    }
    private void ResetartingCurrentLevel()
    {
        StopAllCoroutines();
        ResetAll();
        StartCoroutine(ActivePlate.InitializeNewRound(CurrentLevel));
    }
    private void ResetAll()
    {
        EBManager.Elements = new();
        EBManager.DeletedElements = new();
        EBManager.DeletedBlockedElements = new();
        EBManager.DeletedOverlappedElements = new();
        ActivePlate.SetStatus(true);
        ActivePlate.ResetPlate();
        ActiveReactor.ResetReactor();
    }
    private void FixedUpdate()
    {
        if (EBManager.GameStarted && EBManager.Elements.Count == 0)
        {
            if (CurrentLevel >= CapLevel)
            {
                ActivePlate.SetStatus(false);
                REPanel.Show("You have finished all levels.", false, true);
            }
            else
            {
                ActivePlate.SetStatus(false);
                REPanel.Show($"Level {CurrentLevel} complete.", true);
            }
        }
        if (EBManager.ActiveReactor.Elements.Count >= 4) 
        {
            ActivePlate.SetStatus(false);
            REPanel.Show($"Level {CurrentLevel} faild.", false);
        }
    }
    private void BackingToMainMenu()
    {
        AudioManager.Instance.CachedSounds.Clear();
        AudioManager.Instance.StopAll();
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
