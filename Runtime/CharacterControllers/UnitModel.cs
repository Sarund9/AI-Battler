using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class UnitModel : MonoBehaviour
{
    [SerializeField]
    Rigidbody body;

    [SerializeField]
    Team team;
    [SerializeField]
    bool isLeader;

    [SerializeField]
    float attackRange = 1.3f;

    [SerializeField, Range(1, 10), FormerlySerializedAs("STR")]
    int str = 5; //Damage

    [SerializeField, Range(1, 10), FormerlySerializedAs("REG")]
    int regen = 5; //Regen

    [SerializeField, Range(1, 10), FormerlySerializedAs("CON")]
    int constitucion = 5; //MaxHP

    [SerializeField, Range(1, 10), FormerlySerializedAs("BRB")]
    int bravery = 5; //Bravery

    [SerializeField, Range(1, 10), FormerlySerializedAs("SPD")]
    int speed = 5; //MoveSpeed

    [SerializeField, Range(1, 10), FormerlySerializedAs("LCK")]
    int luck = 5; //Crit Chance - Crits do DoubleDamage

    [SerializeField]
    Weapon weapon;
    [SerializeField]
    GameObject shield;

    [SerializeField]
    float regenTime = 4f;
	
	[SerializeField]
	float invinTime = .1f;
	
    //Unit Cache
    IEnumerable<UnitModel> cache_Ally;
    IEnumerable<UnitModel> cache_Enemy;
    UnitModel leader;
    float dmgTime;

    public float CurrentHP { get; private set; }
    public bool Dead => CurrentHP <= 0;

    public int STR
    {
        get => str;
        set => str = value;
    }
    public int CON
    {
        get => constitucion;
        set => constitucion = value;
    }
    public int REG
    {
        get => regen;
        set => regen = value;
    }
    public int BRB
    {
        get => bravery;
        set => bravery = value;
    }
    public int SPD
    {
        get => speed;
        set => speed = value;
    }
    public int LCK
    {
        get => luck;
        set => luck = value;
    }

    public float AtkDamage => 5f + (STR * 5f) + weapon.Damage;
    public float MaxHP => 75f + (CON * 5f) + (shield is null ? 0f : 25f);
    public float FleeHP => 0.4f - (BRB * 0.03f);
    public float MoveSpeed => 2.5f + (SPD * .25f);
    public float CritChance => 0.1f + (LCK * 0.04f);
    public float RegenRate => 3f + (REG * 1.8f);
    public float AttackRange => attackRange;
    public Team Team => team;
    public float HealthNormal => CurrentHP / MaxHP;
    public bool IsLeader => isLeader;
    public Vector3 Velocity => body.velocity;

    public Weapon MainWeapon => weapon;
    public GameObject ShieldObj => shield;

    public event Action<float> OnTakeDamage;
    public event Action OnDeath;
    public event Action OnWin;

    private void Awake()
    {
        CurrentHP = MaxHP;

        if (body == null)
            body = GetComponent<Rigidbody>();

        var allUnits = FindObjectsOfType<UnitModel>();

        cache_Ally = allUnits
            .Where(u => u.team == team && u != this);
        cache_Enemy = allUnits
            .Where(u => u.team != team);
        
        leader = allUnits
            .FirstOrDefault(u => u.isLeader);
    }

    public IEnumerable<UnitModel> GetAllies()
    {
        return cache_Ally;
    }
    public IEnumerable<UnitModel> GetEnemies()
    {
        return cache_Enemy;
    }
    public UnitModel GetLeader()
    {
        return leader;
    }

    public void TakeDamage(DamageParams damage)
    {
        if (damage.team == team || dmgTime > invinTime)
            return;

        bool crit = UnityEngine.Random.value < damage.critChance;

        float ammount = damage.ammount * (crit ? 2 : 1);

        CurrentHP -= ammount;
        dmgTime = regenTime;
        //Debug.Log($"{team.ToString().ToUpper()} UNIT TAKES {ammount} DAMAGE");
        OnTakeDamage?.Invoke(ammount);
        if (CurrentHP <= 0)
        {
            Die();
        }
    }
    public void Update()
    {
        if (dmgTime < 0 && CurrentHP < MaxHP)
        {
            CurrentHP += RegenRate * Time.deltaTime;
            if (CurrentHP > MaxHP)
            {
                CurrentHP = MaxHP;
            }
        }
        else
        {
            dmgTime -= Time.deltaTime;
        }
    }
    public void Win()
    {
        OnWin?.Invoke();
    }

    public void Die()
    {
        Destroy(gameObject, 2.5f);
        if (isLeader)
            GameManager.Yield(Team);
        OnDeath?.Invoke();
    }

    //IEnumerator Timer(float time, Action action)
    //{

    //}
}

[Flags]
public enum DamageFlags
{
    None = 0,

    Critical,
}

public enum Team
{
    Blue,
    Red,
}