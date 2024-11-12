using GALGAME;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;

public class SLPage : MenuPage
{
    public static SLPage Instance { get; private set; }
    public static int MaxSlotIndex { get; } = 47;
    public enum MenuFuction { Save, Load }
    public MenuFuction CurrentFuction { get; internal set; } = MenuFuction.Save;
    private int CurrentPage { get; set; } = 1;
    public int SlotsPerPage => Slots.Length;
    private bool IsLoaded { get; set; } = false;
    private string SavePath => GalSaveFile.FileSavePath;
    [field: SerializeField]
    public SLSlot[] Slots { get; private set; }
    [field:SerializeField]
    public Texture EmptyFileTexture {  get; private set; }

    private void Awake()
    {
        Instance = this;
    }
    public override void Open()
    {
        base.Open();
        if (!IsLoaded)
        {
            GenerateSlots(CurrentPage);
        }
    }
    public void GenerateSlots(int pageIndex)
    {
        CurrentPage = pageIndex;
        int pageStartingSlotIndex = ((CurrentPage - 1) * SlotsPerPage) + 1;
        int pageEndingSlotIndex = pageStartingSlotIndex + SlotsPerPage - 1;
        for (int i = 0; i < Slots.Length; Interlocked.Increment(ref i))
        {
            SLSlot slot = Slots[i];
            int slotIndex = pageStartingSlotIndex + i;
            
            if (slotIndex < MaxSlotIndex)
            {
                slot.Root.SetActive(true);
                string fullSavePath = Path.Combine(SavePath, slotIndex.ToString()) + GalSaveFile.ID_DataFileType;
                slot.FileIndex = slotIndex;
                slot.FileSavePath = fullSavePath;
                slot.GenerateDetails(CurrentFuction);
}
            else
            {
                slot.Root.SetActive(false);
            }
        }
    }
}
