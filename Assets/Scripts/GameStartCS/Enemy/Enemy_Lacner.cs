using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Lacner : MonoBehaviour
{   
    private enum LacnerState{
        Idel,
        Run,
        Attack
    }

    [Header("移动参数")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float maxChaseDistance = 10f;   // 最大追击距离
    [SerializeField] private float returnThreshold = 0.5f;   // 返回起始点的距离阈值
    [SerializeField] private float attckRange = 1.5f;
    [SerializeField] private bool allowAttack = true;
    [SerializeField] private bool allowMove = true;
    [SerializeField] private float attackGap = 2;

    private Transform player;
    private Vector2 startPos;
    private bool isFacingRight = true;
    private bool isReturning = false;    // 是否正在返回起始点
    private LacnerState lacner = LacnerState.Idel;
    private Rigidbody2D rb;
    private Animator anim;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        startPos = transform.position;
    }

    private void Update()
    {
        Chase();
        SwitchAnim();
    }

    private void FixedUpdate()
    {
        Back();
    }

    private void Chase()
    {
        if (player == null) return;

        // 计算到玩家的向量与距离
        Vector2 toPlayer = player.position - transform.position;
        float distanceToPlayer = toPlayer.magnitude;

        // 超过最大追击距离 → 放弃追击，返回起始点
        if (distanceToPlayer > maxChaseDistance)
        {
            player = null;
            isReturning = true;
            return;
        }

        if(distanceToPlayer <= attckRange && allowAttack)
        {
            Attack();
        }

        // 正常追击
        isReturning = false;

        // 朝向玩家翻转
        if ((toPlayer.x < 0 && isFacingRight) || (toPlayer.x > 0 && !isFacingRight))
            Flip();
        
        // 移动（归一化方向）
        if(allowMove)
        {   
            lacner = LacnerState.Run;
            Vector2 moveDir = toPlayer.normalized;
            rb.MovePosition(rb.position + moveDir * moveSpeed * Time.fixedDeltaTime);
        }
    }

    private void Back()
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

    private void Attack()
    {
        lacner = LacnerState.Attack;
        allowMove = false;
        anim.Play(GetAttackDirection(player.position - transform.position));
        allowAttack = false;
        StartCoroutine(StartDelaySetBool(attackGap));
    }

    private void AttackEndHandle()
    {
        lacner = LacnerState.Idel;
        allowMove = true;
    }

    private IEnumerator StartDelaySetBool(float time)
    {
        yield return new WaitForSeconds(time);
        allowAttack = true;
    }

    private void SwitchAnim()
    {
        switch(lacner)
        {
            case LacnerState.Idel:
            anim.SetBool("isRun",false);
            break;
            case LacnerState.Run:
            anim.SetBool("isRun",true);
            anim.SetBool("Idel",false);
            break;
            case LacnerState.Attack:
            anim.SetBool("isRun",false);
            anim.SetBool("Idel",false);
            break;
        }
    }
    /// <summary>
    /// 根据玩家相对方向返回对应的攻击动画 Trigger 名称
    /// 方向判定基于玩家相对于敌人的方位，考虑敌人当前的朝向（isFacingRight）
    /// </summary>
    private string GetAttackDirection(Vector2 toPlayer)
    {
        // 将方向转换为相对于敌人朝向的局部方向
        float angle = Mathf.Atan2(toPlayer.y, toPlayer.x) * Mathf.Rad2Deg;
        if (!isFacingRight)
        {
            // 如果敌人朝左，镜像角度：以 Y 轴翻转（等价于 180 - angle）
            angle = 180 - angle;
        }

        // 将角度规范化到 [0, 360)
        angle = (angle + 360) % 360;

        // 根据角度区间返回对应的 Trigger 名称
        // 区间划分：上 (67.5~112.5)，上右 (22.5~67.5)，右 (337.5~22.5)，右下 (292.5~337.5)，下 (247.5~292.5)
        if (angle >= 67.5f && angle < 112.5f)
            return "Up_Attack";
        else if (angle >= 22.5f && angle < 67.5f)
            return "Up_Attack_Right";
        else if (angle >= 337.5f || angle < 22.5f)
            return "Attack_Right";
        else if (angle >= 292.5f && angle < 337.5f)
            return "Down_Attack_Right";
        else // 247.5 ~ 292.5
            return "Down_Attack";
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
}