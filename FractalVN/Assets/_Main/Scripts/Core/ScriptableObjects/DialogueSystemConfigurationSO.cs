using CHARACTERS;
using TMPro;
using UnityEngine;
namespace DIALOGUE
{
    /// <summary>
    /// 可视化对话系统设置
    /// </summary>
    [CreateAssetMenu(fileName = "Dialogue System Configuration", menuName = "Dialogue System/Dialogue Configuration Asset")]
    public class DialogueSystemConfigurationSO : ScriptableObject
    {
        //CharacterConfigSO是DialogueSystemConfigurationSO的字段
        public CharacterConfigSO characterConfigurationAsset;
        //定义默认值
        public Color defaultTextColor = Color.white;
        public TMP_FontAsset defaultFont;

        public float defaultNameFontSize = 40f;
        public float defaultDialogueFontSize = 30f;
        public float dialogueFontSizeScale = 1f;
    }
}