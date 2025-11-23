using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
//using UnityEditor.Animations;
//using UnityEditor.UI;
using UnityEngine;
using UnityGLTF;
using Random = UnityEngine.Random;



public class gunAnimation : MonoBehaviour
{





    string[] gunAnimationName =
    {
        "firstperson_shoot1",
        "firstperson_shoot2",
        "firstperson_draw",
        "firstperson_reload",
        "firstperson_lookat01_start",
        "firstperson_lookat02_start"
    };

    //int[]


    private ArrayList gunAnimationHashs = new ArrayList();//在加载时计算hash值并存入，优化每次读取动画的性能

    enum GunAnimation
    {
        shoot,
        shoot2,
        draw,
        reload,
        lookat1,
        lookat2
    };


    Animator animator;//Gun animator
    Animator armAnimator;//Arm animator


    //[Header("冲量设置")]
    public Vector3 baseImpulse = new Vector3(1f, 1f, 0.2f); // 基础冲量方向（基于枪械的局部坐标系）
    public float impulseStrength = 0.8f; // 冲量大小
    public float randomFactor = 0.1f; // 随机因子，让每次抛壳略有不同，更真实

    //Rigidbody bulletShell_Rigidbody;// 弹壳的Rigidbody组件引用



    // Start is called before the first frame update
    void Start()
    {
        
        //setup
        armAnimator = AnimatorLoader.armPrefab.GetComponent<Animator>(); //GetComponent<Animator>();//父节点
        animator = AnimatorLoader.weaponPrefab.GetComponent<Animator>(); //GetComponentsInChildren<Animator>()[1];

        Debug.Log("子物体名称: " + armAnimator.gameObject.name);

        //foreach (Animator child in armAnimator)
        //{
        //    // 打印每个子物体的名称
        //    Debug.Log("子物体名称: " + child.gameObject.name);
        //}

        //Debug.Log($"Animator is loaded. Weapon: {animator.name},  Arm: {armAnimator.name}");


        //subAnimator = GetComponentInChildren()<>;


        gunAnimationHashs.Clear();
        foreach (string name in gunAnimationName)
        {
            gunAnimationHashs.Add(Animator.StringToHash(name));
        }
        Debug.Log("Animation Hashs have been loaded.  Length:" + gunAnimationHashs.Count.ToString());
        Debug.Log("AC Name:  " + transform.name);
    }

    // Update is called once per frame
    void Update()
    {
        
        //Debug.Log($"WeaponAnimator Status : {animator.IsDestroyed()}");

        //切换武器时会自动删除武器组件，此时animator需要重载
        if (animator.IsDestroyed())
        {
            //reload the animator..
            Debug.Log("Weapon Animator reloaded..!!");

            animator = GetComponentsInChildren<Animator>()[1];
            
        }

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0); // 0是动画层的索引，通常用0即可
        //Debug.Log("animation status : " + stateInfo.IsName("firstperson_draw").ToString());


        //if(stateInfo.IsName("firstperson_draw")) animator.SetTrigger("draw");

        if (Input.GetKeyDown(KeyCode.Q))
        {
            animator.Play(getAnimationHash(GunAnimation.draw), -1, 0);
            armAnimator.Play(getAnimationHash(GunAnimation.draw), -1, 0);
            //animator.SetTrigger("draw");

        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            //animator.Play(getAnimationHash(GunAnimation.reload));
            //armAnimator.Play(getAnimationHash(GunAnimation.reload));
            animator.SetTrigger("reload");
            armAnimator.SetTrigger("reload");
            //animator.SetBool("draw", true);
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            animator.SetTrigger("lookat");
            armAnimator.SetTrigger("lookat");
            //animator.Play(getAnimationHash(GunAnimation.lookat1));
            

        }


        if (Input.GetMouseButtonDown(0))
        {
            //shoot
            AnimatorLoader.bulletPrefab.SetActive(true);
            AnimatorLoader.bulletPrefab.transform.position = AnimatorLoader.weaponPrefab.transform.position + Rotation.right * 0.2f + Rotation.forward * 0.5f + Rotation.up * 0.1f;
            ApplyEjectionImpulse(AnimatorLoader.bulletPrefab.GetComponent<Rigidbody>(), Rotation.right , move.velocity);

            animator.Play(getAnimationHash(GunAnimation.shoot), -1, 0);
            armAnimator.Play(getAnimationHash(GunAnimation.shoot), -1, 0);
        }
        if (Input.GetMouseButtonDown(1))
        {
            animator.Play(getAnimationHash(GunAnimation.shoot2), -1, 0);
            armAnimator.Play(getAnimationHash(GunAnimation.shoot2), -1, 0);
        }


    }


    int getAnimationHash(GunAnimation gunEnumID)
    {
        return (int)gunAnimationHashs[(int)gunEnumID];
    }


    void ApplyEjectionImpulse(Rigidbody bulletShell_Rigidbody, Vector3 impluseDirection , Vector3 playerVlocity)
    {
        if (bulletShell_Rigidbody != null)
        {
            bulletShell_Rigidbody.velocity = playerVlocity*2; // 重置速度，确保每次施加冲量时效果一致
            // 1. 计算一个带随机性的冲量方向，避免每次抛壳都一样
            Vector3 randomVariation = new Vector3(
                Random.Range(-randomFactor, randomFactor),
                Random.Range(-randomFactor, randomFactor),
                Random.Range(-randomFactor, randomFactor)
            );
            Vector3 finalImpulseDirection = (impluseDirection + randomVariation).normalized;

            // 2. 计算最终的冲量矢量
            Vector3 impulseVector = finalImpulseDirection * impulseStrength;

            // 3. 施加冲量！关键一步
            bulletShell_Rigidbody.AddForce(impulseVector, ForceMode.Impulse);

            // （可选）4. 同时可以施加一个扭矩（旋转力），让弹壳旋转起来
            Vector3 torque = new Vector3(
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f)
            ) * impulseStrength * 2.4f; // 旋转力度可以小一些
            bulletShell_Rigidbody.AddTorque(torque, ForceMode.Impulse);
        }
        else
        {
            Debug.LogWarning("在弹壳上未找到Rigidbody组件，无法施加冲量。");
        }
    }

 
}
