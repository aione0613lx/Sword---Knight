using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveController : MonoBehaviour
{
    [SerializeField] private SaveView saveView;
    [SerializeField] private string gameSceneName = "Game"; // 在 Inspector 中填写场景名

    private SaveModel saveModel;

    private void Awake()
    {
        saveModel = new SaveModel();

        // 订阅 View 的按钮点击事件
        saveView.OnSaveButtonClick += HandleSaveButtonClick;

        // 根据存档状态更新 UI 文本（示例：显示"新游戏"或"继续游戏"）
        UpdateUIBySaveState();
    }

    private void UpdateUIBySaveState()
    {
        if (saveModel.isUse)
        {
            saveView.UpdateSaveText("继续游戏");
        }
        else
        {
            saveView.UpdateSaveText("新游戏");
        }
    }

    private void HandleSaveButtonClick()
    {
        if (saveModel.isUse)
        {
            // TODO: 加载已有存档的逻辑（读取数据、还原游戏状态等）
            Debug.Log("TODO: 加载已有存档");
            // 示例：加载完成后也要切换到游戏场景
            // SceneManager.LoadScene(gameSceneName);
        }
        else
        {
            // 无存档：创建新世界 → 切换场景
            CreateNewWorld();
        }
    }

    private void CreateNewWorld()
    {
        // 这里可以添加初始化新游戏数据的代码（后续与存档系统结合）
        Debug.Log("创建新世界，切换到场景：" + gameSceneName);
        SceneManager.LoadScene(gameSceneName);
    }

    private void OnDestroy()
    {
        saveView.OnSaveButtonClick -= HandleSaveButtonClick;
    }
}