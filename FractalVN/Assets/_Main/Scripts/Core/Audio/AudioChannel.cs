using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
public class AudioChannel
{
    #region ÊôÐÔ/Property
    public int ChannelIndex { get; }
    private bool IsLevelingVolume => Co_levelingVolume != null;
    private bool CanLevelingVolume => ((ActiveTrack != null && (AudioTracks.Count > 1 || ActiveTrack.CurrentVolume != ActiveTrack.CapVolume)) || (ActiveTrack == null && AudioTracks.Count > 0));
    public Transform TrackContainer { get; }
    public AudioTrack ActiveTrack { get; private set; }
    private List<AudioTrack> AudioTracks { get; }
    private Coroutine Co_levelingVolume { get; set; }
    #endregion
    #region ·½·¨/Method
    public AudioChannel(int channel)
    {
        AudioTracks = new();

        ChannelIndex = channel;
        TrackContainer = new GameObject($"Channel {channel}").transform;
        TrackContainer.SetParent(AudioManager.Instance.transform);
    }
    public AudioTrack PlayTrack(AudioClip audioClip, bool loop, float startVolume, float capVolume, float pitch, string filePath)
    {
        if (TryGetTrack(audioClip.name, out AudioTrack audioTrack))
        {
            if (!audioTrack.IsPlaying)
            {
                audioTrack.PlayMusic();
            }
            ActivateTrack(audioTrack);
            return audioTrack;
        }
        audioTrack = new AudioTrack(audioClip, loop, startVolume, capVolume, pitch, this, AudioManager.Instance.MusicMixer, filePath);
        audioTrack.PlayMusic();
        ActivateTrack(audioTrack);
        return audioTrack;
    }
    public void StopTrack(bool immediate = false)
    {
        if (ActiveTrack == null)
        {
            return;
        }
        if (immediate)
        {
            DestoryTrack(ActiveTrack);
            ActiveTrack = null;
        }
        else
        {
            ActiveTrack = null;
            TryLevelingVolume();
        }
    }
    public bool TryGetTrack(string trackName, out AudioTrack value)
    {
        trackName = trackName.ToLower();
        foreach(AudioTrack audioTrack in AudioTracks)
        {
            if (audioTrack.Name.ToLower() == trackName)
            {
                value = audioTrack;
                return true;
            }
        }
        value = null;
        return false;
    }
    private void TryLevelingVolume()
    {
        if (!IsLevelingVolume)
        {
            Co_levelingVolume = AudioManager.Instance.StartCoroutine(LevelingVolume());
        }
    }
    private IEnumerator LevelingVolume()
    {
        
        while (CanLevelingVolume)
        {
            for (int i = AudioTracks.Count - 1; i >= 0; Interlocked.Decrement(ref i))
            {
                AudioTrack audioTrack = AudioTracks[i];
                float targetVolume = ActiveTrack == audioTrack ? audioTrack.CapVolume : 0;
                if (audioTrack == ActiveTrack && audioTrack.CurrentVolume == targetVolume)
                {
                    continue;
                }
                audioTrack.CurrentVolume = Mathf.MoveTowards(audioTrack.CurrentVolume, targetVolume, AudioManager.C_TrackTransitionSpeed * Time.deltaTime);
                if (audioTrack != ActiveTrack && audioTrack.CurrentVolume == 0)
                {
                    DestoryTrack(audioTrack);
                }
            }
            yield return null;
        }
        Co_levelingVolume = null;
    }
    private void DestoryTrack(AudioTrack audioTrack)
    {
        if (AudioTracks.Contains(audioTrack))
        {
            AudioTracks.Remove(audioTrack);
        }
        Object.Destroy(audioTrack.Root);
    }
    private void ActivateTrack(AudioTrack audioTrack)
    {
        if (!AudioTracks.Contains(audioTrack))
        {
            AudioTracks.Add(audioTrack);
        }
        ActiveTrack = audioTrack;
        TryLevelingVolume();
    }
    #endregion
}
