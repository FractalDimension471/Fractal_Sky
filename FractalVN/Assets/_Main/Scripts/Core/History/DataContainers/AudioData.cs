using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HISTORY
{
    [System.Serializable]
    public class AudioData
    {
        #region  Ù–‘/Property
        [field: SerializeField]
        public int ChannelNumber {  get; set; }
        [field: SerializeField]
        public TrackData Track {  get; set; }

        [System.Serializable]
        public class TrackData
        {
            [field: SerializeField]
            public string Name { get; set; }
            [field: SerializeField]
            public string Path { get; set; }
            [field: SerializeField]
            public float Volume { get; set; }
            [field: SerializeField]
            public float Pitch { get; set; }
            [field: SerializeField]
            public bool Loop { get; set; }
        }
        #endregion
        #region ∑Ω∑®/Method
        public AudioData(AudioChannel channel)
        {
            if (channel.ActiveTrack == null)
            {
                return;
            }
            ChannelNumber = channel.ChannelIndex;
            var currentTrack = channel.ActiveTrack;

            Track = new()
            {
                Name = currentTrack.Name,
                Path = currentTrack.Path,
                Volume = currentTrack.CapVolume,
                Pitch = currentTrack.Pitch,
                Loop = currentTrack.Loop
            };
        }

        public static List<AudioData> Capture()
        {
            List<AudioData> datas = new();
            foreach(var channel in AudioManager.Instance.Channels)
            {
                if(channel.Value.ActiveTrack == null)
                {
                    continue;
                }
                AudioData data = new(channel.Value);
                datas.Add(data);
            }
            return datas;
        }
        public static void Apply(List<AudioData> datas)
        {
            List<int> cache = new();
            foreach(var data in datas)
            {
                AudioChannel channel = AudioManager.Instance.GetChannel(data.ChannelNumber, true);
                if(channel.ActiveTrack == null || channel.ActiveTrack.Name != data.Track.Name)
                {
                    AudioClip audio = HistoryCache.LoadAudio(data.Track.Path);
                    if(audio != null)
                    {
                        channel.StopTrack(true);
                        channel.PlayTrack(audio, data.Track.Loop, data.Track.Volume, data.Track.Volume, data.Track.Pitch, data.Track.Path);
                    }
                    else
                    {
                        Debug.LogWarning($"Cannot load audio track '{data.Track.Path}'");
                    }
                }
                cache.Add(data.ChannelNumber);
            }
            foreach(var channel in AudioManager.Instance.Channels)
            {
                if (!cache.Contains(channel.Value.ChannelIndex))
                {
                    channel.Value.StopTrack(true);
                }
            }
        }
        #endregion
    }
}