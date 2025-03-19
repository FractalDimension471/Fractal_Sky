using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class GraphicLayer
{
    #region ÊôÐÔ/Property
    public int LayerDepth { get; internal set; } = 0;
    private float BasicSpeed => GraphicPanelManager.Instance.DefaultSpeed;
    public Transform Panel { get; internal set; }
    public GraphicObject CurrentGraphic { get; internal set; } = null;
    public List<GraphicObject> OldGraphics { get; } = new();
    #endregion
    #region ·½·¨/Method
    public Coroutine SetTexture(string filePath = "", float speedMultiplier = 1, Texture blendTexture = null, bool immediate = false)
    {
        Texture texture = Resources.Load<Texture>(filePath);
        if (texture == null)
        {
            Debug.LogError($"filepath '{filePath}' is invalid.");
            return null;
        }
        return SetTexture(texture, filePath, speedMultiplier * BasicSpeed, blendTexture);
    }
    public Coroutine SetTexture(Texture texture = null, string filePath = "", float speedMultiplier = 1, Texture blendTexture = null, bool immediate = false)
    {
        return CreateGraphic(texture, filePath, speedMultiplier * BasicSpeed, blendTexture, false, immediate);
    }
    public Coroutine SetVideo(string filePath = "", float speedMultiplier = 1, Texture blendTexture = null, bool useAudio = true, bool immediate = false)
    {
        VideoClip video = Resources.Load<VideoClip>(filePath);
        if (video == null)
        {
            Debug.LogError($"filepath '{filePath}' is invalid.");
            return null;
        }
        return SetVideo(video, filePath, speedMultiplier, blendTexture, useAudio);
    }
    public Coroutine SetVideo(VideoClip video = null, string filePath = "", float speedMultiplier = 1, Texture blendTexture = null, bool useAudio = false, bool immediate = false)
    {
        return CreateGraphic(video, filePath, speedMultiplier * BasicSpeed, blendTexture, useAudio, immediate);
    }
    private Coroutine CreateGraphic<T>(T grahpicData, string filePath, float speedMultiplier, Texture blendTexture, bool useAudio, bool immediate)
    {
        GraphicObject newGraphic = null;
        if (grahpicData is Texture)
        {
            newGraphic = new GraphicObject(this, filePath, grahpicData as Texture, immediate);
        }
        else if (grahpicData is VideoClip)
        {
            newGraphic = new GraphicObject(this, filePath, grahpicData as VideoClip, useAudio, immediate);
        }

        if (CurrentGraphic != null && !OldGraphics.Contains(CurrentGraphic))
        {
            OldGraphics.Add(CurrentGraphic);
        }
        CurrentGraphic = newGraphic;
        if (!immediate)
        {
            return CurrentGraphic.FadeIn(speedMultiplier * BasicSpeed, blendTexture);
        }
        DestoryOldGraphics();
        return null;
    }
    public void DestoryOldGraphics()
    {
        foreach (GraphicObject graphic in OldGraphics)
        {
            if (graphic.Renderer != null)
            {
                Object.Destroy(graphic.Renderer.gameObject);
            }
        }
        OldGraphics.Clear();
    }
    public void Clear(float blendSpeed = 1, Texture blendTexture = null, bool immediate = false)
    {
        if (CurrentGraphic != null)
        {
            if (!immediate)
            {
                CurrentGraphic.FadeOut(blendSpeed, blendTexture);
            }
            else
            {
                CurrentGraphic.Destory();
            }
        }
        if (!immediate)
        {
            foreach (GraphicObject graphic in OldGraphics)
            {
                graphic.FadeOut(blendSpeed, blendTexture);
            }
        }
        else
        {
            foreach (GraphicObject graphic in OldGraphics)
            {
                graphic.Destory();
            }
        }
    }
    #endregion

}
