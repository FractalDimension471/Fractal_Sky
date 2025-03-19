using UnityEngine;
[CreateAssetMenu(fileName = "Game Config", menuName = "Dialogue System/Game Config Asset")]
public class GalConfigSO : ScriptableObject
{
    [field: SerializeField]
    public TextAsset StartingFile { get; private set; }
}
