using UnityEngine;

namespace CHARACTERS
{
    /// <summary>
    /// ���ֽ�ɫ��
    /// </summary>
    public class CharacterText : Character
    {
        /// <summary>
        /// �������ֽ�ɫ
        /// </summary>
        /// <param name="name"></param>
        /// <param name="configData"></param>
        public CharacterText(string name, CharacterConfigData configData) : base(name, configData, prefab: null)
        {
            Debug.Log($"Create Text Character: '{name}'");
        }
    }
}