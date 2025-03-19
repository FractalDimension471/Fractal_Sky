using UnityEngine;

public class FunctionPanel : MonoBehaviour
{
    #region ÊôÐÔ/Property
    public static FunctionPanel Instance { get; private set; }
    private CanvasGroupController CgController { get; set; }

    [SerializeField] private GameObject _Root;
    [SerializeField] private CanvasGroup _PanelCG;
    private bool IsVisible => CgController.IsVisible;
    #endregion
    #region ·½·¨/Method
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        CgController = new(this, _PanelCG);
    }
    public Coroutine SetPanelStatus()
    {
        if (IsVisible)
        {
            _Root.SetActive(false);
            return CgController.Hide();
        }
        else
        {
            _Root.SetActive(true);
            return CgController.Show();
        }
    }
    #endregion
}
