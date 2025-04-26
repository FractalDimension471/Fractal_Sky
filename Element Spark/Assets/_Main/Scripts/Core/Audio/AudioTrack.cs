using UnityEngine;
using UnityEngine.Audio;
public class AudioTrack
{
    #region ÊôÐÔ/Property
    public string Name { get; }
    public string Path { get; }
    public float CurrentVolume { get { return AudioSource.volume; } set { AudioSource.volume = value; } }
    public float CapVolume { get; }
    public float Pitch { get { return AudioSource.pitch; } set { AudioSource.pitch = value; } }
    public bool Loop => AudioSource.loop;
    public bool IsPlaying => AudioSource.isPlaying;
    private AudioChannel Channel { get; }
    private AudioSource AudioSource { get; }
    public GameObject Root => AudioSource.gameObject;
    #endregion
    #region ·½·¨/Method
    public AudioTrack(AudioClip audioClip, bool loop, float startVolume, float capVolume, float pitch, AudioChannel channel, AudioMixerGroup audioMixer, string path)
    {
        Name = audioClip.name;
        Path = path;
        Channel = channel;
        CapVolume = capVolume;

        AudioSource = CreateAudioSource();
        AudioSource.clip = audioClip;
        AudioSource.loop = loop;
        AudioSource.volume = startVolume;
        AudioSource.pitch = pitch;
        AudioSource.outputAudioMixerGroup = audioMixer;
    }
    private AudioSource CreateAudioSource()
    {
        GameObject go = new($"Track - [{Name}]");
        go.transform.SetParent(Channel.TrackContainer);
        AudioSource audioSource = go.AddComponent<AudioSource>();
        return audioSource;
    }
    public void PlayMusic()
    {
        AudioSource.Play();
    }
    public void StopMusic()
    {
        AudioSource.Stop();
    }
    #endregion
}
