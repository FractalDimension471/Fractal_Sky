using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoundEndPanel : MonoBehaviour
{
    public static RoundEndPanel Instance { get; private set; }
    [field:SerializeField]
    public TextMeshProUGUI Title { get; internal set; }
    [field:SerializeField]
    public CanvasGroup CG { get; internal set; }
    [field:SerializeField]
    public Button RetryButton { get; internal set; }
    [field:SerializeField]
    public Button NextButton { get; internal set; }
    [field:SerializeField]
    public Button HomeButton { get; internal set; }
    private CanvasGroupController CGController { get; set; }
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        CGController = new(this, CG);
    }
    public void Show(string title, bool endSuccess, bool isEnd = false)
    {
        Title.text = title;
        HomeButton.gameObject.SetActive(isEnd);
        if (!isEnd)
        {
            NextButton.gameObject.SetActive(endSuccess);
        }
        else
        {
            RetryButton.gameObject.SetActive(false);
            NextButton.gameObject.SetActive(false);
        }
        CGController.SetCanvasStatus(true);
        CGController.Show();
    }
    public void Hide()
    {
        CGController.Hide();
        CGController.SetCanvasStatus(false);
    }
}
