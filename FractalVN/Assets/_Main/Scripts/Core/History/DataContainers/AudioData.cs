using System.Collections.Generic;
using UnityEngine;

namespace HISTORY
{
    [System.Serializable]
    public class AudioData
    {
        #region  Ù–‘/Property

        [field: SerializeField]
        public List<TrackData> Tracks { get; set; }
        [field: SerializeField]
        public List<SoundData> Sounds { get; set; }

        [System.Serializable]
        public class TrackData
        {
            [field: SerializeField]
            public int ChannelNumber { get; set; }
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
        [System.Serializable]
        public class SoundData
        {
            [field: SerializeField]
            public string Path { get; set; }
            [field: SerializeField]
            public float Volume { get; set; }
            [field: SerializeField]
            public float Pitch { get; set; }
        }
        #endregion
        #region ∑Ω∑®/Method
        private static List<TrackData> GetTrackDatas()
        {
            var targetTracks = new List<TrackData>();
            foreach (var channel in AudioManager.Instance.Channels)
            {
                var track = channel.Value.ActiveTrack;
                if (track == null)
                {
                    continue;
                }
                TrackData data = new()
                {
                    ChannelNumber = channel.Value.ChannelIndex,
                    Name = track.Name,
                    Path = track.Path,
                    Pitch = track.Pitch,
                    Volume = track.CapVolume,
                    Loop = track.Loop
                };
                targetTracks.Add(data);
            }
            return targetTracks;
        }
        private static List<SoundData> GetSoundDatas()
        {
            var targetSounds = new List<SoundData>();
            var cachedSounds = AudioManager.Instance.CachedSounds;
            foreach (var sound in cachedSounds)
            {
                SoundData data = new()
                {
                    Path = sound.Key,
                    Pitch = sound.Value.pitch,
                    Volume = sound.Value.volume
                };
                targetSounds.Add(data);
            }
            return targetSounds;
        }
        public static AudioData Capture()
        {
            var data = new AudioData
            {
                Tracks = GetTrackDatas(),
                Sounds = GetSoundDatas()
            };
            return data;
        }
        public static void Apply(AudioData data)
        {
            ApplyTracks(data.Tracks);
            ApplySounds(data.Sounds);
        }
        private static void ApplyTracks(List<TrackData> datas)
        {
            List<int> cache = new();
            foreach (var trackData in datas)
            {
                AudioChannel channel = AudioManager.Instance.GetChannel(trackData.ChannelNumber, true);
                if (channel.ActiveTrack == null || channel.ActiveTrack.Name != trackData.Name)
                {
                    AudioClip audio = HistoryCache.LoadAudio(trackData.Path);
                    if (audio != null)
                    {
                        channel.StopTrack(true);
                        channel.PlayTrack(audio, trackData.Loop, trackData.Volume, trackData.Volume, trackData.Pitch, trackData.Path);
                    }
                    else
                    {
                        Debug.LogWarning($"Cannot load audio track '{trackData.Path}'");
                    }
                }
                cache.Add(trackData.ChannelNumber);
            }
            foreach (var channel in AudioManager.Instance.Channels)
            {
                if (!cache.Contains(channel.Value.ChannelIndex))
                {
                    channel.Value.StopTrack(true);
                }
            }
        }
        private static void ApplySounds(List<SoundData> datas)
        {
            foreach (var soundData in datas)
            {
                AudioManager.Instance.PlaySound(filePath: soundData.Path, volume: soundData.Volume, pitch: soundData.Pitch, loop: true);
            }
        }
        #endregion
    }
}