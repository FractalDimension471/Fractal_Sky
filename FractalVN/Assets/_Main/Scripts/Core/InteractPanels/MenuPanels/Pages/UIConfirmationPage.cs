using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIConfirmationPage : MonoBehaviour
{
    #region ����/Property
    public static UIConfirmationPage Instance { get; private set; }
    [field: SerializeField]
    public CanvasGroup CG { get; private set; }
    [field: SerializeField]
    public TextMeshProUGUI Title { get; private set; }
    [field: SerializeField]
    public LayoutGroup OptionLayout { get; private set; }
    [field: SerializeField]
    public GameObject OptionButtonPrefab { get; private set; }
    public CanvasGroupController CGcontroller { get; private set; }
    public GameObject[] ActiveOptions { get; private set; }

    public class ConfirmationButton
    {
        public string ButtonText { get; private set; }
        public Action TargetAction { get; private set; }
        public bool AutoCloseOnQuit { get; private set; }
        public ConfirmationButton(string text, Action targetAction, bool autoCloseOnQuit = true)
        {
            ButtonText = text;
            TargetAction = targetAction;
            AutoCloseOnQuit = autoCloseOnQuit;
        }
    }
    #endregion
    #region ����/Method
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        ActiveOptions = new GameObject[0];
        CGcontroller = new(this, CG);
    }
    public void Show(string title, params ConfirmationButton[] options)//params�ؼ������ڽ�����������Աת�������������
    {
        if (options.Length == 0)
        {
            Debug.LogError("No option detected in confirmation page.");
            return;
        }
        CreateOptions(options);
        Title.text = title;
        CGcontroller.SetCanvasStatus(true);
        CGcontroller.Show();
    }
    public void Hide()
    {
        CGcontroller.SetCanvasStatus(false);
        CGcontroller.Hide();
    }
    private void CreateOptions(ConfirmationButton[] options)
    {
        foreach (var action in ActiveOptions)
        {
            DestroyImmediate(action);
        }
        ActiveOptions = new GameObject[options.Length];
        int index = 0;
        foreach (var option in options)
        {
            var newButtonObject = Instantiate(OptionButtonPrefab, OptionLayout.transform);
            newButtonObject.SetActive(true);
            //onClickί�йҽӷ���
            if (newButtonObject != null)
            {
                var newButton = newButtonObject.GetComponent<Button>();
                if (option.TargetAction != null)
                {
                    newButton.onClick.AddListener(() => option.TargetAction?.Invoke());
                }

                if (option.AutoCloseOnQuit == true)
                {
                    newButton.onClick.AddListener(() => Hide());
                }
            }
            var buttonText = newButtonObject.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = option.ButtonText;

            ActiveOptions[index] = newButtonObject;
            Interlocked.Increment(ref index);
        }
    }
    #endregion
}
