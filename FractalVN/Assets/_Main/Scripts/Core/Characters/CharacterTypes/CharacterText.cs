using UnityEngine;

namespace CHARACTERS
{
    /// <summary>
    /// 文字角色类
    /// </summary>
    public class CharacterText : Character
    {
        /// <summary>
        /// 构建文字角色
        /// </summary>
        /// <param name="name"></param>
        /// <param name="configData"></param>
        public CharacterText(string name, CharacterConfigData configData) : base(name, configData, prefab: null)
        {
            Debug.Log($"Create Text Character: '{name}'");
        }
    }
}