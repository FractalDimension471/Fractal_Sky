using DIALOGUE;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfigPage : MenuPage
{
    public static ConfigPage Instance { get; private set; }
    [field: SerializeField]
    public GameObject[] SubPages { get; private set; }
    private GameObject ActiveSubPage { get; set; }
    [field: SerializeField]
    public Controls UserControls { get; private set; }
    private static ConfigData Config { get; set; } = Config;
    private AudioManager AudioManager => AudioManager.Instance;
    private AnimationCurve NormalizeCurve => AudioManager.AudioNormalizeCurve;

    [System.Serializable]
    public class Controls
    {
        [field: Header("Generic")]
        [field: SerializeField]
        public Toggle FullScreen { get; private set; }
        [field: SerializeField]
        public Toggle Windowed { get; private set; }
        [field: SerializeField]
        public TMP_Dropdown Resolutions { get; private set; }
        [field: SerializeField]
        public Slider TextSpeed { get; private set; }
        [field: SerializeField]
        public Slider AutoReadSpeed { get; private set; }



        [field: Header("Audio")]
        [field: SerializeField]
        public Slider MainVolume { get; private set; }
        [field: SerializeField]
        public Slider MusicVolume { get; private set; }
        [field: SerializeField]
        public Slider SoundVolume { get; private set; }
        [field: SerializeField]
        public Slider VoiceVolume { get; private set; }
        [field: SerializeField]
        public Toggle MainMutePanel { get; private set; }
        [field: SerializeField]
        public Toggle MainMute { get; private set; }
        [field: SerializeField]
        public Toggle MusicMute { get; private set; }
        [field: SerializeField]
        public Toggle SoundMute { get; private set; }
        [field: SerializeField]
        public Toggle VoiceMute { get; private set; }

        public void OnOneOfTwinToggleSelected(Toggle A, Toggle B, bool selectedA)
        {
            A.isOn = selectedA;
            B.isOn = !selectedA;
        }
    }
    private void Awake()
    {
        Instance = this;
        SubPages.First().SetActive(true);
        ActiveSubPage = SubPages[0];
        SetAvailableResolution();
        LoadConfig();
    }
    private void OnApplicationQuit()
    {
        Config.Save();
        Config = null;
    }
    public void OpenSubPage(string pageName)
    {
        GameObject page = SubPages.First(p => p.name == pageName);
        if (page == null)
        {
            Debug.LogWarning($"Cannot find page '{pageName} in menu.'");
            return;
        }
        if (ActiveSubPage != null && ActiveSubPage != page)
        {
            ActiveSubPage.SetActive(false);
        }
        page.SetActive(true);
        ActiveSubPage = page;
    }
    private void SetAvailableResolution()
    {
        var resolutions = Screen.resolutions.Reverse();
        List<string> options = new();
        foreach (var resolution in resolutions)
        {
            options.Add($"{resolution.width}x{resolution.height}");
        }
        UserControls.Resolutions.ClearOptions();
        UserControls.Resolutions.AddOptions(options);
    }
    private void LoadConfig()
    {
        if (File.Exists(ConfigData.FilePath))
        {
            Config = FileManager.Load<ConfigData>(ConfigData.FilePath, ConfigData.Encrypt);
        }
        else
        {
            Config = new ConfigData();
        }
        Config.Load();
    }
    public void SetDiplayToFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }
    public void OnChangeFullScreenMode()
    {
        Config.IsFullScreen = UserControls.FullScreen.isOn;
        Screen.fullScreen = Config.IsFullScreen;
        if (UserControls.Windowed.isOn == UserControls.FullScreen.isOn)
        {
            UserControls.Windowed.isOn = !UserControls.FullScreen.isOn;
        }
    }
    public void OnChangeWindowedMode()
    {
        Config.IsFullScreen = !UserControls.Windowed.isOn;
        Screen.fullScreen = Config.IsFullScreen;
        if (UserControls.FullScreen.isOn == UserControls.Windowed.isOn)
        {
            UserControls.FullScreen.isOn = !UserControls.Windowed.isOn;
        }
    }
    public void SetDisplayResolution()
    {
        string resolution = UserControls.Resolutions.captionText.text;
        string[] values = resolution.Split('x');
        if (int.TryParse(values[0], out int width) && int.TryParse(values[1], out int height))
        {
            Screen.SetResolution(width, height, Screen.fullScreen);
            Config.CurrentResolution = resolution;
        }
        else
        {
            Debug.LogError($"Detected invalid resolution value: '{resolution}'");
        }
    }
    public void SetTextSpeed()
    {
        Config.CurrentTextSpeed = UserControls.TextSpeed.value;
        if (DialogueSystem.Instance != null)
        {
            DialogueSystem.Instance.ConversationManager.TextArchitect.CurrentTextSpeed = Config.CurrentTextSpeed;
        }
    }
    public void SetAutoReadSpeed()
    {
        Config.CurrentAutoReadSpeed = UserControls.AutoReadSpeed.value;
        if (DialogueSystem.Instance == null)
        {
            return;
        }
        AutoReader reader = DialogueSystem.Instance.Reader;
        if (reader != null)
        {
            reader.SpeedMultiplier = Config.CurrentAutoReadSpeed;
        }
    }
    public void SetMainVolume()
    {
        Config.MainVolume = UserControls.MainVolume.value;
        var mainMixer = AudioManager.MainMixer;
        float volume = Config.MuteMain ? AudioManager.C_MuteVloume : NormalizeCurve.Evaluate(Config.MainVolume);
        mainMixer.audioMixer.SetFloat(AudioManager.ID_MainMixerVolume, volume);
    }
    public void SetMusicVolume()
    {
        Config.MusicVolume = UserControls.MusicVolume.value;
        var musicMixer = AudioManager.MusicMixer;
        float volume = Config.MuteMusic ? AudioManager.C_MuteVloume : NormalizeCurve.Evaluate(Config.MusicVolume);
        musicMixer.audioMixer.SetFloat(AudioManager.ID_MusicMixerVolume, volume);
    }
    public void SetSoundVolume()
    {
        Config.SoundVolume = UserControls.SoundVolume.value;
        var soundMixer = AudioManager.SoundMixer;
        float volume = Config.MuteSound ? AudioManager.C_MuteVloume : NormalizeCurve.Evaluate(Config.SoundVolume);
        soundMixer.audioMixer.SetFloat(AudioManager.ID_SoundMixerVolume, volume);
    }
    public void SetVoiceVolume()
    {
        Config.VoiceVolume = UserControls.VoiceVolume.value;
        var voiceMixer = AudioManager.VoiceMixer;
        float volume = Config.MuteVoice ? AudioManager.C_MuteVloume : NormalizeCurve.Evaluate(Config.VoiceVolume);
        voiceMixer.audioMixer.SetFloat(AudioManager.ID_VoiceMixerVolume, volume);
    }
    public void SetMainMutePanel()
    {
        Config.MuteMain = UserControls.MainMutePanel.isOn;
        if (UserControls.MainMute.isOn != UserControls.MainMutePanel.isOn)
        {
            UserControls.MainMute.isOn = UserControls.MainMutePanel.isOn;
        }
        SetMainVolume();
    }
    public void SetMainMute()
    {
        Config.MuteMain = UserControls.MainMute.isOn;
        if (UserControls.MainMutePanel != null && UserControls.MainMutePanel.isOn != UserControls.MainMute.isOn)
        {
            UserControls.MainMutePanel.isOn = UserControls.MainMute.isOn;
        }

        SetMainVolume();
    }
    public void SetMusicMute()
    {
        Config.MuteMusic = UserControls.MusicMute.isOn;
        SetMusicVolume();
    }
    public void SetSoundMute()
    {
        Config.MuteSound = UserControls.SoundMute.isOn;
        SetSoundVolume();
    }
    public void SetVoiceMute()
    {
        Config.MuteVoice = UserControls.VoiceMute.isOn;
        SetVoiceVolume();
    }
}
