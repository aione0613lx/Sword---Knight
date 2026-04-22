using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveSlotController : MonoBehaviour
{
    [SerializeField] private SaveSlotView saveSlotView;

    private SaveSlotModel saveSlotModel;

    public int slot;

    private void Awake() 
    {
        saveSlotModel = new SaveSlotModel();
        saveSlotModel.OnGainSaveSlotInfo += saveSlotView.UpdateSaveSlotUI;

        saveSlotView.OnSaveSlotButtonDown += SaveButtonDownStartGame;

        EventCenter.AddListener<SaveSlotInfo[]>(EventNameTable.ONTELLSAVEUIUPDATE,SetModelValue);
    }

    

    private void Start() 
    {
        saveSlotView.InitSaveSlotUI();    
    }

    private void OnDestroy() 
    {
        saveSlotModel.OnGainSaveSlotInfo -= saveSlotView.UpdateSaveSlotUI;

        saveSlotView.OnSaveSlotButtonDown -= SaveButtonDownStartGame;

        EventCenter.RemoveListener<SaveSlotInfo[]>(EventNameTable.ONTELLSAVEUIUPDATE,SetModelValue);
    }

    private void SetModelValue(SaveSlotInfo[] saveSlotInfos)
    {
        if(saveSlotInfos.Length < slot) return;

        saveSlotModel.SaveSlotInfo = saveSlotInfos[slot - 1];
    }

    public void SaveButtonDownStartGame()
    {   
        if(LogManager.Instance == null) return;
        if(saveSlotModel.HasSave == true)
        {
            LogManager.Instance.LoadGameStart(slot,saveSlotModel.SaveSlotInfo);
        }
        else
        {
            LogManager.Instance.NewGameStart(slot);
        }
    }
}
