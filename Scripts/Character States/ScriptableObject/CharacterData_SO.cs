using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*


*/
///<summary>
///
///</summary>

[CreateAssetMenu(fileName = "NewData",menuName = "Character States/Data")]
public class CharacterData_SO : ScriptableObject
{
  [Header("States Info")]
  
  /// <summary>
  /// 最大生命值
  /// </summary>
  public int maxHP;
  
  /// <summary>
  /// 当前生命值
  /// </summary>
  public int currentHP;
  /// <summary>
  /// 基本防御
  /// </summary>
  public int baseDefence;
  /// <summary>
  /// 当前防御
  /// </summary>
  public int currentDefence;
}
