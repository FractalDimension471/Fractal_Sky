using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
//using Unity.VisualScripting;
public class GraphicObject
{

    #region 属性/Property
    GraphicPanelManager PanelManager => GraphicPanelManager.Instance;

    public RawImage Renderer { get; private set; }
    public VideoPlayer VideoPlayer { get; private set; }
    public AudioSource AudioSource { get; private set; }
    private GraphicLayer Layer { get; }
    private Coroutine Co_fadingIn { get; set; }
    private Coroutine Co_fadingOut { get; set; }

    public string GraphicPath { get; }
    public string GraphicName { get; }

    private static string[] MaterialRootPaths { get; } =
    {
        "Materials","layerTransitionMaterial"
    };
    private string MaterialPath => Path.Combine(MaterialRootPaths);

    //private const string S_MaterialColor = "_Color";
    private static string ID_MaterialMainTexture { get; } = "_MainTex";
    private static string ID_MaterialBlendTexture { get; } = "_BlendTex";
    private static string ID_MaterialBlend { get; } = "_Blend";
    private static string ID_MaterialAlpha { get; } = "_Alpha";
    private static string ID_DefaultUIMateria { get; } = "Default UI Materia";

    public bool IsVideo => VideoPlayer != null;
    public bool UseAudio => AudioSource != null && !AudioSource.mute;
    #endregion
    #region 方法/Method
    public GraphicObject(GraphicLayer graphicLayer, string graphicPath, Texture texture, bool immediate)
    {
        GraphicPath = graphicPath;
        Layer = graphicLayer;

        GameObject ob = new();
        ob.transform.SetParent(graphicLayer.Panel);
        Renderer = ob.AddComponent<RawImage>();
        GraphicName = texture.name;
        Renderer.name = $"Graphic - [{GraphicName}]";
        InitializeGraphic(immediate);

        Renderer.material.SetTexture(ID_MaterialMainTexture, texture);

    }
    public GraphicObject(GraphicLayer graphicLayer, string graphicPath, VideoClip video, bool useAudio, bool immediate)
    {
        GraphicPath = graphicPath;
        Layer = graphicLayer;
        GameObject ob = new();
        ob.transform.SetParent(graphicLayer.Panel);
        Renderer = ob.AddComponent<RawImage>();
        GraphicName = video.name;
        Renderer.name = $"Graphic - [{GraphicName}]";
        InitializeGraphic(immediate);

        RenderTexture texture = new(Mathf.RoundToInt(video.width), Mathf.RoundToInt(video.height), 0);
        Renderer.material.SetTexture(ID_MaterialMainTexture, texture);
        //播放器设置
        VideoPlayer = Renderer.gameObject.AddComponent<VideoPlayer>();
        AudioSource = Renderer.gameObject.AddComponent<AudioSource>();
        VideoPlayer.SetTargetAudioSource(0, AudioSource);
        //视频设置
        VideoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        VideoPlayer.renderMode = VideoRenderMode.RenderTexture;
        VideoPlayer.source = VideoSource.VideoClip;
        VideoPlayer.clip = video;
        VideoPlayer.targetTexture = texture;
        VideoPlayer.playOnAwake = true;
        VideoPlayer.isLooping = true;

        //音频设置
        AudioSource.volume = immediate ? 1 : 0;
        if (!useAudio)
        {
            AudioSource.mute = true;
        }
        //播放前准备设置
        VideoPlayer.frame = 0;
        VideoPlayer.Prepare();
        VideoPlayer.Play();
    }
    private void InitializeGraphic(bool immediate)
    {
        //2D
        Renderer.transform.localPosition = Vector2.zero;
        Renderer.transform.localScale = Vector2.one;

        RectTransform rectTransform = Renderer.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.one;

        Renderer.material = GetLayerTransitionMaterial();

        float startOpacity = immediate ? 1f : 0f;
        Renderer.material.SetFloat(ID_MaterialBlend, startOpacity);
        Renderer.material.SetFloat(ID_MaterialAlpha, startOpacity);

    }
    private Material GetLayerTransitionMaterial()
    {
        Material material = Resources.Load<Material>(MaterialPath);
        if (material != null)
        {
            return new Material(material);
        }
        return null;
    }
    public Coroutine FadeIn(float blendSpeed = 1f, Texture blendTexture = null)
    {
        if (Co_fadingOut != null)
        {
            PanelManager.StopCoroutine(Co_fadingOut);
        }
        if (Co_fadingIn != null)
        {
            return Co_fadingIn;
        }
        Co_fadingIn = PanelManager.StartCoroutine(Fading(1f, blendSpeed, blendTexture));
        return Co_fadingIn;
    }
    public Coroutine FadeOut(float blendSpeed = 1f, Texture blendTexture = null)
    {
        if (Co_fadingIn != null)
        {
            PanelManager.StopCoroutine(Co_fadingIn);
        }
        if (Co_fadingOut != null)
        {
            return Co_fadingOut;
        }
        Co_fadingOut = PanelManager.StartCoroutine(Fading(0f, blendSpeed, blendTexture));
        return Co_fadingOut;
    }
    private IEnumerator Fading(float target, float speed, Texture texture)
    {
        if (Renderer.material.name == ID_DefaultUIMateria)
        {
            Texture currentTexture = Renderer.material.GetTexture(ID_MaterialMainTexture);
            Renderer.material = GetLayerTransitionMaterial();
            Renderer.material.SetTexture(ID_MaterialMainTexture, currentTexture);
        }
        Renderer.material.SetTexture(ID_MaterialBlendTexture, texture);
        Renderer.material.SetFloat(ID_MaterialAlpha, GetInitialAlpha(target, texture));
        Renderer.material.SetFloat(ID_MaterialBlend, GetInitialBlend(target, texture));
        string opacityParam = texture == null ? ID_MaterialAlpha : ID_MaterialBlend;
        while (Renderer.material.GetFloat(opacityParam) != target)
        {
            float opacity = Mathf.MoveTowards(Renderer.material.GetFloat(opacityParam), target, speed * Time.deltaTime);
            Renderer.material.SetFloat(opacityParam, opacity);
            if (IsVideo)
            {
                AudioSource.volume = opacity;
            }
            yield return null;
        }
        Co_fadingIn = null;
        Co_fadingOut = null;

        if (target == 0)
        {
            Destory();
        }
        else if (Layer.CurrentGraphic != null)
        {
            DestoryBackgroundGraphics();
            Renderer.texture = Renderer.material.GetTexture(ID_MaterialMainTexture);
        }
    }
    private float GetInitialAlpha(float target, Texture texture)
    {
        if (texture != null)
        {
            return 1;
        }
        else if (target > 0)
        {
            return 0;
        }
        else
        {
            return 1;
        }
    }
    private float GetInitialBlend(float target, Texture texture)
    {
        if (texture != null)
        {
            if (target > 0)
            {
                return 0;
            }
            return 1;
        }
        else
        {
            return 1;
        }
    }
    public void Destory()
    {
        if (Layer.CurrentGraphic != null && Layer.CurrentGraphic.Renderer == Renderer)
        {
            Layer.CurrentGraphic = null;
        }
        if (Layer.OldGraphics.Contains(this))
        {
            Layer.OldGraphics.Remove(this);
        }
        Object.Destroy(Renderer.gameObject);
    }
    private void DestoryBackgroundGraphics()
    {
        Layer.DestoryOldGraphics();
    }
    #endregion

}
