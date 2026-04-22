using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlayerController : MonoBehaviour
{   
    public PlayerStatsSO playerSO;
    public PlayerHealth playerHealth;
    private bool isFacingRight = true;
    private Vector2 moveInput;
    [SerializeField]private bool allowMove = true;
    [SerializeField]private bool allowGuard = true;
    [SerializeField]private bool allowAttack = true;
    private Animator anim;
    private Transform acPoint;//攻击碰撞点
    public LayerMask enemyLayer;
    private float radius = 2f;

    private void Start() 
    {
        anim = GetComponent<Animator>();
        playerHealth = GetComponent<PlayerHealth>();
        acPoint = GetComponentInChildren<Transform>();
    }

    private void Update() 
    {   
        if(Input.GetButtonDown("AttckPlayer"))
        {
            playerHealth.ReceiveDamage(3);
        }
        
        if(Input.GetButtonDown("OpenShop"))
        {
            EventCenter.EventTrigger(EventNameTable.ONOPENSHOP);
        }

        if(allowMove)
        {
            Move();
        }
        if(Input.GetButtonDown("Guard") && allowGuard && Input.GetButtonDown("Attack") && allowAttack)
        {
            EnhancedAttack();
            Debug.Log("重击！");
        }
        else if(Input.GetButtonDown("Guard") && allowGuard)
        {
            Guard();
        }
        else if(Input.GetButtonDown("Attack") && allowAttack)
        {
            Attack();
        }
    }

    #region 移动模块

    public void Move()
    {   
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        anim.SetFloat("moveX",Mathf.Abs(moveInput.x));
        anim.SetFloat("moveY",Mathf.Abs(moveInput.y));

        moveInput.Normalize();
        if(moveInput.x < 0 && isFacingRight || moveInput.x > 0 && !isFacingRight)
        {
            Flip();
        }

        transform.position += (Vector3)moveInput * playerSO.speed * Time.deltaTime;
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1f;
        transform.localScale = scale;
    }

    #endregion

    #region 防御模块

    private void Guard()
    {
        allowMove = false;
        allowGuard = false;
        anim.Play("Guard");
        playerHealth.UpdateDefense(100);
        StartCoroutine(StartDelaySetBool(1.5f,"Guard"));
    }

    public void GuardEndHandle()
    {
        allowMove = true;
        playerHealth.UpdateDefense(-100);
    }

    #endregion

    #region 攻击模块

    private void Attack()
    {
        allowMove = false;
        allowAttack = false;
        anim.Play("Attack1");
        StartCoroutine(StartDelaySetBool(1.5f,"Attack"));
    }

    public void AttackEndHandle()
    {
        allowMove = true;
    }

    public void AttackHit()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(acPoint.position,radius,enemyLayer);

        if(hits.Length <= 0) return;

        foreach(var hit in hits)
        {
            EnemyController enemy = hit.GetComponent<EnemyController>();
            Debug.Log(enemy.gameObject.name);
            if(enemy != null)
            {   
                enemy.UpdateHP(-playerSO.damage);
            }
        }
    }
    #endregion

    #region 强化攻击模块
    private void EnhancedAttack()
    {
        allowMove = false;
        allowAttack = false;
        allowGuard = false;
        anim.Play("Attack2");
        StartCoroutine(StartDelaySetBool(3,"Attack"));
        StartCoroutine(StartDelaySetBool(3,"Guard"));
    }

    public void EnhancedAttackEndHandle()
    {
        allowMove = true;
    }
    #endregion


    private IEnumerator StartDelaySetBool(float time,string flag)
    {
        yield return new WaitForSeconds(time);

        switch(flag)
        {
            case "Guard" : allowGuard = true;
            break;
            case "Attack" : allowAttack = true;
            break;
        }
    }
}
