using TMPro;
using UnityEngine;
namespace DIALOGUE
{

    [System.Serializable]

    public class NameContainer
    {
        #region ����/Property
        [SerializeField]
        private GameObject _root;
        private GameObject Root => _root;

        [field: SerializeField]
        public TextMeshProUGUI NameText { get; private set; }
        #endregion
        #region ����/Method
        /// <summary>
        /// �ɼ�������
        /// </summary>
        /// <param name="isvisible"></param>
        /// <param name="nameToShow"></param>
        public void SetVisibility(bool isvisible = true, string nameToShow = "")
        {
            if (isvisible)
            {
                Root.SetActive(true);
                if (nameToShow != string.Empty)
                {
                    NameText.text = nameToShow;
                }
            }
            else
            {
                Root.SetActive(false);
            }
        }
        /// <summary>
        /// ����������ɫ
        /// </summary>
        /// <param name="color"></param>
        public void SetNameColor(Color color) => NameText.color = color;
        /// <summary>
        /// ������������
        /// </summary>
        /// <param name="font"></param>
        public void SetNameFont(TMP_FontAsset font) => NameText.font = font;
        public void SetNameFontSize(float fontSize) => NameText.fontSize = fontSize;
        #endregion
    }
}

