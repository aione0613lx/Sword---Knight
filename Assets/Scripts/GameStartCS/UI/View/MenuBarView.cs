using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MenuBarView : MonoBehaviour
{
    [Header("按钮")]
    [SerializeField] private Button settingButton;
    [SerializeField] private Button statsButton;
    [SerializeField] private Button skillTreeButton;
    [SerializeField] private Button backpackButton;

    [Header("菜单栏画布组")]
    [SerializeField] private CanvasGroup menuBarPanel;

    [Header("各个面板的 RectTransform")]
    [SerializeField] private RectTransform settingPanel;
    [SerializeField] private RectTransform statsPanel;
    [SerializeField] private RectTransform skillTreePanel;
    [SerializeField] private RectTransform backpackPanel;

    [Header("动画参数")]
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private Ease easeType = Ease.OutCubic;

    // 暴露给 Controller 的点击事件
    public event Action OnSettingClick;
    public event Action OnStatsClick;
    public event Action OnSkillTreeClick;
    public event Action OnBackpackClick;

    private void Awake()
    {
        settingButton.onClick.AddListener(() => OnSettingClick?.Invoke());
        statsButton.onClick.AddListener(() => OnStatsClick?.Invoke());
        skillTreeButton.onClick.AddListener(() => OnSkillTreeClick?.Invoke());
        backpackButton.onClick.AddListener(() => OnBackpackClick?.Invoke());
    }

    /// <summary>
    /// 显示/隐藏整个菜单栏（包括四个按钮）
    /// </summary>
    public void SetMenuBarVisible(bool visible)
    {
        menuBarPanel.alpha = visible ? 1 : 0;
        menuBarPanel.interactable = visible;
        menuBarPanel.blocksRaycasts = visible;
    }

    /// <summary>
    /// 移动指定面板到目标位置
    /// </summary>
    public void MovePanel(RectTransform panel, Vector2 targetPos)
    {
        if (panel == null) return;
        panel.DOKill();
        panel.DOAnchorPos(targetPos, duration).SetEase(easeType);
    }

    // 以下是各面板的便捷移动方法
    public void MoveSettingPanel(Vector2 pos) => MovePanel(settingPanel, pos);
    public void MoveStatsPanel(Vector2 pos) => MovePanel(statsPanel, pos);
    public void MoveSkillTreePanel(Vector2 pos) => MovePanel(skillTreePanel, pos);
    public void MoveBackpackPanel(Vector2 pos) => MovePanel(backpackPanel, pos);
}