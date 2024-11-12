using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace CHARACTERS
{
    /// <summary>
    /// ���ӻ���ɫ����
    /// </summary>
    [CreateAssetMenu(fileName = "Character Configuration Asset", menuName="Dialogue System/Character Configuration Asset")]
    public class CharacterConfigSO : ScriptableObject
    {
        #region ����/Property
        public CharacterConfigData[] characters;
        #endregion
        #region ����/Method
        /// <summary>
        /// ��ȡ��ɫ������Ϣ
        /// </summary>
        /// <param name="characterName"></param>
        /// <returns></returns>
        public CharacterConfigData GetCharacterConfigData(string characterName, bool safe = true)
        {
            characterName = characterName.ToLower();
            for(int t = 0; t < characters.Length; Interlocked.Increment(ref t))
            {
                CharacterConfigData data = characters[t];
                if (string.Equals(characterName, data.Name.ToLower())|| string.Equals(characterName, data.Alias.ToLower()))
                {
                    //���ظ���
                    return safe ? data.Copy() : data;
                }
            }
            return CharacterConfigData.DefaultData;
        }
        #endregion
    }
}