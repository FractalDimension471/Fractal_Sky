using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HistoryLog
{
    public GameObject Root {  get; internal set; }
    public TextMeshProUGUI NameText { get; internal set; }
    public TextMeshProUGUI DialogueText { get; internal set; }
    public float NameFontSize { get; internal set; } = 18f;
    public float DialogueFontSize { get; internal set; } = 18f;
}
