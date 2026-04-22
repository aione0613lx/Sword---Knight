using UnityEngine;
using UnityEngine.UI;

public class GitHubController : MonoBehaviour
{
    [SerializeField] private Button button;

    private const string GITHUB_URL = "https://github.com/aione0613lx";

    private void Awake()
    {
        if (button != null)
        {
            button.onClick.AddListener(GOGitHub);
        }
        else
        {
            Debug.LogError("GitHubController: Button 未赋值！请在 Inspector 中拖拽。");
        }
    }

    /// <summary>
    /// 跳转到 GitHub 主页
    /// </summary>
    public void GOGitHub()
    {
        if (!string.IsNullOrEmpty(GITHUB_URL))
        {
            Application.OpenURL(GITHUB_URL);
        }
        else
        {
            Debug.LogWarning("GitHub URL 为空，请检查脚本中的 GITHUB_URL 常量。");
        }
    }

    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(GOGitHub);
        }
    }
}