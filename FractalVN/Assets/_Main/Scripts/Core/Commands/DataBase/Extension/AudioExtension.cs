using System;
using UnityEngine;

namespace COMMANDS
{
    public class AudioExtension : DatabaseExtention
    {
        #region  Ù–‘/Property
        private static string[] ID_Sound { get; } = { "/s", "/sound" };
        private static string[] ID_Voice { get; } = { "/v", "voice" };
        private static string[] ID_Music { get; } = { "/m", "/music" };
        private static string[] ID_Volume { get; } = { "/vol", "/volume" };
        private static string[] ID_Pitch { get; } = { "/p", "pitch" };
        private static string[] ID_Loop { get; } = { "/l", "/loop" };
        private static string[] ID_Channel { get; } = { "/c", "/channel" };
        private static string[] ID_StartVolume { get; } = { "/sv", "/startvolume" };
        private static string[] ID_CapVolume { get; } = { "/cv", "/capvolume" };
        #endregion
        #region ∑Ω∑®/Method
        new public static void Extend(CommandDatabase database)
        {
            database.AddCommand("playsound", new Action<string[]>(PlaySound));
            database.AddCommand("stopsound", new Action<string>(StopSound));

            database.AddCommand("playvoice", new Action<string[]>(PlayVoice));
            database.AddCommand("stopvoice", new Action<string>(StopSound));

            database.AddCommand("playmusic", new Action<string[]>(PlayMusic));
            database.AddCommand("stopmusic", new Action<string>(StopMusic));
            database.AddCommand("stopallmusic", new Action(StopAllMusic));
        }
        private static string GetAudioPath(string[] rootPaths, string audioName)
        {
            return FilePaths.GetPath(rootPaths, audioName);
        }
        private static void PlaySound(string[] data)
        {
            var parameters = ConvertDataToParameters(data);
            parameters.TryGetValue(ID_Sound, out string soundName);
            parameters.TryGetValue(ID_Volume, out float volume, 1f);
            parameters.TryGetValue(ID_Pitch, out float pitch, 1f);
            parameters.TryGetValue(ID_Loop, out bool loop, false);

            string filePath = GetAudioPath(FilePaths.DefaultSoundPaths, soundName);
            AudioClip sound = Resources.Load<AudioClip>(filePath);
            if (sound == null)
            {
                Debug.LogError($"Can not load sound '{soundName}'!");
                return;
            }
            AudioManager.Instance.PlaySound(sound, volume: volume, pitch: pitch, loop: loop, filePath: filePath);
        }
        private static void StopSound(string data)
        {
            AudioManager.Instance.StopSound(data);
        }
        private static void PlayVoice(string[] data)
        {
            var parameters = ConvertDataToParameters(data);
            parameters.TryGetValue(ID_Voice, out string voiceName);
            parameters.TryGetValue(ID_Volume, out float volume, 1f);
            parameters.TryGetValue(ID_Pitch, out float pitch, 1f);
            parameters.TryGetValue(ID_Loop, out bool loop, false);

            AudioClip voice = Resources.Load<AudioClip>(GetAudioPath(FilePaths.DefaultVoicePaths, voiceName));
            if (voice == null)
            {
                Debug.LogError($"Can not load voice '{voiceName}'!");
                return;
            }
            AudioManager.Instance.PlayVoice(voice, volume, pitch, loop);
        }
        private static void PlayMusic(string[] data)
        {

            var parameters = ConvertDataToParameters(data);
            parameters.TryGetValue(ID_Music, out string musicName);
            parameters.TryGetValue(ID_Channel, out int channelNumber, 0);
            parameters.TryGetValue(ID_Loop, out bool loop, true);
            //parameters.TryGetValue(ID_Immediate, out bool immediate, false);
            parameters.TryGetValue(ID_StartVolume, out float startVolume, 0f);
            parameters.TryGetValue(ID_CapVolume, out float capVolume, 1f);
            parameters.TryGetValue(ID_Pitch, out float pitch, 1f);
            string filePath = GetAudioPath(FilePaths.DefaultMusicPaths, musicName);
            AudioClip music = Resources.Load<AudioClip>(filePath);
            if (music == null)
            {
                Debug.LogError($"Can not load voice '{musicName}'!");
            }
            AudioManager.Instance.PlayMusic(music, channelNumber, loop, startVolume, capVolume, pitch, filePath);
        }
        private static void StopMusic(string data)
        {
            if (int.TryParse(data, out int channel))
            {
                AudioManager.Instance.StopMusic(channel);
            }
        }
        private static void StopAllMusic()
        {
            AudioManager.Instance.StopMusic();
        }
        #endregion
    }
}