using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SLNavigationBar : MonoBehaviour
{
    private static int MaxPageCount { get; } = 6;
    [field: SerializeField]
    public SLPage Page { get; private set; }
    [field: SerializeField]
    public GameObject ButtonPrefab { get; private set; }
    [field: SerializeField]
    public TextMeshProUGUI PageIndex { get; private set; }
    private bool Initialized { get; set; } = false;
    public int SelectedPageIndex { get; internal set; } = 1;
    private int MaxPages { get; set; } = 0;
    // Start is called before the first frame update
    void Start()
    {
        Initialize();
        UpdatePageIndex(1);
    }
    private void Initialize()
    {
        if (Initialized)
        {
            return;
        }
        Initialized = true;
        MaxPages = Mathf.CeilToInt((float)SLPage.MaxSlotIndex / Page.SlotsPerPage);//Àƒ…·ŒÂ»Î
        int maxPageIndex = MaxPageCount < MaxPages ? MaxPageCount + 1 : MaxPages + 1;
        for (int i = 1; i < maxPageIndex; Interlocked.Increment(ref i))
        {
            GameObject newButtonObject = Instantiate(ButtonPrefab, ButtonPrefab.transform.parent);
            newButtonObject.SetActive(true);
            Button newButton = newButtonObject.GetComponent<Button>();
            newButtonObject.name = i.ToString();

            TextMeshProUGUI title = newButtonObject.GetComponentInChildren<TextMeshProUGUI>();
            title.text = i.ToString();

            int selectedPageIndex = i;
            newButton.onClick.AddListener(() => OnPageButtonSelected(selectedPageIndex));
        }
    }
    private void OnPageButtonSelected(int pageIndex)
    {
        SelectedPageIndex = pageIndex;
        Page.GenerateSlots(pageIndex);
        UpdatePageIndex(pageIndex);
    }
    private void UpdatePageIndex(int index)
    {
        PageIndex.text = $"{index}/{MaxPageCount}";
    }
}
