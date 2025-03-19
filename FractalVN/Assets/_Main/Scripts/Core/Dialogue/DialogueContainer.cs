using TMPro;
using UnityEngine;
namespace DIALOGUE
{
    //确保Unity内的属性列表可见
    [System.Serializable]
    //定义对话框
    public class DialogueContainer
    {
        #region 属性/Property
        //声明对话框包含的内容
        [SerializeField]
        private GameObject _RootContainer;
        public GameObject RootContainer => _RootContainer;

        [SerializeField]
        private TextMeshProUGUI _DialogueText;
        public TextMeshProUGUI DialogueText => _DialogueText;
        private CanvasGroupController CgController { get; set; }

        private bool Initialized { get; set; } = false;
        public bool IsVisible => CgController.IsVisible;
        #endregion
        #region 方法/Method
        public void Initialize()
        {
            if (Initialized)
            {
                return;
            }
            CgController = new(DialogueSystem.Instance, RootContainer.GetComponent<CanvasGroup>());
            Initialized = true;
        }
        //用于设定属性的相关函数
        public void SetDialogueColor(Color color) => DialogueText.color = color;
        public void SetDialogueFont(TMP_FontAsset font) => DialogueText.font = font;
        public void SetDialogueFontSize(float fontSize) => DialogueText.fontSize = fontSize;

        public Coroutine Show(float speedMultiplier = 1f, bool immediate = false) => CgController.Show();
        public Coroutine Hide(float speedMultiplier = 1f, bool immediate = false) => CgController.Hide();
        #endregion
    }
}

