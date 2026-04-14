using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class SettingPointHandle : MonoBehaviour, IPointerClickHandler
{
    private SettingPannelGenerator generator;
    private RectTransform rectTransform;
    private Tweener moveTweener;       // 当前按钮的动画

    [Header("调试")]
    [SerializeField] private bool debugMode = true;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        generator = GetComponentInParent<SettingPannelGenerator>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!eventData.pointerPress.CompareTag("PickSettingButton"))
            return;

        // 防止重复点击同一按钮（使用 InstanceID 更可靠）
        if (generator.lastMovedButton == rectTransform)
        {
            if (debugMode) Debug.Log($"[{gameObject.name}] 已经是选中状态，不做处理");
            return;
        }

        // 1. 将上一个按钮归位
        if (generator.lastMovedButton != null)
        {
            if (debugMode) Debug.Log($"[{gameObject.name}] 将上一个按钮 [{generator.lastMovedButton.name}] 移回原位 {generator.lastOriginalPos}");

            //在上一个按钮身上获取或添加 SettingPointHandle 组件，调用它的移动方法
            SettingPointHandle lastHandle = generator.lastMovedButton.GetComponent<SettingPointHandle>();
            if (lastHandle != null)
            {
                lastHandle.MoveToPosition(generator.lastOriginalPos);
            }
            else
            {
                // 如果没挂载脚本，直接利用DOTween移动回去
                generator.lastMovedButton.DOKill();
                generator.lastMovedButton.DOAnchorPos(generator.lastOriginalPos, generator.duration).SetEase(generator.easeType);
            }
        }

        // 2. 记录当前按钮的原始位置
        generator.lastMovedButton = rectTransform;
        generator.lastOriginalPos = rectTransform.anchoredPosition;

        // 3. 移动当前按钮
        Vector2 targetPos = generator.lastOriginalPos + new Vector2(generator.moveDistance, 0);
        if (debugMode) Debug.Log($"[{gameObject.name}] 从 {generator.lastOriginalPos} 移动到 {targetPos}");

        MoveToPosition(targetPos);
    }

    /// <summary>
    /// 公共方法：将该按钮移动到指定锚点位置
    /// </summary>
    public void MoveToPosition(Vector2 targetAnchoredPos)
    {
        moveTweener?.Kill();
        moveTweener = rectTransform.DOAnchorPos(targetAnchoredPos, generator.duration).SetEase(generator.easeType);
    }
}