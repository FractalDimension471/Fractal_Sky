using GALGAME;
using System;
using System.Linq;
using UnityEngine;
[Serializable]
public class ConfigData
{
    public static ConfigData ActiveConfig { get; internal set; }
    public static string DefaultConfigFileName => "GalConfig";
    public static string FilePath => FilePaths.GetPath(FilePaths.RunTimePath, DefaultConfigFileName + GalSaveFile.ID_DataFileType);
    public static bool Encrypt { get; } = true;
    //Generic
    public bool IsFullScreen = true;
    public string CurrentResolution = "1920x1080";
    public float CurrentTextSpeed = 1f;
    public float CurrentAutoReadSpeed = 1f;
    //Audio
    public bool MuteMain = false;
    public bool MuteMusic = false;
    public bool MuteSound = false;
    public bool MuteVoice = false;
    public float MainVolume = 1f;
    public float MusicVolume = 1f;
    public float SoundVolume = 1f;
    public float VoiceVolume = 1f;
    //other
    public float HistoryLogScale = 1f;
    public void Save()
    {
        FileManager.Save(FilePath, JsonUtility.ToJson(ActiveConfig), Encrypt);
        Debug.Log("Config data saved!");
    }
    public void Load()
    {
        ActiveConfig = this;
        var controls = ConfigPage.Instance.UserControls;
        //Generic   
        ConfigPage.Instance.SetDiplayToFullScreen(IsFullScreen);
        controls.OnOneOfTwinToggleSelected(controls.FullScreen, controls.Windowed, IsFullScreen);
        int resolutionIndex = controls.Resolutions.options.IndexOf(controls.Resolutions.options.FirstOrDefault(r => r.text == CurrentResolution));
        controls.Resolutions.value = resolutionIndex;
        controls.TextSpeed.value = CurrentTextSpeed;
        controls.AutoReadSpeed.value = CurrentAutoReadSpeed;
        //Audio
        controls.MainVolume.value = MainVolume;
        controls.MusicVolume.value = MusicVolume;
        controls.SoundVolume.value = SoundVolume;
        controls.VoiceVolume.value = VoiceVolume;
        if (controls.MainMutePanel != null)
        {
            controls.MainMutePanel.isOn = MuteMain;
        }
        controls.MainMute.isOn = MuteMain;
        controls.MusicMute.isOn = MuteMusic;
        controls.SoundMute.isOn = MuteSound;
        controls.VoiceMute.isOn = MuteVoice;
    }
}
