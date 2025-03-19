using TMPro;
using UnityEngine;
namespace DIALOGUE
{
    //ȷ��Unity�ڵ������б�ɼ�
    [System.Serializable]
    //����Ի���
    public class DialogueContainer
    {
        #region ����/Property
        //�����Ի������������
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
        #region ����/Method
        public void Initialize()
        {
            if (Initialized)
            {
                return;
            }
            CgController = new(DialogueSystem.Instance, RootContainer.GetComponent<CanvasGroup>());
            Initialized = true;
        }
        //�����趨���Ե���غ���
        public void SetDialogueColor(Color color) => DialogueText.color = color;
        public void SetDialogueFont(TMP_FontAsset font) => DialogueText.font = font;
        public void SetDialogueFontSize(float fontSize) => DialogueText.fontSize = fontSize;

        public Coroutine Show(float speedMultiplier = 1f, bool immediate = false) => CgController.Show();
        public Coroutine Hide(float speedMultiplier = 1f, bool immediate = false) => CgController.Hide();
        #endregion
    }
}

