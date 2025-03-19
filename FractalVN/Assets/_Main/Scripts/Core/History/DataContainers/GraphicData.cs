using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace HISTORY
{
    [System.Serializable]
    public class GraphicData
    {
        #region  Ù–‘/Property
        [field: SerializeField]
        public string PanelName { get; set; }
        [field: SerializeField]
        public List<LayerData> LayerDatas { get; set; }

        [System.Serializable]
        public class LayerData
        {
            [field: SerializeField]
            public string GraphicName { get; set; }
            [field: SerializeField]
            public string GraphicPath { get; set; }
            [field: SerializeField]
            public int Depth { get; set; }
            [field: SerializeField]
            public bool IsVedio { get; set; }
            [field: SerializeField]
            public bool UseAudio { get; set; }
        }
        #endregion
        #region ∑Ω∑®/Method
        public GraphicData(GraphicPanel panel)
        {
            LayerDatas = new();
            PanelName = panel.PanelName;
            foreach (var layer in panel.GraphicLayers)
            {
                if (layer.CurrentGraphic == null)
                {
                    return;
                }
                var currentGraphic = layer.CurrentGraphic;
                LayerData layerData = new()
                {
                    Depth = layer.LayerDepth,
                    GraphicName = currentGraphic.GraphicName,
                    GraphicPath = currentGraphic.GraphicPath,
                    IsVedio = currentGraphic.IsVideo,
                    UseAudio = currentGraphic.UseAudio
                };
                LayerDatas.Add(layerData);
            }
        }
        public static List<GraphicData> Capture()
        {
            List<GraphicData> datas = new();
            foreach (var panel in GraphicPanelManager.Instance.AllPanels)
            {
                if (panel.IsClear)
                {
                    continue;
                }
                GraphicData data = new(panel);
                datas.Add(data);
            }
            return datas;
        }
        public static void Apply(List<GraphicData> datas)
        {
            List<string> cache = new();
            foreach (var data in datas)
            {
                var panel = GraphicPanelManager.Instance.GetGraphicPanel(data.PanelName);
                foreach (var layerData in data.LayerDatas)
                {
                    var layer = panel.GetGraphicLayer(layerData.Depth, true);
                    if (layer.CurrentGraphic == null || layer.CurrentGraphic.GraphicName != layerData.GraphicName)
                    {
                        if (!layerData.IsVedio)
                        {
                            Texture texture = HistoryCache.LoadImage(layerData.GraphicPath);
                            if (texture != null)
                            {
                                layer.SetTexture(texture, layerData.GraphicPath, immediate: true);
                            }
                            else
                            {
                                Debug.LogWarning($"Cannot load image '{layerData.GraphicPath}'");
                            }
                        }
                        else
                        {
                            VideoClip video = HistoryCache.LoadVideo(layerData.GraphicPath);
                            if (video != null)
                            {
                                layer.SetVideo(video, layerData.GraphicPath, immediate: true);
                            }
                            else
                            {
                                Debug.LogWarning($"Cannot load video '{layerData.GraphicPath}'");
                            }
                        }
                    }
                }
                cache.Add(panel.PanelName);
            }
            foreach (var panel in GraphicPanelManager.Instance.AllPanels)
            {
                if (!cache.Contains(panel.PanelName))
                {
                    panel.Clear(immediate: true);
                }
            }
        }
        #endregion
    }
}