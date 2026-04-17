using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlayerController : MonoBehaviour
{   
    [SerializeField] private PlayerStatsSO playerSO;
    private bool isFacingRight = true;
    private Vector2 moveInput;
    [SerializeField]private bool allowMove = true;
    [SerializeField]private bool allowGuard = true;
    [SerializeField]private bool allowAttack = true;
    private Animator anim;

    private void Start() 
    {
        anim = GetComponent<Animator>();
    }

    private void Update() 
    { 
        
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
        StartCoroutine(StartDelaySetBool(3,"Guard"));
    }

    public void GuardEndHandle()
    {
        allowMove = true;
    }

    #endregion

    #region 攻击模块

    private void Attack()
    {
        allowMove = false;
        allowAttack = false;
        anim.Play("Attack1");
        StartCoroutine(StartDelaySetBool(3,"Attack"));
    }

    public void AttackEndHandle()
    {
        allowMove = true;
    }
    #endregion

    #region 强化攻击模块
    private void EnhancedAttack()
    {
        allowMove = false;
        allowAttack = false;
        allowGuard = false;
        anim.Play("Attack2");
        StartCoroutine(StartDelaySetBool(6,"Attack"));
        StartCoroutine(StartDelaySetBool(6,"Guard"));
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
