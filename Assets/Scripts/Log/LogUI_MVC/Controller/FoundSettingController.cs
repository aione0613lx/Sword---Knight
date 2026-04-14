using UnityEngine;

public class FoundSettingController : MonoBehaviour
{
    [SerializeField] private FoundSettingView view;

    private FoundSettingModel model;

    private void Awake()
    {
        // 创建 Model 实例
        model = new FoundSettingModel();

        // 订阅 View 的用户操作事件
        view.OnBGMToggled += HandleBGMToggle;
        view.OnFoleyToggled += HandleFoleyToggle;
        view.OnAllowCheatingToggled += HandleAllowCheatingToggle;
        view.OnDifficultySelected += HandleDifficultySelected;

        // 订阅 Model 的数据变化事件，同步更新 View
        model.OnBGMChanged += view.SetBGMTickVisible;
        model.OnFoleyChanged += view.SetFoleyTickVisible;
        model.OnAllowCheatingChanged += view.SetAllowCheatingTickVisible;
        model.OnDifficultyChanged += view.UpdateDifficultyText;

        // 初始化 View 显示（根据 Model 的默认值）
        RefreshView();
    }

    private void RefreshView()
    {
        view.SetBGMTickVisible(model.BGMEnabled);
        view.SetFoleyTickVisible(model.FoleyEnabled);
        view.SetAllowCheatingTickVisible(model.AllowCheating);
        view.UpdateDifficultyText(model.CurrentDifficulty);
    }

    private void HandleBGMToggle()
    {
        model.BGMEnabled = !model.BGMEnabled;
    }

    private void HandleFoleyToggle()
    {
        model.FoleyEnabled = !model.FoleyEnabled;
    }

    private void HandleAllowCheatingToggle()
    {
        model.AllowCheating = !model.AllowCheating;
    }

    private void HandleDifficultySelected(DifficultyLevel level)
    {
        model.CurrentDifficulty = level;
    }

    private void OnDestroy()
    {
        // 取消订阅，防止悬空引用
        view.OnBGMToggled -= HandleBGMToggle;
        view.OnFoleyToggled -= HandleFoleyToggle;
        view.OnAllowCheatingToggled -= HandleAllowCheatingToggle;
        view.OnDifficultySelected -= HandleDifficultySelected;

        model.OnBGMChanged -= view.SetBGMTickVisible;
        model.OnFoleyChanged -= view.SetFoleyTickVisible;
        model.OnAllowCheatingChanged -= view.SetAllowCheatingTickVisible;
        model.OnDifficultyChanged -= view.UpdateDifficultyText;
    }
}