using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FoundSettingView : MonoBehaviour
{
    [Header("开关按钮")]
    [SerializeField] private Button BGMButton;
    [SerializeField] private Button foleyButton;
    [SerializeField] private Button allowCheatingButton;

    [Header("勾选图标（Image 组件）")]
    [SerializeField] private Image BGMTick;
    [SerializeField] private Image foleyTick;
    [SerializeField] private Image allowCheatingTick;

    [Header("难度选择按钮")]
    [SerializeField] private Button challengeLevelEasy;
    [SerializeField] private Button challengeLevelAverage;
    [SerializeField] private Button challengeLevelDifficulty;

    [Header("难度显示文本")]
    [SerializeField] private TMP_Text challengeLevelText;

    // 对外暴露的事件，供 Controller 订阅
    public event Action OnBGMToggled;
    public event Action OnFoleyToggled;
    public event Action OnAllowCheatingToggled;
    public event Action<DifficultyLevel> OnDifficultySelected;

    private void Awake()
    {
        // 绑定开关按钮点击事件
        BGMButton.onClick.AddListener(() => OnBGMToggled?.Invoke());
        foleyButton.onClick.AddListener(() => OnFoleyToggled?.Invoke());
        allowCheatingButton.onClick.AddListener(() => OnAllowCheatingToggled?.Invoke());

        // 绑定难度选择按钮点击事件
        challengeLevelEasy.onClick.AddListener(() => OnDifficultySelected?.Invoke(DifficultyLevel.Easy));
        challengeLevelAverage.onClick.AddListener(() => OnDifficultySelected?.Invoke(DifficultyLevel.Average));
        challengeLevelDifficulty.onClick.AddListener(() => OnDifficultySelected?.Invoke(DifficultyLevel.Difficulty));
    }


    public void SetBGMTickVisible(bool visible)
    {
        BGMTick.gameObject.SetActive(visible);
    }

    public void SetFoleyTickVisible(bool visible)
    {
        foleyTick.gameObject.SetActive(visible);
    }

    public void SetAllowCheatingTickVisible(bool visible)
    {
        allowCheatingTick.gameObject.SetActive(visible);
    }

    public void UpdateDifficultyText(DifficultyLevel level)
    {
        string levelStr = level switch
        {
            DifficultyLevel.Easy => "简单",
            DifficultyLevel.Average => "普通",
            DifficultyLevel.Difficulty => "困难",
            _ => "普通"
        };
        challengeLevelText.text = "游戏难度：" + levelStr;
    }

    private void OnDestroy()
    {
        // 清理监听，避免内存泄漏
        BGMButton.onClick.RemoveAllListeners();
        foleyButton.onClick.RemoveAllListeners();
        allowCheatingButton.onClick.RemoveAllListeners();
        challengeLevelEasy.onClick.RemoveAllListeners();
        challengeLevelAverage.onClick.RemoveAllListeners();
        challengeLevelDifficulty.onClick.RemoveAllListeners();
    }
}