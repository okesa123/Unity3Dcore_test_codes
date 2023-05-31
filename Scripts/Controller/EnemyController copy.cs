using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;



public enum EnemyStates { GUARD, PATROL, CHASE, DEAD }
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CharacterStates))]
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Animator))]
public class EnemyController : MonoBehaviour,IEndGameObserver
{
    #region 新增的Attack类与列表Attacks
    protected class Attack
    {
        public string AttackName {get;set;}
        public float AttackRange {get;set;}
    }
    Attack normalAttack;
    Attack skillAttack;
    
    List<Attack> Attacks = new List<Attack>();
    #endregion



    CharacterStates characterStates;
    /// <summary>
    /// 计时器，记录驻留观察时间
    /// </summary>
    private float counter_LookAtTime;
    /// <summary>
    /// 计时器，记录上一次攻击的时间
    /// </summary>
    private float counter_LastAttackTime;
    private Vector3 randomWayPoint;
    private Vector3 originalWayPoint;
    private EnemyStates enemyStates;
    protected Animator animator_Enemy;
    private NavMeshAgent nav_Enemy;
    protected GameObject attackTarget;
    private bool playerDead;
    private Quaternion originalRotation;
    private BoxCollider boxCollider;
    /* 配合动画的bool值 */
    bool isChasing;
    bool foundPlayer;
    bool isWalking;
    bool isLookingAround;
    bool isDead;

    [Header("EnemySettings")]
    [SerializeField] float sightRadius;
    [SerializeField] float chasingSpeed = 5f;

    [Header("PatrolSettings")]
    [SerializeField] bool isGuarding;
    [SerializeField] float patrolSpeed;
    [SerializeField] float patrolRadius;
    [SerializeField] float lookAtTime;
    protected string attackType;

    private void Awake()
    {
        nav_Enemy = GetComponent<NavMeshAgent>();
        animator_Enemy = GetComponent<Animator>();
        originalWayPoint = this.transform.position;
        originalRotation = this.transform.rotation;
        counter_LookAtTime = lookAtTime;
        characterStates = GetComponent<CharacterStates>();
        boxCollider = GetComponent<BoxCollider>();

        
        #region 初始化normalAttack和skillAttack
        Attacks.Add(normalAttack);
        normalAttack.AttackName = "NormalAttack";
        normalAttack.AttackRange = characterStates.attackData.attckRange;
        
        if(characterStates.attackData.hasSkill!= false)
        {
           skillAttack.AttackRange = cha
        }

        #endregion

    }

    private void Start()
    {
        
        if (isGuarding)
        {
            enemyStates = EnemyStates.GUARD;
        }
        else
        {
            enemyStates = EnemyStates.PATROL;
            GetNewWayPoint();
        }
        //FIXME:在切换环境的时候修改
        GameManager.Instance.AddObserver(this);
    }
    
    // 切换场景的时候启用
    //  private void OnEnable() 
    //  {
    //     GameManager.Instance.AddObserver(this);
    // }

     private void OnDisable() 
     {
        if(!GameManager.IsInitialized) return;
         GameManager.Instance.RemoveObserver(this);
     }

    private void Update()
    {

        if(characterStates.CurrentHP <= 0) isDead = true;
        if(!playerDead)
        {
             SwitchStates();
             SwitchAnimation();
             counter_LastAttackTime -= Time.deltaTime;
        }
       
        //Debug.Log(nav_Enemy.stoppingDistance);
        //Debug.Log(Vector3.Distance(randomWayPoint,this.transform.position));
        

        
        
        

    }

    private void SwitchAnimation()
    {
        foundPlayer = FoundPlayer();
        /* 设置各类动画 */
        animator_Enemy.SetBool("isChasing", isChasing);
        animator_Enemy.SetBool("foundPlayer", foundPlayer);
        animator_Enemy.SetBool("isWalking", isWalking);
        animator_Enemy.SetBool("isLookingAround", isLookingAround);
        animator_Enemy.SetBool("isCritical", characterStates.isCritical);
        animator_Enemy.SetBool("isDead", isDead);


    }

    void SwitchStates()
    {
        if(isDead)
        {
            enemyStates = EnemyStates.DEAD;
        }
        else if (FoundPlayer())
        {
            enemyStates = EnemyStates.CHASE;
        }
        switch (enemyStates)
        {

            case EnemyStates.GUARD://站桩守卫
                GuardState();
                break;
            case EnemyStates.PATROL://巡逻
                PatrolState();
                break;
            case EnemyStates.CHASE:
                ChaseState();
                break;
            case EnemyStates.DEAD:
                DeadState();
                break;
        }
    }

    private void DeadState()
    {
        nav_Enemy.enabled = false;
        boxCollider.enabled = false;
        Destroy(this.gameObject,2f);

    }

    private void GuardState()
    {
         isLookingAround = true;
         isWalking = true;
         nav_Enemy.isStopped = false;
         nav_Enemy.destination = originalWayPoint;

         if(Vector3.SqrMagnitude(originalWayPoint - this.transform.position) 
                                                 <= nav_Enemy.stoppingDistance)
         {
              nav_Enemy.isStopped = true;
              isWalking = false;
              counter_LookAtTime = lookAtTime;
              //Quaternion(a,b,t) 以一个量化的[0,1]的t值由a角度转向b角度
              this.transform.rotation = Quaternion.Lerp(this.transform.rotation,originalRotation,0.05f);
              
         }
            
         
    }

