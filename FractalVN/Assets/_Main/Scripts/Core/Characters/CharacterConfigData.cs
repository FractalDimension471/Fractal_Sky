using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DIALOGUE;
using AYellowpaper.SerializedCollections;

namespace CHARACTERS
{
    /// <summary>
    /// 角色设置信息
    /// </summary>
    [System.Serializable]
    public class CharacterConfigData
    {
        #region 属性/Property
        //定义角色基础信息
        [field:SerializeField]
        public string Name { get; set; }
        [field: SerializeField]
        public string Alias {  get; set; }
        [field: SerializeField]
        public float NameFontSize {  get; set; }
        [field: SerializeField]
        public float DialogueFontSize {  get; set; }
        [field: SerializeField]
        public Character.CharacterType CharacterType {  get; set; }
        //定义字体颜色
        [field: SerializeField]
        public Color NameColor {  get; set; }
        [field: SerializeField]
        public Color DialogueColor { get; set; }
        //定义字体材质
        [field: SerializeField]
        public TMP_FontAsset NameFont {  get; set; }
        [field: SerializeField]
        public TMP_FontAsset DialogueFont {  get; set; }
        [SerializeField, SerializedDictionary("Path / ID", "Sprite")]
        private SerializedDictionary<string, Sprite> _sprites = new();
        public SerializedDictionary<string, Sprite> Sprites => _sprites;
        //引用系统设置中的默认颜色和字体
        private static Color DefaultColor => DialogueSystem.Instance.Config.defaultTextColor;
        private static TMP_FontAsset DefaultFont => DialogueSystem.Instance.Config.defaultFont;
        //定义属性作为默认值
        public static CharacterConfigData DefaultData
        {
            get
            {
                CharacterConfigData data = new()
                {
                    Name = "",
                    Alias = "",
                    CharacterType = Character.CharacterType.Text,

                    NameColor = new Color(DefaultColor.r, DefaultColor.g, DefaultColor.b, DefaultColor.a),
                    DialogueColor = new Color(DefaultColor.r, DefaultColor.g, DefaultColor.b, DefaultColor.a),

                    NameFont = DefaultFont,
                    DialogueFont = DefaultFont,

                    NameFontSize = DialogueSystem.Instance.Config.defaultNameFontSize,
                    DialogueFontSize = DialogueSystem.Instance.Config.defaultDialogueFontSize
                };
                return data;
            }
        }
        #endregion
        #region 方法/Method
        /// <summary>
        ///定义副本用于数据传输（防止改变原数据）
        /// </summary>
        /// <returns></returns>
        public CharacterConfigData Copy()
        {
            CharacterConfigData data = new()
            {
                Name = Name,
                Alias = Alias,
                CharacterType = CharacterType,

                NameColor = new Color(NameColor.r, NameColor.g, NameColor.b, NameColor.a),
                DialogueColor = new Color(DialogueColor.r, DialogueColor.g, DialogueColor.b, DialogueColor.a),

                NameFont = NameFont,
                DialogueFont = DialogueFont,

                NameFontSize = NameFontSize,
                DialogueFontSize = DialogueFontSize
            };
            return data;
        }
        #endregion
    }
}