using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Lacner : EnemyController
{   
    private enum LacnerState{
        Idel,
        Run,
        Attack,
        Return
    }

    [Header("移动参数")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float maxChaseDistance = 15f;
    [SerializeField] private float returnThreshold = 0.5f;
    [SerializeField] private float attckRange = 1.5f;
    [SerializeField] private bool allowAttack = true;
    [SerializeField] private bool allowMove = true;
    [SerializeField] private float attackGap = 2;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float hitRadius = 0.85f;
    [SerializeField] private Vector2 hitOffset = new Vector2(1.1f, 0f);

    private Transform player;
    private bool isFacingRight = true;
    private bool isReturning = false;
    private LacnerState lacner = LacnerState.Idel;
    private Rigidbody2D rb;
    private Animator anim;
    private Vector2 lastHitCenter;

    private void Awake()
    {   
        enemySO = Instantiate(enemySO);
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
        if (player == null)
        {
            if (isReturning) lacner = LacnerState.Return;
            else lacner = LacnerState.Idel;
            return;
        }

        // 计算到玩家的向量与距离
        Vector2 toPlayer = player.position - transform.position;
        float distanceToPlayer = toPlayer.magnitude;
        float distanceFromStart = Vector2.Distance(transform.position, startPos);

        // 超过起始点可追击距离时，放弃追击并返航
        if (distanceFromStart > maxChaseDistance)
        {
            player = null;
            isReturning = true;
            lacner = LacnerState.Return;
            return;
        }

        if(distanceToPlayer <= attckRange)
        {
            // 进入攻击距离后停止位移，避免持续顶着玩家产生推力
            rb.velocity = Vector2.zero;

            if (allowAttack)
            {
                Attack();
            }
            else
            {
                lacner = LacnerState.Idel;
            }
            return;
        }

        isReturning = false;
        lacner = LacnerState.Run;

        // 朝向玩家翻转
        if ((toPlayer.x < 0 && isFacingRight) || (toPlayer.x > 0 && !isFacingRight))
            Flip();
        
        if(allowMove)
        {   
            Vector2 moveDir = toPlayer.normalized;
            rb.MovePosition(rb.position + moveDir * moveSpeed * Time.fixedDeltaTime);
        }
    }

    private void Back()
    {
        // 返回起始点逻辑（使用 FixedUpdate 保持物理平滑）
        if (isReturning && player == null)
        {
            lacner = LacnerState.Return;
            Vector2 toStart = startPos - rb.position;
            float distanceToStart = toStart.magnitude;

            if (distanceToStart <= returnThreshold)
            {
                isReturning = false;
                rb.MovePosition(startPos);
                lacner = LacnerState.Idel;
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
        float facingX = isFacingRight ? 1f : -1f;
        lastHitCenter = (Vector2)transform.position + new Vector2(hitOffset.x * facingX, hitOffset.y);
        anim.Play(GetAttackDirection(player.position - transform.position));
        allowAttack = false;
        StartCoroutine(StartDelaySetBool(attackGap));
    }

    // Animation Event: 攻击动画结束帧调用
    public void AttackEndHandle()
    {
        lacner = LacnerState.Idel;
        allowMove = true;
    }

    // Animation Event: 攻击命中帧调用
    public void AttackHit()
    {
        Collider2D hit = Physics2D.OverlapCircle(lastHitCenter, hitRadius, playerLayer);
        if (hit == null) return;

        PlayerHealth hp = hit.GetComponent<PlayerHealth>();
        if (hp == null) return;
        hp.ReceiveDamage(enemySO.damage);
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
            case LacnerState.Return:
            anim.SetBool("isRun",true);
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
            isReturning = false;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (player != other.transform) return;

        player = null;
        isReturning = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(lastHitCenter, hitRadius);
    }
}