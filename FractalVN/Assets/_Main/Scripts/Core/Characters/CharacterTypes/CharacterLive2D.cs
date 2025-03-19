using UnityEngine;

namespace CHARACTERS
{
    public class CharacterLive2D : Character
    {
        //构造函数是一个单独类别，继承的时候需要单独处理
        public CharacterLive2D(string name, CharacterConfigData configData, GameObject prefab, string rootAssetsFolder) : base(name, configData, prefab)
        {

        }
    }
}