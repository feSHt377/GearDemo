using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
//using UnityEditor.UI;
using UnityEngine;
using UnityEngine.Rendering;



//Layer 8 为地面专用layer！！！！！！！
//Layer 8 为地面专用layer！！！！！！！
//Layer 8 为地面专用layer！！！！！！！
//Layer 8 为地面专用layer！！！！！！！

public class move : MonoBehaviour
{
    public CharacterController characterController;
    private CollisionFlags collisionFlags;


    float speed = 10;//persecond
    //position
    float inputX = 0;
    float inputY = 0;


    //Player
    Vector3 movement = Vector3.zero;

    [Header("Player Speed")]
    [Tooltip("walkSpeed")]public float walkSpeed = 10;
    [Tooltip("duckSpeed")]public float duckSpeed = 5;

    //[Header("Player Status")]
    public bool isJump;
    public static bool isGround;
    public float jumpForceSettle = 0.054f;
    public float gravitySettle = 0.035f;
    //public float ForceScale = 10f;
    public float jumpForce ;
    public float gravity;
    public Vector3 jump = Vector3.zero;
    public static Vector3 velocity = Vector3.zero;//当前速度





    //Layer 8 为地面专用layer！！！！！！！
    private Collider[] hitColliders = new Collider[100];//预创建碰撞体数组，防止触发gc



    //// 碰撞器模式：与物体发生物理碰撞
    //private void OnTriggerEnter(Collider collision)
    //{
    //    Debug.Log($"collision entering {collision.gameObject.name} layer : {collision.gameObject.layer}");
    //    isGround = true;
    //    isJump = false;
    //    //jumpForce = 0;
    //}

    
    //private void OnTriggerExit(Collider collision)
    //{
    //    Debug.Log($"collision exited!!! {collision.gameObject.name} layer : {collision.gameObject.layer}");

    //    if (collision.gameObject.layer == 0)
    //    {
    //        isGround = false;
    //        isJump = true;
    //    }
    //}


    void GroundCheck()
    {
        Transform foot = transform.Find("foot");
        SphereCollider foot_collider = foot.GetComponent<SphereCollider>();
        // 在当前对象位置为中心，半径为5的球体内检测所有碰撞体
        int numHits = Physics.OverlapSphereNonAlloc(foot.position, foot_collider.radius , hitColliders);
        //Debug.Log("Ground check");

        for (int i = 0; i < numHits; i++)
        {
            // 对每个检测到的碰撞体执行操作，例如打印名称
            Collider collider = hitColliders[i];
            //Debug.Log($"检测到物体: {collider.gameObject.name} , layer :{collider.gameObject.layer}");
            if (collider.gameObject != this.gameObject && collider.gameObject != foot.gameObject)
            { //不为玩家本身和足部的判定体，那么表示碰撞体含有新的数据，目前，我们判定所有layer为0的碰撞体为玩家可以踩上的“plane”
                //Debug.Log($"laned: {collider.gameObject.name} ");
                isGround = true;
                
                return;//直接结束判定
            }
        }

        isGround = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Hello World");
        characterController = GetComponent<CharacterController>();

        jumpForce = 0;
        gravity = gravitySettle;
        isGround = false;
        isJump = false;
    }

    // Update is called once per frame
    void Update()
    {

        
        GroundCheck();

        Jump();
        Move();

        
        if (Input.GetKey(KeyCode.Escape))
        {
            Debug.Log("quit....");
            Application.Quit();

            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif
        }




    }


    void Jump()
    {
        //Debug.Log($"Jump:{isJump}");
        if (Input.GetKey(KeyCode.Space))
        {
            if (!isJump && isGround)
            {
                //never jump.
                isJump = true;
                isGround = false;
                jumpForce = jumpForceSettle;
                Debug.Log("jump");
            }
            
        }
        if (!isGround)
        {
            jumpForce -= gravity * Time.deltaTime;
        }
        else
        {
            jumpForce = -0.006f;
            isJump = false;
        }
    //falling...

    jump = new Vector3(0, jumpForce, 0);

    characterController.Move(jump);
        //Debug.Log("flags  ::" + collisionFlags);

        //if (collisionFlags == CollisionFlags.Below)
        //{
        //    isJump = false;
        //    isGround = true;
        //    jumpForce = 0;
        //    //Debug.Log("grounded...");
        //}
        //else {
        //    isGround = false;
        //    //Debug.Log("air !!! grounded...");
        //}

    }


    void Move()
    {

        inputX = Input.GetAxis("Horizontal");
        inputY = Input.GetAxis("Vertical");

        velocity = characterController.velocity;

        movement = transform.right * inputX * speed * Time.deltaTime + transform.forward * inputY * speed * Time.deltaTime;
        //movement.y = -0.0001f;

        CollisionFlags collisionFlags1 = characterController.Move(movement);


       /* if (collisionFlags1 != CollisionFlags.Below)
        {
            isGround = false;
            Debug.Log("air!!!"+collisionFlags1);
        }*/
    }
}
