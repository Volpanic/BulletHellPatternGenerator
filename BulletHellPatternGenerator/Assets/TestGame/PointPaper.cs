using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointPaper : MonoBehaviour
{
    public Transform Target;

    private float timer = 0;
    private float maxtime = 0;
    Vector3 initalPos = Vector3.zero;

    private void Start()
    {
        initalPos = transform.position;
        maxtime = (Target.position - transform.position).magnitude * 0.025f;
    }

    void FixedUpdate()
    {
        timer += Time.deltaTime;
        transform.position = Vector3.Lerp(initalPos,Target.position, timer / maxtime);
        if (timer >= maxtime)
        {
            Target.BroadcastMessage("AddPoints", 10, SendMessageOptions.DontRequireReceiver);
            Destroy(gameObject);
        }
    }
}
