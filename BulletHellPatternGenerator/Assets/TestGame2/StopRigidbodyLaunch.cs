using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopRigidbodyLaunch : MonoBehaviour
{
    public Rigidbody Body;
    public float DisableDuration = 2f;

    private float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (Body == null) enabled = false;
        else Body.detectCollisions = false;
    }

    private void OnEnable()
    {
        timer = 0;
        Body.detectCollisions = false;
    }

    private void Update()
    {
        if (timer >= DisableDuration)
        {
            Body.detectCollisions = true;
        }
        timer += Time.deltaTime;
    }
}
