using System;
using UnityEngine;

public class MenuBarModel
{
    private bool isMenuBarOpen = false;

    public event Action<bool> OnMenuBarToggle;
    public event Action<PanelType> OnPanelOpen;

    public Vector2 startPos;
    public Vector2 endPos;

    public PanelType currentOpenPanel = PanelType.None;

    public MenuBarModel(Vector2 startPos, Vector2 endPos)
    {
        this.startPos = startPos;
        this.endPos = endPos;
    }

    public bool IsMenuBarOpen
    {
        get => isMenuBarOpen;
        set
        {
            if (isMenuBarOpen != value)
            {
                isMenuBarOpen = value;
                OnMenuBarToggle?.Invoke(isMenuBarOpen);
            }
        }
    }

    public void OpenPanel(PanelType panel)
    {
        // 如果点击的是当前已打开的面板，则关闭它（设为 None）
        if (currentOpenPanel == panel)
            panel = PanelType.None;

        currentOpenPanel = panel;
        OnPanelOpen?.Invoke(panel);
    }

    public void CloseAllPanels()
    {
        if (currentOpenPanel != PanelType.None)
        {
            currentOpenPanel = PanelType.None;
            OnPanelOpen?.Invoke(PanelType.None);
        }
    }
}

public enum PanelType
{
    None,
    Setting,
    Stats,
    SkillTree,
    Backpack
}