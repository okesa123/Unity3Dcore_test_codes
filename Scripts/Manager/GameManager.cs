using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*


*/
///<summary>
///
///</summary>
public class GameManager : Singleton<GameManager>
{
    public CharacterStates playerStates;

    List<IEndGameObserver> endGameObservers = new List<IEndGameObserver>();
    
    /// <summary>
    /// 反向注册，调用该方法的人注册为玩家的SO
    /// </summary>
    /// <param name="player">玩家的SO</param>
    public void RigisterPlayer(CharacterStates player)
    {
       playerStates = player;
    }
    
    public void AddObserver(IEndGameObserver observer)
    {
        endGameObservers.Add(observer);
    }

    public void RemoveObserver(IEndGameObserver observer)
    {
        endGameObservers.Remove(observer);
    }
    
    public void NotifyObservers()
    {
         foreach(var observer in endGameObservers)
         {
            observer.EndNotify();
         }
    }

    private  void Update() 
    {
        if(playerStates ==null)
        {
            throw new NotImplementedException("没有找到该玩家"); 
        }
        if(playerStates.CurrentHP <= 0)
        {
            NotifyObservers();
        }
    }

}
