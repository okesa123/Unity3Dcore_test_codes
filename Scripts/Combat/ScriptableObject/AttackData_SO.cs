using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAttcak" ,menuName = "Character States/AttackData")]
public class AttackData_SO : ScriptableObject

{
   [Header("近战属性")]
   public float attckRange;
   public float damage_Attack;

   [Header("技能属性")]
   public bool hasSkill;
   public float skillRange;
   public float damage_Skill;

   [Header("攻击cd")]
   public float coolDown;

   [Header("暴击相关")]
   
   /// <summary>
   /// 暴击伤害倍率
   /// </summary>
   public float criticalMultiplier;
   /// <summary>
   /// 暴击率
   /// </summary>
   public float criticalRate;
}