    private void PatrolState()
    {
        isChasing = false;
        nav_Enemy.speed = patrolSpeed;
        if (Vector3.Distance(randomWayPoint, this.transform.position) <= nav_Enemy.stoppingDistance)
        {

            isWalking = false;
            if (counter_LookAtTime > 0)
            {
                isLookingAround = true;
                nav_Enemy.destination = this.transform.position;
                counter_LookAtTime -= Time.deltaTime;
            }
            else
            {
                isLookingAround = false;
                GetNewWayPoint();
            }

        }
        else
        {
            isWalking = true;
            nav_Enemy.destination = randomWayPoint;

        }
    }


    void ChaseState()
    {
        if (!FoundPlayer())
        {
            isChasing = false;
            if (counter_LookAtTime > 0)
            {
                isLookingAround = true;
                counter_LookAtTime -= Time.deltaTime;
                nav_Enemy.destination = this.transform.position;
            }
            else
            {
                nav_Enemy.isStopped = false;
                isLookingAround = false;
                isWalking = true;
                if (isGuarding)
                {
                    enemyStates = EnemyStates.GUARD;
                }
                else
                {
                    enemyStates = EnemyStates.PATROL;
                }
            }
            //回到上一个状态
        }
        else//即FoundPlayer是True;
        {
            isWalking = false;
            nav_Enemy.isStopped  = false;
            if (TargetInAttackRange() || TargetInSkillRange())
            {
                //进入攻击范围，停止追逐开始攻击
                isChasing = false;
                nav_Enemy.isStopped = true;
                Attack();
            }
            else
            {
                //追逐
                ChasingPlayer();
            }
        }
    }

    /// <summary>
    /// 判断视野范围内是否有玩家
    /// </summary>
    /// <returns>true找到玩家，false未找到</returns>
    bool FoundPlayer()
    {

        //(a,b)以a为中心点的b半径内的所有colliders的数组为nearbys
        var nearbys = Physics.OverlapSphere(this.transform.position, sightRadius);
        foreach (var target in nearbys)
        {
            if (target.gameObject.CompareTag("Player"))
            {
                attackTarget = target.gameObject;
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 追逐玩家
    /// </summary>
    void ChasingPlayer()
    {
        transform.LookAt(attackTarget.transform);
        isChasing = true;
        nav_Enemy.speed = chasingSpeed;
        nav_Enemy.destination = attackTarget.transform.position;
        
    }

    void Attack()
    {
        if (counter_LastAttackTime < 0)
        {
            counter_LastAttackTime = characterStates.attackData.coolDown;
            //暴击判断
            //UnityEngine.Random.value 是 随机产生一个0-1中的数
            this.transform.LookAt(attackTarget.transform);
            characterStates.isCritical = UnityEngine.Random.value <= characterStates.attackData.criticalRate;
            if (TargetInAttackRange())
            {
                //FIXME:只会进行普通攻击
                attackType = "Attack";
                //近距离攻击
                animator_Enemy.SetTrigger("Attack");
            }
        }
    }



    void GetNewWayPoint()
    {
        counter_LookAtTime = lookAtTime;
        float randomX = UnityEngine.Random.Range(-patrolRadius, patrolRadius);
        float randomZ = UnityEngine.Random.Range(-patrolRadius, patrolRadius);
        randomWayPoint = new Vector3(originalWayPoint.x + randomX,
                            this.transform.position.y, originalWayPoint.z + randomZ);
        NavMeshHit hitPoint;
        //防止随机点选到不可行走的区域
        //1是指navmeshagent中walkable的图块
        randomWayPoint = NavMesh.SamplePosition(randomWayPoint, out hitPoint, patrolRadius, 1) ? hitPoint.position : this.transform.position;
    }

    bool TargetInAttackRange()
    {
        if (attackTarget != null)
            return Vector3.Distance(attackTarget.transform.position, this.transform.position) <= characterStates.attackData.attckRange;
        else
            return false;
    }

    bool TargetInSkillRange()
    {
        if (attackTarget != null)
            return Vector3.Distance(attackTarget.transform.position, this.transform.position) <= characterStates.attackData.skillRange;
        else
            return false;
    }








    /// <summary>
    /// 画出巡逻范围
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, patrolRadius);
    }


    //Animation Event
    void Hit_FormEnemy(string attackTypeAnimation)
    {
        if (attackTarget != null)

        {
            if (attackTypeAnimation == null)
                throw new NotImplementedException("没有该类型攻击");

            attackTypeAnimation = attackType;
            var targetStates = attackTarget.GetComponent<CharacterStates>();
            targetStates.TakeDamage(characterStates, attackType, targetStates);
        }

    }

    public void EndNotify()
    {
        //TODO:获胜动画
        //TODO:停止移动
        //TODO:停止agent
        Debug.Log("endGame");
        animator_Enemy.SetBool("isWin",true);
        isChasing = false;
        isWalking = false;
        attackTarget = null;
        playerDead = true;
    }

    void GetRandomAttackType()
    {

    }

    private void NormalAttack()
    {

    }


}


