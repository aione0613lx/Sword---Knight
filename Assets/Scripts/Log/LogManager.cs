using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogManager : SingletonMono<LogManager>
{
    [Header("Setting画布参数")]
    public RectTransform settingPannel;
    public Vector2 hidePos = new Vector2(0, 1500);
    public Vector2 showPos = new Vector2(0, 0);

    [Header("动画参数")]
    public float duration = 0.5f;
    public Ease easeType = Ease.OutCubic;

    private Tweener panelTweener; // 用于防止动画冲突
    
    public void GameStartGenerator()
    {
        

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
