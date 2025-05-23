using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
public class AudioManager : MonoBehaviour
{
    #region ����/Property
    public static AudioManager Instance { get; private set; }
    public static string ID_MainMixerVolume { get; } = "MainVolume";
    public static string ID_MusicMixerVolume { get; } = "MusicVolume";
    public static string ID_SoundMixerVolume { get; } = "SoundVolume";
    public static float C_MuteVloume { get; } = -80f;

    public Dictionary<int, AudioChannel> Channels { get; } = new();
    [field: SerializeField]
    public AudioMixerGroup MainMixer { get; private set; }
    [field: SerializeField]
    public AudioMixerGroup MusicMixer { get; private set; }
    [field: SerializeField]
    public AudioMixerGroup SoundMixer { get; private set; }
    [field: SerializeField]
    public AnimationCurve AudioNormalizeCurve { get; private set; }
    [field:SerializeField]
    public Toggle MuteToggle { get; internal set; }

    public Dictionary<string, AudioSource> CachedSounds { get; } = new();
    public AudioSource[] AllSound => SoundRoot.GetComponentsInChildren<AudioSource>();
    private Transform SoundRoot { get; set; }
    private static string ID_SoundParentName { get; } = "Sound";
    public static float C_TrackTransitionSpeed { get; } = 1f;

    #endregion
    #region ����/Method
    private void Awake()
    {
        if (Instance == null)
        {
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else
        {
            DestroyImmediate(gameObject);
            return;
        }

        SoundRoot = new GameObject(ID_SoundParentName).transform;
        SoundRoot.SetParent(transform);
    }
    public AudioSource PlaySound(string filePath, AudioMixerGroup audioMixer = null, float volume = 1, float pitch = 1, bool loop = false)
    {
        AudioClip audioClip = Resources.Load<AudioClip>(filePath);
        if (audioClip == null)
        {
            Debug.LogError($"Can not load audio file '{filePath}'!");
            return null;
        }
        return PlaySound(audioClip, audioMixer, volume, pitch, loop, filePath);
    }
    public AudioSource PlaySound(AudioClip audioClip, AudioMixerGroup audioMixer = null, float volume = 1, float pitch = 1, bool loop = false, string filePath = "")
    {

        AudioSource soundSource = new GameObject($"Sound - [{audioClip.name}]").AddComponent<AudioSource>();
        soundSource.transform.SetParent(SoundRoot);
        soundSource.clip = audioClip;

        if (loop)
        {
            CachedSounds.Add(filePath, soundSource);
        }

        if (audioMixer == null)
        {
            audioMixer = SoundMixer;
        }

        soundSource.outputAudioMixerGroup = audioMixer;
        soundSource.volume = volume;
        soundSource.pitch = pitch;
        soundSource.loop = loop;
        if (!loop)
        {
            Destroy(soundSource.gameObject, (audioClip.length / pitch) + 1);//�ӳ�ɾ��
        }
        soundSource.Play();
        return soundSource;
    }

    public void StopSound(AudioClip audioClip) => StopSound(audioClip.name);
    public void StopSound(string soundName)
    {
        soundName = soundName.ToLower();
        AudioSource[] audioSources = SoundRoot.GetComponentsInChildren<AudioSource>();
        foreach (AudioSource audioSource in audioSources)
        {
            if (audioSource.clip.name.ToLower() == soundName)
            {
                Destroy(audioSource.gameObject);
                return;
            }
        }
    }
    public AudioTrack PlayMusic(string filePath, int channelNumber = 0, bool loop = true, float startVolume = 0f, float capVolume = 1f, float pitch = 1f)
    {
        AudioClip audioClip = Resources.Load<AudioClip>(filePath);
        if (audioClip == null)
        {
            Debug.LogError($"Can not load audio file '{filePath}'!");
        }
        return PlayMusic(audioClip, channelNumber, loop, startVolume, capVolume, pitch, filePath);
    }
    public AudioTrack PlayMusic(AudioClip audioClip, int channelNumber = 0, bool loop = true, float startVolume = 0f, float capVolume = 1f, float pitch = 1f, string filePath = "")
    {
        AudioChannel channel = GetChannel(channelNumber, true);
        if (channel != null)
        {
            AudioTrack audioTrack = channel.PlayTrack(audioClip, loop, startVolume, capVolume, pitch, filePath);
            return audioTrack;
        }
        return null;
    }
    public void StopMusic(int channelNumber)
    {
        AudioChannel channel = GetChannel(channelNumber);
        if (channel == null)
        {
            return;
        }
        channel.StopTrack();
    }
    public void StopMusic(string musicName)
    {
        musicName = musicName.ToLower();
        foreach (AudioChannel channel in Channels.Values)
        {
            if (channel.ActiveTrack != null && channel.ActiveTrack.Name.ToLower() == musicName)
            {
                channel.StopTrack();
                return;
            }
        }
    }
    public void StopMusic()
    {
        foreach (AudioChannel channel in Channels.Values)
        {
            channel.StopTrack();
        }
    }
    public AudioChannel GetChannel(int channelNumber, bool createIfNotExist = false)
    {
        AudioChannel channel;
        if (Channels.TryGetValue(channelNumber, out channel))
        {
            return channel;
        }
        else if (createIfNotExist)
        {
            channel = new AudioChannel(channelNumber);
            Channels.Add(channelNumber, channel);
            return channel;
        }
        return channel;
    }
    public void StopAll()
    {
        StopAllSounds();
        StopAllTracks();
    }
    public void StopAllTracks()
    {
        foreach (var channel in Channels.Values)
        {
            channel.StopTrack();
        }
    }
    public void StopAllSounds()
    {
        foreach (AudioSource audioSource in AllSound)
        {
            Destroy(audioSource.gameObject);
        }
    }
    #endregion
}