using System.Collections.Generic;
using UnityEngine;

public class GraphicPanelManager : MonoBehaviour
{
    #region 属性/Property
    public static GraphicPanelManager Instance { get; private set; }
    public float DefaultSpeed = 1;
    [SerializeField]
    private List<GraphicPanel> _AllPanels = new();
    public List<GraphicPanel> AllPanels => _AllPanels;
    #endregion
    #region 方法/Method
    private void Awake()
    {
        Instance = this;
    }
    /// <summary>
    /// 获取图形面板
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public GraphicPanel GetGraphicPanel(string name)
    {
        name = name.ToLower();
        foreach (GraphicPanel graphicPanel in AllPanels)
        {
            if (graphicPanel.PanelName.ToLower() == name)
            {
                return graphicPanel;
            }
        }
        return null;
    }
    #endregion

}
