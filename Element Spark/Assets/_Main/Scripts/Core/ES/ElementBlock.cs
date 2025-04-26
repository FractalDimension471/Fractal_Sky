using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
[System.Serializable]
public class ElementBlock
{

    [field: SerializeField]
    public int ID { get; internal set; }
    [field: SerializeField]
    public GameObject Root { get; internal set; }
    [field: SerializeField]
    public List<int> BlockedElements { get; internal set; }
    [field:SerializeField]
    public List<int> OverlappedElements { get; internal set; }
    public int Featrue { get; internal set; }
    public Button E_Button { get; internal set; }
    public CanvasGroup CG { get; internal set; }

    public ElementBlock(int iD, int featrue, GameObject root, Transform parent)
    {
        ID = iD;
        Featrue = featrue;
        Root = root;

        if (root != null && parent != null)
        {
            CG = root.GetComponent<CanvasGroup>();
            if (featrue == 0)
            {
                Hide();
            }
            root.name = iD.ToString();
            var title = root.GetComponentInChildren<TextMeshProUGUI>();
            title.text = ESSystem.Instance.ElementSO.GetElementConfigData(featrue).Name;
            E_Button = root.GetComponent<Button>();
            E_Button.onClick.AddListener(() => ElementBlockManager.Instance.OnElementBlockClicked(iD));
        }
    }
    public void Show()
    {
        CG.alpha = 1;
        CG.interactable = true;
        CG.blocksRaycasts = true;
    }
    public void Hide()
    {
        CG.alpha = 0;
        CG.interactable = false;
        CG.blocksRaycasts = false;
    }
}
