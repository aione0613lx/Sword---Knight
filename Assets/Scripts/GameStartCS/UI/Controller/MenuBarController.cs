using UnityEngine;

public class MenuBarController : MonoBehaviour
{
    [SerializeField] private MenuBarView menuBarView;
    [SerializeField] private Vector2 startPos = new Vector2(1600, 0);
    [SerializeField] private Vector2 endPos = new Vector2(500, 0);

    private MenuBarModel menuBarModel;

    private void Awake()
    {
        menuBarModel = new MenuBarModel(startPos, endPos);

        // 订阅 View 的按钮点击事件
        menuBarView.OnSettingClick += () => HandlePanelClick(PanelType.Setting);
        menuBarView.OnStatsClick += () => HandlePanelClick(PanelType.Stats);
        menuBarView.OnSkillTreeClick += () => HandlePanelClick(PanelType.SkillTree);
        menuBarView.OnBackpackClick += () => HandlePanelClick(PanelType.Backpack);

        // 订阅 Model 的状态变化事件
        menuBarModel.OnMenuBarToggle += OnMenuBarToggled;
        menuBarModel.OnPanelOpen += OnPanelChanged;
    }

    private void Start()
    {
        // 初始化所有面板到屏幕外，菜单栏隐藏
        ResetAllPanelsToStart();
        menuBarView.SetMenuBarVisible(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            menuBarModel.IsMenuBarOpen = !menuBarModel.IsMenuBarOpen;
        }
    }

    private void OnMenuBarToggled(bool isOpen)
    {
        menuBarView.SetMenuBarVisible(isOpen);

        // 当菜单栏关闭时，自动关闭所有打开的面板
        if (!isOpen)
        {
            menuBarModel.CloseAllPanels();
        }
    }

    private void HandlePanelClick(PanelType clickedPanel)
    {
        // 仅当菜单栏打开时才处理面板切换
        if (!menuBarModel.IsMenuBarOpen) return;

        menuBarModel.OpenPanel(clickedPanel);
    }

    private void OnPanelChanged(PanelType openPanel)
    {
        // 先将所有面板移回起始位置
        ResetAllPanelsToStart();

        // 再根据当前要打开的面板，将其移动到终点位置
        switch (openPanel)
        {
            case PanelType.Setting:
                menuBarView.MoveSettingPanel(endPos);
                break;
            case PanelType.Stats:
                menuBarView.MoveStatsPanel(endPos);
                break;
            case PanelType.SkillTree:
                menuBarView.MoveSkillTreePanel(endPos);
                break;
            case PanelType.Backpack:
                menuBarView.MoveBackpackPanel(endPos);
                break;
            case PanelType.None:
                // 全部已归位，无需额外操作
                break;
        }
    }

    private void ResetAllPanelsToStart()
    {
        menuBarView.MoveSettingPanel(startPos);
        menuBarView.MoveStatsPanel(startPos);
        menuBarView.MoveSkillTreePanel(startPos);
        menuBarView.MoveBackpackPanel(startPos);
    }

    private void OnDestroy()
    {
        menuBarModel.OnMenuBarToggle -= OnMenuBarToggled;
        menuBarModel.OnPanelOpen -= OnPanelChanged;
    }
}