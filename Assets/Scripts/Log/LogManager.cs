using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LogManager : SingletonMono<LogManager>
{
    [Header("Setting画布参数")]
    public RectTransform settingPannel;
    public Vector2 hidePos = new Vector2(0, 1500);
    public Vector2 showPos = new Vector2(0, 0);

    [Header("Saves画布参数")]
    public RectTransform saveSlotsPannel;

    [Header("动画参数")]
    public float duration = 0.5f;
    public Ease easeType = Ease.OutCubic;

    private Tweener panelTweener; // 用于防止动画冲突

    public void TellSaveDataUIUpdate()
    {   
        Debug.Log("更新SaveUI");
        SaveSlotInfo[] saveSlotInfos = SaveSystem.GetAllSaveSlots();
        Debug.Log("SaveSlotInfos:" + saveSlotInfos.Length);
        if(saveSlotInfos.Length == 0) return;

        EventCenter.EventTrigger<SaveSlotInfo[]>(EventNameTable.ONTELLSAVEUIUPDATE,saveSlotInfos);
    }
    
    public void GameStartGenerator()
    {
        EventCenter.Clear();
        SceneManager.LoadScene(1);
    }

    public void NewGameStart(int slot)
    {
        SettingConfigManager.Instance.slot = slot;
        SettingConfigManager.Instance.saveName = "Save" + slot.ToString();
        SettingConfigManager.Instance.isNew = true;
        GameStartGenerator();
    }

    public void LoadGameStart(int slot,SaveSlotInfo saveSlot)
    {
        SettingConfigManager.Instance.slot = slot;
        SettingConfigManager.Instance.saveName = saveSlot.saveName;
        SettingConfigManager.Instance.isNew = false;
        GameStartGenerator();
    }

    public void OpenSavesPannel()
    {
        TellSaveDataUIUpdate();
        // 停止正在进行的动画，避免冲突
        panelTweener?.Kill();
        // 将画布从当前位置平滑移动到显示位置
        panelTweener = saveSlotsPannel.DOAnchorPos(showPos, duration).SetEase(easeType);
    }

    public void CloseSavesPannel()
    {
        panelTweener?.Kill();
        // 将画布平滑移回隐藏位置
        panelTweener = saveSlotsPannel.DOAnchorPos(hidePos, duration).SetEase(easeType);
    }

    public void OpenSettingPanel()
    {
        // 停止正在进行的动画，避免冲突
        panelTweener?.Kill();
        // 将画布从当前位置平滑移动到显示位置
        panelTweener = settingPannel.DOAnchorPos(showPos, duration).SetEase(easeType);
    }

    public void CloseSettingPannel()
    {
        panelTweener?.Kill();
        // 将画布平滑移回隐藏位置
        panelTweener = settingPannel.DOAnchorPos(hidePos, duration).SetEase(easeType);
    }

    public void ExitGame()
    {
        Debug.Log("退出游戏！");
        Application.Quit();
    }
}
