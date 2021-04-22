using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BulletHellPatternGenerator : MonoBehaviour
{
    public GameObject Bullet;
    [Range(0.1f, 24f)]
    public float TimeBetween = 0.1f;

    [Range(4,128)]
    public int BulletAmount = 4;

    private float Timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Timer += Time.deltaTime;

        while(TimeBetween != 0 && Timer >= Mathf.Abs(TimeBetween))
        {
            Timer -= TimeBetween;

            DoBullet();

            if(Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();

                #if UNITYEDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
                #endif
            }
        }
    }

    private void DoBullet()
    {
        float segR = (Mathf.PI * 2f) / (float)BulletAmount;


        BH_Bullet pulse;

        for(int i = 0; i < BulletAmount; i++)
        {
            Vector3 p = new Vector3(Mathf.Cos(segR * i), Mathf.Sin(segR * i),0);
            p = transform.TransformDirection(p);
            //p = Vector3.Cross(p, transform.right + transform.forward + transform.up);


            pulse = Instantiate(Bullet, transform.position + p,Quaternion.identity).GetComponent<BH_Bullet>();
            pulse.Direction = new Vector3(p.x, p.y, p.z);
            pulse.MoveSpeed = 16;
        }
    }
}
