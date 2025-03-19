using TMPro;
using UnityEngine;
namespace DIALOGUE
{

    [System.Serializable]

    public class NameContainer
    {
        #region 属性/Property
        [SerializeField]
        private GameObject _root;
        private GameObject Root => _root;

        [field: SerializeField]
        public TextMeshProUGUI NameText { get; private set; }
        #endregion
        #region 方法/Method
        /// <summary>
        /// 可见性设置
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
        /// 设置名称颜色
        /// </summary>
        /// <param name="color"></param>
        public void SetNameColor(Color color) => NameText.color = color;
        /// <summary>
        /// 设置名称字体
        /// </summary>
        /// <param name="font"></param>
        public void SetNameFont(TMP_FontAsset font) => NameText.font = font;
        public void SetNameFontSize(float fontSize) => NameText.fontSize = fontSize;
        #endregion
    }
}

