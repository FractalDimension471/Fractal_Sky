using DIALOGUE;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestConversationQueue : MonoBehaviour
{
    List<string> lines;
    Conversation conversation;
    private void Start()
    {
        StartCoroutine(Test());
    }
    IEnumerator Test()
    {
        lines = new()
        {
            "这是第一句",
            "接下来我们将进行测试"
        };
        yield return DialogueSystem.Instance.Say(lines);
    }
    private void Update()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            lines = new()
            {
                "如果看到这段话，说明你按下了Q键",
                "End Q"
            };
            conversation = new(lines);
            DialogueSystem.Instance.ConversationManager.Enqueue(conversation);
        }
        if (Input.GetKey(KeyCode.E))
        {
            lines = new()
            {
                "如果看到这段话，说明你按下了E键",
                "End E"
            };
            conversation = new(lines);
            DialogueSystem.Instance.ConversationManager.EnqueuePriority(conversation);
        }
    }
}
