using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlotView : MonoBehaviour
{
    [SerializeField] private Button saveSlotButton;
    [SerializeField] private TMP_Text saveState;
    [SerializeField] private TMP_Text saveName;
    [SerializeField] private TMP_Text lastPlayTime;
    [SerializeField] private TMP_Text level;

    public event Action OnSaveSlotButtonDown;

    private void Awake() 
    {
        saveSlotButton.onClick.AddListener(() => OnSaveSlotButtonDown?.Invoke());
    }

    public void InitSaveSlotUI()
    {
        saveName.text = "";
        lastPlayTime.text = "";
        level.text = "";
        saveState.text = "新游戏";
    }

    public void UpdateSaveSlotUI(SaveSlotInfo saveSlot)
    {
        saveName.text = saveSlot.saveName;
        lastPlayTime.text = saveSlot.saveTime;
        level.text = saveSlot.playerLevel.ToString();
        saveState.text = "继续游戏！";
    }
}
