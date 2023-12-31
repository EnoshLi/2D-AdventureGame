
using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour
{
    [Header("基本属性")]
    //最大生命值
    public float maxHealth;

    //当前生命值
    public float currentHealth;

    //最大耐力值
    public float maxPower;

    //当前耐力值
    public float currentPower;

    //耐力回复速度
    public float powerRecoverSpeed;

    [Header("状态")]
    //无敌时间
    public float invulnerableDuration;

    //计时器
    public float invulnerableCounter;

    //无敌
    public bool invulnerable;

    //伤害事件
    public UnityEvent<Transform> OnTakedamage;

    //死亡事件
    public UnityEvent OnDie;

    //生命UI变化事件
    public UnityEvent<Character> OnHealthChange;

    private void Start()
    {
        currentHealth = maxHealth;
        currentPower = maxPower;
        OnHealthChange?.Invoke(this);
    }

    private void Update()
    {
        if (invulnerable) 
        {
            invulnerableCounter -=Time.deltaTime;
            if (invulnerableCounter <= 0) 
            {
                invulnerable = false;
            }
        }
        if (currentPower < maxPower)
            currentPower += Time.deltaTime * powerRecoverSpeed;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Water"))
        {
            //死亡
            currentHealth = 0;
            OnHealthChange?.Invoke(this);
            OnDie?.Invoke();
        }
    }
    /// <summary>
    /// 触发无敌帧
    /// </summary>
    private void TriggerInvulnerable() 
    {
        if (!invulnerable)
        {
            invulnerable = true;
            invulnerableCounter = invulnerableDuration ;
        }
    }
    /// <summary>
    /// 受到伤害
    /// </summary>
    /// <param name="attacker"></param>
    public void TakeDamage(Attack attacker)
    {
        //Debug.Log(attacker.damage);
        if (invulnerable)
            return;
        if (currentHealth - attacker.damage > 0)
        {
            currentHealth -= attacker.damage;
            TriggerInvulnerable();
            //执行受伤
            OnTakedamage?.Invoke(attacker.transform);
        }
        else
        {
            currentHealth = 0;
            //触发死亡
            OnDie?.Invoke();
        }
        OnHealthChange?.Invoke(this);

    }
    public void Onslide(int cost)
    {
        currentPower -= cost;
        OnHealthChange?.Invoke(this);
    }
}
