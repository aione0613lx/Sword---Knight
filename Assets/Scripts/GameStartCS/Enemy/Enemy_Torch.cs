using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Torch : EnemyController
{
    private enum TorchState
    {
        Idel,
        Run,
        Attack,
        Return
    }

    [Header("移动参数")]
    [SerializeField] private float moveSpeed = 2.8f;
    [SerializeField] private float stopAttackDistance = 1.6f;
    [SerializeField] private float maxChaseFromStart = 15f;
    [SerializeField] private float returnStopDistance = 0.2f;

    [Header("攻击参数")]
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float attackRadius = 0.9f;
    [SerializeField] private float attackCooldown = 1.2f;
    [SerializeField] private float oneBodyLength = 1f;
    [SerializeField] private float similarXThreshold = 0.8f;
    [SerializeField] private Vector2 forwardAttackOffset = new Vector2(1.1f, 0f);
    [SerializeField] private Vector2 upAttackOffset = new Vector2(0f, 1f);
    [SerializeField] private Vector2 downAttackOffset = new Vector2(0f, -1f);

    private Transform player;
    private Rigidbody2D rb;
    private Animator anim;
    private TorchState torchState = TorchState.Idel;
    private bool isFacingRight = true;
    private bool isAttacking;
    private float nextAttackTime;
    private Vector2 lastAttackCenter;

    private void Awake()
    {
        enemySO = Instantiate(enemySO);
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        startPos = transform.position;
    }

    private void Update()
    {
        if (player == null)
        {
            if (torchState != TorchState.Return)
            {
                torchState = TorchState.Idel;
            }
            SwitchAnim();
            return;
        }

        if (isAttacking)
        {
            SwitchAnim();
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        float distanceFromStart = Vector2.Distance(transform.position, startPos);

        if (distanceFromStart > maxChaseFromStart)
        {
            player = null;
            torchState = TorchState.Return;
            SwitchAnim();
            return;
        }

        if (distanceToPlayer <= stopAttackDistance)
        {
            rb.velocity = Vector2.zero;
            if (Time.time >= nextAttackTime)
            {
                StartAttack();
            }
        }
        else
        {
            torchState = TorchState.Run;
            ChasePlayer();
        }

        SwitchAnim();
    }

    private void FixedUpdate()
    {
        if (torchState == TorchState.Return && !isAttacking)
        {
            ReturnToStart();
        }
    }

    private void ChasePlayer()
    {
        if (player == null) return;

        Vector2 toPlayer = (player.position - transform.position).normalized;
        if (toPlayer.x != 0f)
        {
            FaceTo(toPlayer.x);
        }

        rb.MovePosition(rb.position + toPlayer * moveSpeed * Time.fixedDeltaTime);
    }

    private void ReturnToStart()
    {
        // 返航期间始终保持 Run 状态
        torchState = TorchState.Return;

        Vector2 toStart = startPos - rb.position;
        float distance = toStart.magnitude;
        if (distance <= returnStopDistance)
        {
            rb.MovePosition(startPos);
            rb.velocity = Vector2.zero;
            torchState = TorchState.Idel;
            return;
        }

        Vector2 dir = toStart.normalized;
        if (dir.x != 0f)
        {
            FaceTo(dir.x);
        }

        rb.MovePosition(rb.position + dir * moveSpeed * Time.fixedDeltaTime);
    }

    private void StartAttack()
    {
        isAttacking = true;
        torchState = TorchState.Attack;
        nextAttackTime = Time.time + attackCooldown;

        Vector2 toPlayer = player.position - transform.position;
        string attackAnim = SelectAttackAnim(toPlayer, out Vector2 attackCenter);
        lastAttackCenter = attackCenter;
        anim.Play(attackAnim);
    }

    private string SelectAttackAnim(Vector2 toPlayer, out Vector2 attackCenter)
    {
        Vector2 origin = transform.position;
        float facingX = isFacingRight ? 1f : -1f;
        Vector2 frontOffset = new Vector2(forwardAttackOffset.x * facingX, forwardAttackOffset.y);

        bool isUpper = Mathf.Abs(toPlayer.x) <= similarXThreshold && toPlayer.y >= oneBodyLength;
        bool isLower = Mathf.Abs(toPlayer.x) <= similarXThreshold && toPlayer.y <= -oneBodyLength;

        if (isUpper)
        {
            attackCenter = origin + upAttackOffset;
            return "Attack3";
        }

        if (isLower)
        {
            attackCenter = origin + downAttackOffset;
            return "Attack2";
        }

        attackCenter = origin + frontOffset;
        return "Attack1";
    }

    private void DealDamageAt(Vector2 center)
    {
        Collider2D hit = Physics2D.OverlapCircle(center, attackRadius, playerLayer);
        if (hit == null) return;

        PlayerHealth hp = hit.GetComponent<PlayerHealth>();
        Debug.Log(hp.gameObject.name);
        if (hp == null) return;

        hp.ReceiveDamage(enemySO.damage);
    }

    // Animation Event: 在攻击动画的命中帧调用
    public void AttackHit()
    {
        DealDamageAt(lastAttackCenter);
    }

    // Animation Event: 在攻击动画末尾调用
    public void AttackEndHandle()
    {
        isAttacking = false;
        if (player != null)
        {
            torchState = TorchState.Run;
        }
        else
        {
            torchState = TorchState.Return;
        }
    }

    private void SwitchAnim()
    {
        if (anim == null) return;

        bool isRun = torchState == TorchState.Run || torchState == TorchState.Return;
        anim.SetBool("isRun", isRun);
        anim.SetBool("Idel", !isRun && torchState != TorchState.Attack);
    }

    private void FaceTo(float xDirection)
    {
        if (xDirection < 0f && isFacingRight)
        {
            Flip();
        }
        else if (xDirection > 0f && !isFacingRight)
        {
            Flip();
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
        if (!other.CompareTag("Player")) return;
        player = other.transform;
        torchState = TorchState.Run;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (player != other.transform) return;
        player = null;
        if (!isAttacking)
        {
            torchState = TorchState.Return;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, stopAttackDistance);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(startPos, maxChaseFromStart);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(lastAttackCenter, attackRadius);
    }
}
