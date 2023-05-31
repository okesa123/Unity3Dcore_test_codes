using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*


*/
///<summary>
///
///</summary>
public class CharacterStates : MonoBehaviour
{
   public CharacterData_SO templateData;
   CharacterData_SO characterData;
   public AttackData_SO attackData;
   [HideInInspector] public bool isCritical;
   
   
   private void Awake() 
   {
      if(templateData!= null)
      characterData = Instantiate(templateData);
   }
   //属性的写法
   #region Read From Data_SO
    
   public int MaxHP
   {
      get  { if(characterData!= null)  return characterData.maxHP;  else return 0;} 
      set  {  characterData.maxHP = value; }
   }

   public int CurrentHP
   {
      get  { if(characterData!= null)  return characterData.currentHP;  else return 0;} 
      set  {  characterData.currentHP = value; }
   }

   public int BaseDefence
   {
      get  { if(characterData!= null)  return characterData.baseDefence;  else return 0;} 
      set  {  characterData.baseDefence = value; }
   }

   public int CurrentDefence
   {
      get  { if(characterData!= null)  return characterData.currentDefence;  else return 0;} 
      set  {  characterData.currentDefence = value; }
   }

   
   
   #endregion
   
   #region Character Combat
   
   public void TakeDamage(CharacterStates attacker,string attackType,CharacterStates defener)
   {
      int damage  = Mathf.Max(attacker.CurrentDamage(attackType) - defener.CurrentDefence,0);//Mathf.max(a,b)返回两个数里面的最大值
      CurrentHP = Mathf.Max(CurrentHP - damage,0);
      

      if(attacker.isCritical)
      {
         defener.GetComponent<Animator>().SetTrigger("GetHit");
      }
      //TODO:经验值更新
      //TODO:UI设计
   }

    

    private int CurrentDamage(string attackType)
    {
        float coreDamage;
        if(attackType == "Skill")
          coreDamage = attackData.damage_Skill;
        else 
        if(attackType == "Attack")
          coreDamage = attackData.damage_Attack;
        else
        throw new NotImplementedException("没有该类型攻击");

        if(isCritical)
        {
           Debug.Log("爆了" + coreDamage);
           coreDamage *= attackData.criticalMultiplier;
        }
       

        return (int)coreDamage;
    }

    


    #endregion
   

   #region 
   #endregion
}
