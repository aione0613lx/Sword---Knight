using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Lacner : MonoBehaviour
{
    [Header("移动参数")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float maxChaseDistance = 10f;   // 最大追击距离
    [SerializeField] private float returnThreshold = 0.5f;   // 返回起始点的距离阈值

    private Transform player;
    private Vector2 startPos;
    private bool isFacingRight = true;
    private bool isChasing = false;      // 是否正在追击
    private bool isReturning = false;    // 是否正在返回起始点

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        startPos = transform.position;
    }

    private void Update()
    {
        if (player == null) return;

        // 计算到玩家的向量与距离
        Vector2 toPlayer = player.position - transform.position;
        float distanceToPlayer = toPlayer.magnitude;

        // 超过最大追击距离 → 放弃追击，返回起始点
        if (distanceToPlayer > maxChaseDistance)
        {
            player = null;
            isChasing = false;
            isReturning = true;
            return;
        }

        // 正常追击
        isChasing = true;
        isReturning = false;

        // 朝向玩家翻转
        if ((toPlayer.x < 0 && isFacingRight) || (toPlayer.x > 0 && !isFacingRight))
            Flip();

        // 移动（归一化方向）
        Vector2 moveDir = toPlayer.normalized;
        rb.MovePosition(rb.position + moveDir * moveSpeed * Time.fixedDeltaTime);
    }

    private void FixedUpdate()
    {
        // 返回起始点逻辑（使用 FixedUpdate 保持物理平滑）
        if (isReturning && player == null)
        {
            Vector2 toStart = startPos - rb.position;
            float distanceToStart = toStart.magnitude;

            if (distanceToStart <= returnThreshold)
            {
                isReturning = false;
                rb.MovePosition(startPos);
                return;
            }

            // 朝向起始点翻转
            if ((toStart.x < 0 && isFacingRight) || (toStart.x > 0 && !isFacingRight))
                Flip();

            Vector2 moveDir = toStart.normalized;
            rb.MovePosition(rb.position + moveDir * moveSpeed * Time.fixedDeltaTime);
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1f;
        transform.localScale = scale;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("发现玩家，开始追击");
            player = other.transform;
            startPos = transform.position;   // 记录当前位置作为返回点
            isReturning = false;
        }
    }

    // 可视化调试：在 Scene 视图中显示追击范围
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxChaseDistance);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(startPos == Vector2.zero ? transform.position : startPos, 0.5f);
    }
}