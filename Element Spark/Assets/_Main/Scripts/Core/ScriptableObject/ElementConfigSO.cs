using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = "Element Configuration Asset", menuName = "Configuration/Element Configuration Asset")]
public class ElementConfigSO : ScriptableObject
{
    public ElementConfigData[] Elements;
    public ElementConfigData GetElementConfigData(int feature) => Elements.First(e => e.Feature == feature);
}
[System.Serializable]
public class ElementConfigData
{
    [field: SerializeField]
    public string Name { get; internal set; }
    [field: SerializeField]
    public int Feature { get; internal set; }
    [field: SerializeField]
    public Color SparkColor { get; internal set; }
}
