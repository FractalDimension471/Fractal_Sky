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
            "���ǵ�һ��",
            "���������ǽ����в���"
        };
        yield return DialogueSystem.Instance.Say(lines);
    }
    private void Update()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            lines = new()
            {
                "���������λ���˵���㰴����Q��",
                "End Q"
            };
            conversation = new(lines);
            DialogueSystem.Instance.ConversationManager.Enqueue(conversation);
        }
        if (Input.GetKey(KeyCode.E))
        {
            lines = new()
            {
                "���������λ���˵���㰴����E��",
                "End E"
            };
            conversation = new(lines);
            DialogueSystem.Instance.ConversationManager.EnqueuePriority(conversation);
        }
    }
}
