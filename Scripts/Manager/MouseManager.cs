using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/* 序列化创建事件，但是嘛，由于在多个不同关卡中都要反复注册不方便拖拽，于是用代码创建事件 */
// [System.Serializable]
// public class MouseEventVector3:UnityEvent<Vector3>{}

public class MouseManager : Singleton<MouseManager>
{
   
   RaycastHit hitInfo;
   public event Action<Vector3> OnMouseClickedGround;
   public event Action<GameObject> OnMouseClickedEnemy;
   

   [SerializeField] private Texture2D onMoving,point,Attack,arrow,leave;


   //重写singleton中的Awake方法
   protected override  void Awake() 
   {
     base.Awake();
     DontDestroyOnLoad(this);
   }

   private void Update() 
   {
     SetCursorTexture();
     MouseControl();
   }

    private void MouseControl()
    {
        if(Input.GetMouseButtonDown(0)&& hitInfo.collider!=null)
        {
          if(hitInfo.collider.gameObject.CompareTag("Ground"))
            OnMouseClickedGround?.Invoke(hitInfo.point);
          if(hitInfo.collider.gameObject.CompareTag("Enemy"))
             OnMouseClickedEnemy?.Invoke(hitInfo.collider.gameObject);
             
          
            
        }
    }

    private void SetCursorTexture()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);//向鼠标点击的地方发射ray
         if(Physics.Raycast(ray,out hitInfo))//将ray打到的物体的中的信息传输到HitInfo中
         {
             switch(hitInfo.collider.gameObject.tag){
               case "Ground":
               Cursor.SetCursor(onMoving,new Vector2(16,16),CursorMode.Auto);
               break;
               case "Enemy":
               Cursor.SetCursor(Attack,new Vector2(16,16), CursorMode.Auto);
               break;
             }
            
         }
    }


}
