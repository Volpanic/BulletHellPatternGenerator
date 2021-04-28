using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public float MoveSpeed = 10f;
    public float ShiftMoveSpeed = 2.5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = new Vector3(Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical"),0).normalized;

        if(Input.GetKey(KeyCode.LeftShift))
        {
            transform.position += direction * ShiftMoveSpeed * Time.deltaTime;
        }
        else
        {
            transform.position += direction * MoveSpeed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Hurt");
    }
}
