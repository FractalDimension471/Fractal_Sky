using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class InputPanel : MonoBehaviour
{
    #region  Ù–‘/Property
    public static InputPanel Instance { get; private set; }
    private CanvasGroupController CgController { get; set; }
    [SerializeField] private CanvasGroup _PanelCG;
    [SerializeField] private TMP_InputField _InputField;
    [SerializeField] private TMP_Text _TitleText;
    [SerializeField] private Button _AcceptButton;
    public string LastInput { get; private set; }
    public bool IsWaitingOnUserInput { get; set; }
    #endregion
    #region ∑Ω∑®/Method
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        CgController = new(this, _PanelCG);
        Initialize();
    }
    public void Initialize()
    {
        CgController.Alpha = 0f;
        CgController.SetCanvasStatus(false);

        _AcceptButton.gameObject.SetActive(false);
        _AcceptButton.onClick.AddListener(OnAcceptInput);
        _InputField.onValueChanged.AddListener(OnInputChanged);
    }
    public void Show(string title)
    {
        IsWaitingOnUserInput = true;

        _TitleText.text = title;
        _InputField.text = string.Empty;
        CgController.Show();
        CgController.SetCanvasStatus(true);
    }
    public void Hide()
    {
        CgController.Hide();
        CgController.SetCanvasStatus(false);

        IsWaitingOnUserInput = false;
    }
    public void OnAcceptInput()
    {
        if (_InputField.text == string.Empty)
        {
            return;
        }
        LastInput = _InputField.text;
        Hide();
    }
    public void OnInputChanged(string _str)
    {
        _AcceptButton.gameObject.SetActive(HasValidText());
    }
    private bool HasValidText()
    {
        return _InputField.text != string.Empty;
    }
    #endregion
}
