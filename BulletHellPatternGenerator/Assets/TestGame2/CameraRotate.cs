using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotate : MonoBehaviour
{
    public float RotSpeed = 10;
    public Transform LookAtPoint;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Quaternion.Euler(40f * Time.deltaTime, 0, 0) * transform.position;

        if (LookAtPoint != null) transform.LookAt(LookAtPoint,Vector3.up);
    }
}
