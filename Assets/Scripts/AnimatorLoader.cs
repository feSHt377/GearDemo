using System;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Animations;
using UnityEngine;
//using UnityEngine.UIElements;

/// <summary>
/// only use on Main Camera!!!!!!!!!!!!!!!!
/// </summary>
public class AnimatorLoader : MonoBehaviour
{

    //default gun viewmodel settle!!!
    [Header("Default Viewmodel Offset")]
    public Vector3 position = new Vector3(0.1f, -0.07f, 0.15f);
    public Vector3 rotation = new Vector3(-4.33f, 0f, 0);
    public Vector3 scale = Vector3.one;

    //[Header("Default Animation Controller Path")]
    string acp = "AnimationController/";


    public static GameObject weaponPrefab;
    public static GameObject armPrefab;
    public static GameObject bulletPrefab;

    // Start is called before the first frame update
    void Start()
    {
        //weapon_pist_deagle
        //weapon_knife_m9
        //loadPrefab("weapon_knife_m9");
        loadPrefab("weapon_pist_deagle");
        //animator.runtimeAnimatorController = AnimatorController.CreateAnimatorControllerAtPath("Assets/AnimationController/knife/m9.controller");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            updateWeaponPrefab("weapon_rif_ak47");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) {
            updateWeaponPrefab("weapon_pist_deagle");
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            updateWeaponPrefab("weapon_knife_m9");
        }
    }


    //在调用此函数之前务必确认两核心预制体已加载！！！！ 
    void updateWeaponPrefab(string weaponPrefabPath) {
        string animationName = weaponPrefabPath;//动画Animation Controller与prefab文件同名

        //Destroy(weaponPrefab.GetComponent<Animator>());
        Destroy(weaponPrefab);

        loadPrefab(weaponPrefabPath);
    }

    void loadPrefab(string weaponPrefabPath)
    {
        //指定预制体在Resources文件夹下的路径，不需要文件扩展名
        //string weaponPrefabPath = "weapon_knife_m9"; // 例如: Resources/Weapons/Pistol.prefab

        string animationName = weaponPrefabPath;//动画Animation Controller与prefab文件同名

        string armPrefabPath = "Arm";
        
        bool needLoadArmPrefab = true;


        if (armPrefab != null) needLoadArmPrefab = false;

        //加载预制体资源
        GameObject weaponPrefabToLoad = Resources.Load<GameObject>(weaponPrefabPath);
        GameObject armPrefabToLoad;
        GameObject bulletShellPrefabToLoad;
        if (bulletPrefab == null)
        {
            bulletShellPrefabToLoad = Resources.Load<GameObject>("Scene/BulletShell");
            bulletPrefab = Instantiate(bulletShellPrefabToLoad);
            bulletPrefab.SetActive(false);//隐藏弹壳预制体
            
            Debug.Log("loaded bulletshell.");
        }


        //检查是否加载成功
        if (weaponPrefabToLoad != null)
        {
            
            if (needLoadArmPrefab)
            {
                Debug.Log("NEED LOAD THE ARM MODEL!!!!!!!!!!");
                //实例化预制体，并将当前Arm物体设为父物体
                armPrefabToLoad = Resources.Load<GameObject>(armPrefabPath);
                armPrefab = Instantiate(armPrefabToLoad, this.transform);

                //重置实例的本地坐标和旋转，使其与父物体对齐 仅在初始化时才设置。
                armPrefab.transform.localPosition = position;
                armPrefab.transform.localRotation = Quaternion.Euler(rotation);

            }
            
            //将arm模型设置为枪械的子组件
            weaponPrefab = Instantiate(weaponPrefabToLoad, armPrefab.transform);


            

            //weaponPrefab.transform.localPosition = position;

            //weaponPrefab.transform.localRotation = Quaternion.Euler(rotation);

            //特殊操作，将arm模型的动画根节点更名为于weapon动画根节点相同，这样才可以正常播放动画

            GameObject weaponAnimationRootNode = findRootNodeName(weaponPrefab);//获取根节点
            GameObject armAnimationRootNode = findRootNodeName(armPrefab);//获取根节点

            armAnimationRootNode.name = weaponAnimationRootNode.name;//绑定动画的核心代码


            loadAndBindAnimation(weaponPrefab, animationName);

            loadAndBindAnimation(armPrefab, animationName);

            Debug.Log("预制体加载并附加成功！");
        }
        else
        {
            Debug.LogError($"无法在路径 '{weaponPrefabPath}' 找到预制体。请检查路径和Resources文件夹。");
        }
    }

    void loadAndBindAnimation(GameObject weaponModelComponent, string ACName)
    {
        try
        {
            string path = acp + ACName;
            // 从Resources文件夹加载Controller，注意不需要文件扩展名
            RuntimeAnimatorController loadedController = Resources.Load<RuntimeAnimatorController>(path);
            if (loadedController == null) Debug.Log($"{path}Controller is null");

            Animator weapon_atr = weaponModelComponent.GetComponent<Animator>();
            if (weapon_atr == null) Debug.Log("Animator is null");

            weapon_atr.runtimeAnimatorController = loadedController;

            Debug.Log($"Animation Controller<{ACName}> loaded successfully.");
        }
        catch (Exception e) {
            Debug.Log($"Exception from loading Animation Controller: {ACName}");
            Debug.LogException(e);
        }
    }

    GameObject findRootNodeName(GameObject comp)
    {
        // 获取所有子级（包括自身）的Transform组件
        Transform[] allChildrenWeapon = comp.GetComponentsInChildren<Transform>();
        //string RootNodeName;

        foreach (Transform child in allChildrenWeapon)
        {
            // 打印每个子物体的名称
            //Debug.Log("子物体名称: " + child.gameObject.name);
            if (child.gameObject.name.EndsWith(".vmdl_c"))
            {
                return child.gameObject;
            }
            //if (child.gameObject.name == "Arm") child.gameObject.name = "Arm";
        }
        return null;
    }
}
