using CHARACTERS;
using TMPro;
using UnityEngine;
namespace DIALOGUE
{
    /// <summary>
    /// ���ӻ��Ի�ϵͳ����
    /// </summary>
    [CreateAssetMenu(fileName = "Dialogue System Configuration", menuName = "Dialogue System/Dialogue Configuration Asset")]
    public class DialogueSystemConfigurationSO : ScriptableObject
    {
        //CharacterConfigSO��DialogueSystemConfigurationSO���ֶ�
        public CharacterConfigSO characterConfigurationAsset;
        //����Ĭ��ֵ
        public Color defaultTextColor = Color.white;
        public TMP_FontAsset defaultFont;

        public float defaultNameFontSize = 40f;
        public float defaultDialogueFontSize = 30f;
        public float dialogueFontSizeScale = 1f;
    }
}