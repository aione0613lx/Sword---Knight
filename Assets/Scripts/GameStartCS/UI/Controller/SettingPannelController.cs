using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingPannelController : MonoBehaviour
{
    [SerializeField] private Button openBar;
    [SerializeField] private Button saveButton;
    [SerializeField] private Button returnBar;
    [SerializeField] private Button tipUIButton;

    [SerializeField] private GameObject tipUI;

    private void Awake() 
    {
        openBar.onClick.AddListener(() => OpenBar());
        saveButton.onClick.AddListener(() => SaveGameButtonDown());
        returnBar.onClick.AddListener(() => ReturnBar());
        tipUIButton.onClick.AddListener(() => CloseTipUI());
        tipUI.gameObject.SetActive(false);
    }

    private void OpenBar()
    {

    }

    public void CloseTipUI()
    {
        tipUI.gameObject.SetActive(false);
    }
    
    private void SaveGameButtonDown()
    {
        if(GameManager.Instance == null || SettingConfigManager.Instance == null) return;

        GameManager.Instance.SaveGame(SettingConfigManager.Instance.slot,SettingConfigManager.Instance.saveName);

        tipUI.gameObject.SetActive(true);
    }

    public void ReturnBar()
    {
        if(GameManager.Instance != null) GameManager.Instance.SwitchScene(0);
    }
}
