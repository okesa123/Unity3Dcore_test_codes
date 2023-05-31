using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;


/*


*/
///<summary>
///
///</summary>
public class PlayerController : MonoBehaviour
{

  NavMeshAgent nav_Player;
  Animator playerAnimator;
  float counter_PlayerAttack;
  CharacterStates characterStates;
  GameObject attackTarget;
  String attackType;
  bool isDead;


  private void Awake() 
  {
    nav_Player = GetComponent<NavMeshAgent>();
    playerAnimator = GetComponent<Animator>();
    characterStates = GetComponent<CharacterStates>();
    
  }

  private void Start() {
    MouseManager.Instance.OnMouseClickedGround += MoveToTarget; 
    MouseManager.Instance.OnMouseClickedEnemy += EventAttack; 

    GameManager.Instance.RigisterPlayer(characterStates);
  }

   

    private void Update()
    {
        
           SwitchAnimation();
           counter_PlayerAttack += Time.deltaTime;

           
        isDead = characterStates.CurrentHP <= 0;

       
        
        
        

        
    }

    private void SwitchAnimation()
    {
        playerAnimator.SetBool("isDead",isDead);
        playerAnimator.SetFloat("PlayerSpeed", nav_Player.velocity.sqrMagnitude);//将Vector3值直接转化为浮点数
        playerAnimator.SetBool("isCritical",characterStates.isCritical);
    }

    public void MoveToTarget(Vector3 target)
  {
     
     StopAllCoroutines();//取消所有的携程(打断攻击动作)

     if(isDead) return;
     nav_Player.isStopped = false;
     nav_Player.destination = target;
  }

   private void EventAttack(GameObject target)
    {
        if(isDead)  return;
        if(target != null)
        {
          characterStates.isCritical = UnityEngine.Random.value <= characterStates.attackData.criticalRate;
          attackType = "Attack";
          attackTarget = target;
          StartCoroutine(MoveToAttackTarget());
        }
    }

    IEnumerator MoveToAttackTarget()
    {
        nav_Player.isStopped = false;

        transform.LookAt(attackTarget.transform);//看向攻击目标

        //当两者相差距离大时走向目标
        while (Vector3.Distance(attackTarget.transform.position, this.transform.position) > characterStates.attackData.attckRange)
        {
            
            nav_Player.destination = attackTarget.transform.position;
            yield return null;
        }

        //到达时停下
        nav_Player.isStopped = true;

        Attack();
    }
      
    private void Attack()
    {
        
        if (counter_PlayerAttack > characterStates.attackData.coolDown)
        {
            playerAnimator.SetTrigger("Attack");
            counter_PlayerAttack = 0f;
        }
    }

    //Animation Event动画事件
    void Hit_FromPlayer(string attackTypeAnimation)
    {
         if(attackTypeAnimation == null)
         throw new NotImplementedException("没有该类型攻击"); 

         attackTypeAnimation = attackType;
         var targetStates = attackTarget.GetComponent<CharacterStates>();
         targetStates.TakeDamage(characterStates,attackType,targetStates);
    }


}
