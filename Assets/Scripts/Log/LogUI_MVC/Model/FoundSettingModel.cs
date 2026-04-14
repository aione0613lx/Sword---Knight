using System;

[Serializable]
public class FoundSettingModel
{
    public event Action<bool> OnBGMChanged;
    public event Action<bool> OnFoleyChanged;
    public event Action<bool> OnAllowCheatingChanged;
    public event Action<DifficultyLevel> OnDifficultyChanged;

    private bool bgmEnabled = true;
    private bool foleyEnabled = true;
    private bool allowCheating = false;
    private DifficultyLevel currentDifficulty = DifficultyLevel.Average;

    public bool BGMEnabled
    {
        get => bgmEnabled;
        set
        {
            if (bgmEnabled != value)
            {
                bgmEnabled = value;
                OnBGMChanged?.Invoke(value);
            }
        }
    }

    public bool FoleyEnabled
    {
        get => foleyEnabled;
        set
        {
            if (foleyEnabled != value)
            {
                foleyEnabled = value;
                OnFoleyChanged?.Invoke(value);
            }
        }
    }

    public bool AllowCheating
    {
        get => allowCheating;
        set
        {
            if (allowCheating != value)
            {
                allowCheating = value;
                OnAllowCheatingChanged?.Invoke(value);
            }
        }
    }

    public DifficultyLevel CurrentDifficulty
    {
        get => currentDifficulty;
        set
        {
            if (currentDifficulty != value)
            {
                currentDifficulty = value;
                OnDifficultyChanged?.Invoke(value);
            }
        }
    }
}

public enum DifficultyLevel
{
    Easy,
    Average,
    Difficulty
}