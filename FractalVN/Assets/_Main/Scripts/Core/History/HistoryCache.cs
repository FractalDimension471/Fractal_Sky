using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

public class HistoryCache
{
    public static Dictionary<string, (object asset, int staleIndex)> LoadedAssets { get; } = new();

    public static bool TryLoadObject<T>(string path, out T result)
    {
        object asset;
        if (LoadedAssets.ContainsKey(path))
        {
            asset = (T)LoadedAssets[path].asset;
        }
        else
        {
            asset = Resources.Load(path);
            if (asset != null)
            {
                LoadedAssets[path] = (asset, 0);
            }
        }

        if (asset != null)
        {
            if (asset is T)
            {
                result = (T)asset;
                return true;
            }
            else
            {
                Debug.LogWarning($"Object '{path}' was not the expected type!");
            }
        }
        result = default;
        return false;
    }
    public static TMP_FontAsset LoadFont(string path)
    {
        if (!TryLoadObject(path, out TMP_FontAsset result))
        {
            Debug.LogWarning($"Cannot load font '{path}'");
            return null;
        }
        return result;
    }
    public static AudioClip LoadAudio(string path)
    {
        if (!TryLoadObject(path, out AudioClip result))
        {
            Debug.LogWarning($"Cannot load audio '{path}'");
            return null;
        }
        return result;
    }
    public static Texture LoadImage(string path)
    {
        if (!TryLoadObject(path, out Texture result))
        {
            Debug.LogWarning($"Cannot load image '{path}'");
            return null;
        }
        return result;
    }
    public static VideoClip LoadVideo(string path)
    {
        if (!TryLoadObject(path, out VideoClip result))
        {
            Debug.LogWarning($"Cannot load video'{path}'");
            return null;
        }
        return result;
    }
}
