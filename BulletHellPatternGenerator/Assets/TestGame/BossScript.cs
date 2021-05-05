using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossScript : MonoBehaviour
{
    private Vector3 targetPosition;
    private Vector3 startPosition;
    private float transitionDuration = 1;
    private float transitionTimer = 0;
    private bool transitioning = false;

    public int MaxHp = 100;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(transitioning)
        {
            transitionTimer += Time.deltaTime;

            transform.position = Vector3.Lerp(startPosition, targetPosition, transitionTimer / transitionDuration);

            if(transitionTimer >= transitionDuration)
            {
                transitioning = false;
                transitionTimer = 0;
                transform.position = targetPosition;
            }
        }
    }

    public void MoveToPoint(Vector3 targetPos, float durationInSeconds)
    {
        targetPosition = targetPos;
        transitionDuration = durationInSeconds;
        transitionTimer = 0;
        transitioning = true;
        startPosition = transform.position;
    }
}
