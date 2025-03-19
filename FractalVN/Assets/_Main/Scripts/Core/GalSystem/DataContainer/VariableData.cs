using System;
using UnityEngine;

namespace GALGAME
{
    [Serializable]
    public class VariableData
    {
        [field: SerializeField]
        public string Name { get; set; } = "";
        [field: SerializeField]
        public string Value { get; set; } = "";
        [field: SerializeField]
        public string Type { get; set; } = "";
    }
}