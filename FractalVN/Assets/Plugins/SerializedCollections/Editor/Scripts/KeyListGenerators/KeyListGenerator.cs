using System.Collections;
using UnityEngine;

namespace AYellowpaper.SerializedCollections.KeysGenerators
{
    public abstract class KeyListGenerator : ScriptableObject
    {
        public abstract IEnumerable GetKeys(System.Type type);
    }
}