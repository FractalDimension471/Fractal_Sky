using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

public class Reactor : MonoBehaviour
{
    public static Reactor Instance { get; private set; }
    [field: SerializeField]
    public List<ElementBlock> Elements { get; internal set; }
    [field: SerializeField]
    public GameObject Prefab { get; internal set; }
    public bool IsResting { get; private set; } = false;
    private void Awake()
    {
        Instance = this;
    }
    public void InsertElementBlock(ElementBlock eb)
    {
        foreach (var e in Elements)
        {
            if (eb.Featrue == e.Featrue)
            {
                DestroyImmediate(e.Root);
                Elements.Remove(e);
                return;
            }
        }
        Elements.Add(eb);
        var ob = GameObject.Instantiate(Prefab, transform);
        ob.SetActive(true);
        ob.name = eb.ID.ToString();
        ob.GetComponentInChildren<TextMeshProUGUI>().text = ESSystem.Instance.ElementSO.GetElementConfigData(eb.Featrue).Name;
        eb.Root = ob;

    }
    public void ResetReactor()
    {
        for(int i = 0; i < transform.childCount; Interlocked.Increment(ref i))
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        Elements = new();
    }
}
