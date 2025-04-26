using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ElementBlockManager : MonoBehaviour
{
    public static ElementBlockManager Instance { get; private set; }
    [field: SerializeField]
    public List<ElementBlock> Elements { get; internal set; }
    public Stack<ElementBlock> DeletedElements { get; internal set; }
    public Stack<List<(int, int)>> DeletedBlockedElements { get; internal set; }
    public Stack<List<(int, int)>> DeletedOverlappedElements { get; internal set; }
    public Plate ActivePalte => Plate.Instance;
    public Reactor ActiveReactor => Reactor.Instance;
    public bool GameStarted { get; private set; } = false;
    public event Action<int> ClickElement;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        Initialize();
    }
    private void Initialize()
    {
        ClickElement += ClickingElement;
        DeletedElements = new();
        DeletedBlockedElements = new();
        DeletedOverlappedElements = new();
    }
    private void ClickingElement(int id)
    {
        if (GameStarted == false)
        {
            GameStarted = true;
        }
        var eb = ActivePalte.Elements.First(e => e.ID == id);

        if (eb.BlockedElements.Count < 2 && eb.OverlappedElements.Count < 1)
        {
            eb.Hide();
            DeletedElements.Push(eb);
            ActiveReactor.InsertElementBlock(eb);
            Elements.Remove(Elements.First(e => e.ID == id));
            DeleteBlockedAndOverlappedElementsByID(id);
        }
    }
    public void BackTrack()
    {
        var lastEB = DeletedElements.Pop();
        Elements.Add(lastEB);
        ActivePalte.Elements.First(e => e.ID == lastEB.ID).Show();
        ApplyBlockedElements(DeletedBlockedElements.Pop());
        ApplyOverlappedElements(DeletedOverlappedElements.Pop());
        var lastREB = ActiveReactor.Elements.First(e => e.ID == lastEB.ID);
        Destroy(lastREB.Root);
        ActiveReactor.Elements.Remove(lastREB);
    }
    public void UseMagnet()
    {
        if (!GameStarted)
        {
            GameStarted = true;
        }
        var currentID = Elements[UnityEngine.Random.Range(0, Elements.Count - 1)].ID;
        var currentEB = ActivePalte.Elements.First(e => e.ID == currentID);
        currentEB.Hide();
        Elements.Remove(Elements.First(e => e.ID == currentEB.ID));
        DeleteBlockedAndOverlappedElementsByID(currentEB.ID);
        var matchedEB = ActivePalte.Elements.First(e => e.Featrue == currentEB.Featrue && e.CG.interactable == true);
        matchedEB.Hide();
        Elements.Remove(Elements.First(e => e.ID == matchedEB.ID));
        DeleteBlockedAndOverlappedElementsByID(matchedEB.ID);
    }
    private void ApplyBlockedElements(List<(int,int)> data)
    {
        foreach(var d in data)
        {
            var currentEB = ActivePalte.Elements.First(e => e.ID == d.Item1);
            currentEB.BlockedElements.Add(d.Item2);
        }
    }
    private void ApplyOverlappedElements(List<(int, int)> data)
    {
        foreach(var d in data)
        {
            var currentEB = ActivePalte.Elements.First(e => e.ID == d.Item1);
            currentEB.OverlappedElements.Add(d.Item2);
        }
    }
    private void DeleteBlockedAndOverlappedElementsByID(int id)
    {
        var deletedBList = new List<(int, int)>();
        var deletedOlist = new List<(int, int)>();
        foreach (var e in ActivePalte.Elements)
        {
            if (e.BlockedElements.Contains(id))
            {
                deletedBList.Add((e.ID, id));
                e.BlockedElements.Remove(id);
            }
            if (e.OverlappedElements.Contains(id))
            {
                deletedOlist.Add((e.ID, id));
                e.OverlappedElements.Remove(id);
            }
        }
        DeletedBlockedElements.Push(deletedBList);
        DeletedOverlappedElements.Push(deletedOlist);
    }
    public void OnElementBlockClicked(int id)
    {
        ClickElement?.Invoke(id);
    }
    public ElementBlock CreateElementBlock(int id, int feature, GameObject root, Transform parent)
    {
        var newEB = new ElementBlock(id, feature, root, parent);
        Elements.Add(newEB);
        return newEB;
    }
}
