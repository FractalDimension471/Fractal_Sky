using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
public class PlateLevel : MonoBehaviour
{
    public bool Initialized { get; private set; } = false;
    private int LastFeature { get; set; } = 0;
    private bool LastFeatureUsed { get; set; } = false;
    private List<int> UsedFeature { get; set; } = new();
    [field: SerializeField]
    public List<ElementBlock> Elements { get; private set; }
    private void Start()
    {
        Initialize();
    }
    private void Initialize()
    {
        var rawElements = Elements.Select(e => e).ToList();
        var preElements = new List<ElementBlock>();
        while (rawElements.Count > 0)
        {
            var i = Random.Range(0, rawElements.Count - 1);
            var e = rawElements[i];
            if (!preElements.Contains(e))
            {
                preElements.Add(e);
                rawElements.Remove(e);
            }
        }
        foreach (var e in preElements)
        {
            var newFeature = Random.Range(1, ESSystem.Instance.ElementSO.Elements.Count());
            var targetFeature = 0;
            if (LastFeature == 0)
            {
                LastFeature = newFeature;
                targetFeature = newFeature;
            }
            else
            {
                if (!LastFeatureUsed)
                {
                    targetFeature = LastFeature;
                    LastFeatureUsed = true;
                }
                else
                {
                    LastFeature = newFeature;
                    targetFeature = newFeature;
                    LastFeatureUsed = false;
                }
            }
            var newEB = ElementBlockManager.Instance.CreateElementBlock(e.ID, targetFeature, e.Root, transform);
            e.Featrue = newEB.Featrue;
            e.Root = newEB.Root;
            e.CG = newEB.CG;
            e.E_Button = newEB.E_Button;
        }
        Initialized = true;
    }
    private void GetAllElements()
    {
        var count = transform.childCount;
        for (int i = 0; i < count; Interlocked.Increment(ref i))
        {
            var elementOb = transform.GetChild(i).gameObject;
            var newFeature = Random.Range(1, count);
            var targetFeature = 0;
            if (LastFeature == 0)
            {
                LastFeature = newFeature;
                targetFeature = newFeature;
            }
            else
            {
                if (!LastFeatureUsed)
                {
                    targetFeature = LastFeature;
                    LastFeatureUsed = true;
                }
                else
                {
                    LastFeature = newFeature;
                    targetFeature = newFeature;
                    LastFeatureUsed = false;
                }
            }
            var newEB = ElementBlockManager.Instance.CreateElementBlock(i, targetFeature, elementOb, transform);
        }
    }
}
