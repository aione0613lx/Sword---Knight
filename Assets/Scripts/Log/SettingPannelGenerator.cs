using DG.Tweening;
using UnityEngine;

public class SettingPannelGenerator : MonoBehaviour
{
    [Header("引用")]
    public GameObject[] settings;

    [Header("移动参数")]
    public float moveDistance = 50f;

    [Header("动画参数")]
    public float duration = 0.5f;
    public Ease easeType = Ease.OutCubic;

    // 记录上一次被移动的按钮及其原始锚点位置
    [HideInInspector] public RectTransform lastMovedButton;
    [HideInInspector] public Vector2 lastOriginalPos;
}