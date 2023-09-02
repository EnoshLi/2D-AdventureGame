using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    //刚体
    protected Rigidbody2D rb;

    //动画
    [HideInInspector]public Animator animator;

    //碰撞检测
    [HideInInspector]public PhysicsCheck physicsCheck;

    [Header("基本参数")]
    //速度
    public float normalSpeed;

    //冲刺速度
    public float chaseSpeed;

    //当前速度
    [HideInInspector] public float currentSpeed;

    //方向
    public Vector3 faceDir;

    //方向力
    public float hurtForce;

    //攻击者
    public Transform attcker;
    [Header("计时器")]
    //等候时间
    public float waitTime;

    //等待计时器
    public float waitTimeCounter;

    [Header("状态")]

    //等待状态
    public bool wait;

    //受伤状态
    public bool isHurt;

    //死亡状态
    public bool isDead;

    //当前状态
    private BaseState currentState;

    //巡逻状态
    protected BaseState patrolState;

    //冲锋状态
    protected BaseState chaseState;

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        physicsCheck = GetComponent<PhysicsCheck>();
        currentSpeed = normalSpeed;
        waitTimeCounter = waitTime;

    }
    private void OnEnable()
    {
        currentState = patrolState;
        currentState.onEnter(this);
    }

    private void Update()
    {
        
        faceDir = new Vector3(-transform.localScale.x, 0, 0);      
        currentState.Logicupdate();
        TimeCounter();
    }
    private void FixedUpdate()
    {
        

        if(!isHurt && !isDead && !wait)
            Move();
        
        currentState.PhysicsUpdate();
    }
    private void OnDisable()
    {
        currentState.OnExit();
    }

    /// <summary>
    /// 移动
    /// </summary>
    public virtual void  Move()
    {

        rb.velocity = new Vector2(currentSpeed * faceDir.x * Time.deltaTime, rb.velocity.y);
    }

   /// <summary>
   /// 计时器
   /// </summary>
    public void TimeCounter()
    {
        
        if (wait)
        { 
            waitTimeCounter -= Time.deltaTime;
            if (waitTimeCounter <= 0)
            { 
                wait = false;
                waitTimeCounter = waitTime;
                transform.localScale = new Vector3(faceDir.x, 1, 1);
            }
        }
    }
    public void OnTakeDamage(Transform attackTrans)
    {
        attcker = attackTrans;

        //转身
        if (attackTrans.position.x - transform.position.x > 0)
            transform.localScale = new Vector3(-1,1,1);
        if(attackTrans.position.x - transform.position.x < 0)
            transform.localScale = new Vector3(1, 1, 1);

        //受伤被击退
        isHurt = true;
        animator.SetTrigger("Hurt");
        Vector2 dir = new Vector2(transform.position.x - attackTrans.position.x,0).normalized;
        rb.AddForce(dir*hurtForce,ForceMode2D.Impulse);

        //启动协程
        StartCoroutine(OnHurt(dir));
    }

    /// <summary>
    /// 怪物击退
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    private IEnumerator OnHurt(Vector2 dir)
    {
        rb.AddForce(dir * hurtForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.45f);
        isHurt = false;
    }

    /// <summary>
    /// 怪物死亡
    /// </summary>
    public void OnDie()
    {
        gameObject.layer = 2;
        animator.SetBool("Dead",true);
        isDead = true;
    }

    public void DestroyAfterAnimation()
    {
        Destroy(this.gameObject);
    }
}
