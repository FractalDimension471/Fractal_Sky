using DIALOGUE;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILogicalLine
{
    string KeyWord { get; }
    bool Matches(DialogueLine dialogueLine);
    IEnumerator Excute(DialogueLine dialogueLine);
}
