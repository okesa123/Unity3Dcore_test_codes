using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 泛型单例模式的写法
/// </summary>
/// <typeparam name="T">任意类型的类型</typeparam>

/* Singleton<T>是一个继承自Unity的MonoBehaviour类的泛型类。它是一个单例类，意味着在程序运行期间只能存在一个实例。

where T : Singleton<T>这部分是对泛型类型参数T进行约束的语法。它的意思是泛型类型T必须是Singleton<T>的派生类或者是Singleton<T>本身。也就是说，只有继承自Singleton<T>的类才能作为类型参数传递给Singleton<T>类。

通过这种方式，使用者可以通过继承Singleton<T>类来创建自己的单例类，并且保证在程序运行期间只会有一个实例存在。这是一种常见的设计模式，用于确保某些类的唯一性和全局可访问性。 */
public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
   private static T instance;
   
   
   public static T Instance
   {
      get { return instance;}
   }
   
   //protected可以被子类继承   virtual可以在子类中被重写
   protected virtual void Awake() 
   {
       if(instance !=null)
       Destroy(this.gameObject);
       else 
           instance = (T)this;//不同类型的实例都可以进来
   }
   
   /// <summary>
   /// 检测单例是否被唤醒(生成)
   /// </summary>
   /// <value>true已被唤醒，false未被唤醒</value>
   public static bool IsInitialized
   {
        get{return instance != null;}
   }

   protected virtual void  OnDestroy() 
   {
      if(instance == this)
      {
         instance = null;
      }
   }
}
