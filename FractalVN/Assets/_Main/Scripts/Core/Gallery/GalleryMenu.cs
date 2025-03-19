using System;
using System.IO;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GalleryMenu : MenuPage
{
    //[field: SerializeField]
    ////public CanvasGroup CG { get; private set; }
    //private CanvasGroupController CGcontroller { get; set; }
    [field: SerializeField]
    private CGUnit[] CGs { get; set; }
    [field: SerializeField]
    private Button NaviBarButtonPrefab { get; set; }
    [field: SerializeField]
    private TextMeshProUGUI PageIndex { get; set; }
    [field: SerializeField]
    private CanvasGroup PreviewPageCG { get; set; }
    private CanvasGroupController PreviewPageCGcontroller { get; set; }
    [field: SerializeField]
    private Button PreviewPageImageButton { get; set; }
    [field: SerializeField]
    private Sprite[] GalleryImages { get; set; }
    private int PreviewsPerPage => CGs.Length;
    private int ImageCount => GalleryImages.Length;
    private int PageCount => Mathf.CeilToInt((float)ImageCount / PreviewsPerPage);

    private bool Initialized { get; set; } = false;
    [Serializable]
    class CGUnit
    {
        [field: SerializeField]
        public Button ImageButton { get; private set; }
        [field: SerializeField]
        public TextMeshProUGUI Name { get; private set; }
    }
    private void Start()
    {
        CGcontroller = new(this, CG);
        PreviewPageCGcontroller = new(this, PreviewPageCG);

        GalleryConfig.Load();
        GetAllGalleryImages();
        //foreach (var image in GalleryImages)
        //{
        //    GalleryConfig.UnlockCG(image.name);
        //}
    }
    public void Initialize()
    {
        if (Initialized)
        {
            return;
        }
        BuildNaviBar();
        LoadPage(1);
        Initialized = true;
    }
    private void BuildNaviBar()
    {
        for (int i = 1; i <= PageCount; Interlocked.Increment(ref i))
        {
            var newButtonObject = Instantiate(NaviBarButtonPrefab.gameObject, NaviBarButtonPrefab.transform.parent);
            newButtonObject.SetActive(true);
            var newButton = newButtonObject.GetComponent<Button>();
            var newButtonTitle = newButton.GetComponentInChildren<TextMeshProUGUI>();
            int pageIndex = i;//±Õ°üÎÊÌâ
            newButton.onClick.AddListener(() => LoadPage(pageIndex));
            newButtonTitle.text = i.ToString();
        }
    }
    private void LoadPage(int pageIndex)
    {
        PageIndex.text = $"{pageIndex}/{PageCount}";

        int startingIndex = (pageIndex - 1) * PreviewsPerPage;
        for (int i = 0; i < PreviewsPerPage; Interlocked.Increment(ref i))
        {
            //Ware about difference between i & index!
            int index = i + startingIndex;
            var button = CGs[i].ImageButton;
            button.onClick.RemoveAllListeners();

            var title = CGs[i].Name;
            if (index >= ImageCount)
            {
                button.transform.parent.gameObject.SetActive(false);
                continue;
            }
            else
            {
                button.transform.parent.gameObject.SetActive(true);
                var renderer = button.targetGraphic as Image;
                var previewImage = GalleryImages[index];
                if (GalleryConfig.ImageUnlocked(previewImage.name))
                {
                    renderer.color = Color.white;
                    renderer.sprite = previewImage;
                    title.text = renderer.sprite.name;
                    button.onClick.AddListener(() => ShowPreviewImage(previewImage));
                }
                else
                {
                    renderer.color = Color.black;
                    renderer.sprite = null;
                    title.text = "";
                }
            }
        }
    }
    private void GetAllGalleryImages()
    {
        GalleryImages = Resources.LoadAll<Sprite>(Path.Combine(FilePaths.DefaultGalleryPaths));
    }
    private void ShowPreviewImage(Sprite image)
    {
        var renderer = PreviewPageImageButton.targetGraphic as Image;
        renderer.sprite = image;
        PreviewPageCGcontroller.Show();
        PreviewPageCGcontroller.SetCanvasStatus(true);
    }
    public void HidePreviewImage()
    {
        PreviewPageCGcontroller.Hide();
        PreviewPageCGcontroller.SetCanvasStatus(false);
    }
}
