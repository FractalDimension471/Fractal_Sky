using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace CHARACTERS
{
    /// <summary>
    /// 可视化角色设置
    /// </summary>
    [CreateAssetMenu(fileName = "Character Configuration Asset", menuName="Dialogue System/Character Configuration Asset")]
    public class CharacterConfigSO : ScriptableObject
    {
        #region 属性/Property
        public CharacterConfigData[] characters;
        #endregion
        #region 方法/Method
        /// <summary>
        /// 获取角色设置信息
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
                    //返回副本
                    return safe ? data.Copy() : data;
                }
            }
            return CharacterConfigData.DefaultData;
        }
        #endregion
    }
}