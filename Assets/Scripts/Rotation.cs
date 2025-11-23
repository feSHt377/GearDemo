using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Rotation : MonoBehaviour
{

    Transform playerBody;
    
    //rotation
    float yMax = 89;
    float yMin = -89f;
    float x = 0;
    float y = 0;
    float sensity = 400;
    float yRocation = 0;

    //mouse setup
    bool inited = false;
    float mx = 0;
    float my = 0;


    public static Vector3 forward = Vector3.zero;
    public static Vector3 right = Vector3.zero;
    public static Vector3 up = Vector3.zero;
    

    // Start is called before the first frame update
    void Start()
    {
        playerBody = transform.GetComponentInParent<CharacterController>().transform;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(this.transform.forward);

        forward = transform.forward;
        right = transform.right;
        up = transform.up;
        

        mx = Input.GetAxis("Mouse X");
        my = Input.GetAxis("Mouse Y");
        if (inited && Mathf.Abs(my) < 5)//忽略第一次锁定的大范围偏转
        {
            //x += mx * sensity * Time.deltaTime;
            //y -= my * sensity * Time.deltaTime;
            //y = Mathf.Clamp(y, yMin, yMax); // 限制Y轴旋转范围
            mx *= sensity * Time.deltaTime;
            my *= sensity * Time.deltaTime;

        }
        else inited = true;

        //Quaternion rotation = Quaternion.Euler(y, x, 0);

        //Vector3 rotation = new Vector3(x, y, 0);
        yRocation -= my;
        yRocation = Mathf.Clamp(yRocation, yMin, yMax);
        transform.localRotation = Quaternion.Euler(yRocation, 0, 0);

        playerBody.Rotate(Vector3.up * mx);
    }
}
