using System;
using System.Diagnostics;

namespace AYellowpaper.SerializedCollections
{
    [Conditional("UNITY_EDITOR")]
    public class SerializedDictionaryAttribute : Attribute
    {
        public readonly string KeyName;
        public readonly string ValueName;

        public SerializedDictionaryAttribute(string keyName = null, string valueName = null)
        {
            KeyName = keyName;
            ValueName = valueName;
        }
    }
}