using DIALOGUE;
using System.Collections;

public interface ILogicalLine
{
    string KeyWord { get; }
    bool Matches(DialogueLine dialogueLine);
    IEnumerator Excute(DialogueLine dialogueLine);
}
