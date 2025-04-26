using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Plate : MonoBehaviour
{
    public static Plate Instance { get; private set; }
    [field: SerializeField]
    public List<ElementBlock> Elements { get; internal set; }
    [field: SerializeField]
    public PlateLevel ActiveLevel { get; internal set; }
    [field: SerializeField]
    public CanvasGroup CG { get; internal set; }
    private CanvasGroupController CGController { get; set; }
    public bool IsResting { get; private set; } = false;
    private ESSystem ES => ESSystem.Instance;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        CGController = new(this, CG);
    }
    public IEnumerator InitializeNewRound(int level)
    {
        var newLevelPrefab = Resources.Load<GameObject>(FilePaths.GetPath(FilePaths.DefaultGameLevelPaths, $"Level{level}"));
        var newLevelob = GameObject.Instantiate(newLevelPrefab, transform);
        var newLevel = newLevelob.GetComponent<PlateLevel>();
        ActiveLevel = newLevel;
        while (!ActiveLevel.Initialized)
        {
            yield return null;
        }
        Elements = ActiveLevel.Elements;
        UpdateItemCount();
    }
    public void UpdateItemCount()
    {
        var magnetCount = ES.MagnetButton.gameObject.GetComponentInChildren<TextMeshProUGUI>();
        var backTrackCount = ES.BackTrackButton.gameObject.GetComponentInChildren<TextMeshProUGUI>();
        magnetCount.text = ES.MagnetCount.ToString();
        backTrackCount.text = ES.BackTrackCount.ToString();
    }
    public void ResetPlate()
    {
        ActiveLevel = null;
        Destroy(transform.GetChild(0).gameObject);
        Elements = new();
    }
    public void SetStatus(bool enable) => CGController.SetCanvasStatus(enable);


}
