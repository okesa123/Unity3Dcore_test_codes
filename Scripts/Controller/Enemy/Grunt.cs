using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


/*


*/
///<summary>
///
///</summary>
public class Grunt : EnemyController
{
    [Header("Skill——KickOff")]
    public float KickForce;

    public void KickOff()
    {
        
        //attackType = "Skill";
        animator_Enemy.SetTrigger("Skill");
        this.transform.LookAt(attackTarget.transform);

        Vector3 kickDirc = attackTarget.transform.position - this.transform.position;
        kickDirc.Normalize();//将vector3类的kickDirc标准化

        attackTarget.GetComponent<NavMeshAgent>().isStopped = true;
        attackTarget.GetComponent<NavMeshAgent>().velocity = kickDirc*KickForce;
    }
}
