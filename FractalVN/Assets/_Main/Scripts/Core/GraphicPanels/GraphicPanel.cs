using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class GraphicPanel
{
    #region 属性/Property
    [SerializeField]
    private string _PanelName = "";
    public string PanelName=> _PanelName;
    public bool IsClear => GraphicLayers == null || GraphicLayers.All(l => l.CurrentGraphic == null);
    [SerializeField]
    private GameObject _Root = null;
    public GameObject Root => _Root;
    public List<GraphicLayer> GraphicLayers { get; } = new();
    #endregion
    #region 方法/Method
    /// <summary>
    /// 获取图层
    /// </summary>
    /// <param name="layerDepth"></param>
    /// <returns></returns>
    public GraphicLayer GetGraphicLayer(int layerDepth, bool createIfNotExist = false)
    {
        foreach(var layer in GraphicLayers)
        {
            if (layer.LayerDepth == layerDepth)
            {
                return layer;
            }
        }
        if (createIfNotExist)
        {
            return CreateGraphicLayer(layerDepth);
        }
        return null;
    }
    public GraphicLayer CreateGraphicLayer(int layerDepth)
    {
        GraphicLayer graphicLayer = new();
        //构造函数的初始化内容
        //GameObject panel = new GameObject(string.Format(GraphicLayer.S_ObjectNameFormat,layerDepth));//一种字符串插值方法
        GameObject panel = new($"Layer {layerDepth}");
        RectTransform rectTransform = panel.AddComponent<RectTransform>();
        panel.AddComponent<CanvasGroup>();
        panel.transform.SetParent(Root.transform, false);

        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.one;

        graphicLayer.Panel = panel.transform;
        graphicLayer.LayerDepth = layerDepth;

        int index = GraphicLayers.FindIndex(l => l.LayerDepth > layerDepth);
        if (index == -1)
        {
            GraphicLayers.Add(graphicLayer);
        }
        else
        {
            GraphicLayers.Insert(index, graphicLayer);
        }

        foreach(var layer in GraphicLayers)
        {
            layer.Panel.SetSiblingIndex(layer.LayerDepth);
        }
        return graphicLayer;
    }
    public void Clear(float blendSpeed = 1, Texture blendTexture = null, bool immediate = false)
    {
        foreach(GraphicLayer layer in GraphicLayers)
        {
            layer.Clear(blendSpeed, blendTexture, immediate);
        }
    }

    #endregion
}
