using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Fades in at creation, meant to 
// run at start of the scene for smooth transition

public class FadeIn : MonoBehaviour
{
    //Fade info
    public Image FadeInImage;
    private Color fadeOutColor = new Color(0, 0, 0, 1);

    public bool TransitionComplete
    {
        get
        {
            if (!reverse && fadeOutColor.a == 0) return true;
            if (reverse  && fadeOutColor.a == 1) return true;
            return false;
        }
    }

    public bool Reverse
    {
        set
        {
            if(value)
            {
                fadeOutColor = new Color(0, 0, 0, 0);
                reverse = true;
            }
            else
            {
                fadeOutColor = new Color(0, 0, 0, 1);
                reverse = false;
            }
        }
    }


    private bool reverse = false;

    // Start is called before the first frame update
    void Start()
    {
        FadeInImage.color = fadeOutColor;
    }

    // Update is called once per frame
    void Update()
    {
        if(reverse) fadeOutColor.a = Mathf.MoveTowards(fadeOutColor.a, 1, Time.deltaTime);
        else fadeOutColor.a = Mathf.MoveTowards(fadeOutColor.a, 0, Time.deltaTime);
        FadeInImage.color = fadeOutColor;
    }

    // Restarts the fade in
    public void Restart()
    {
        fadeOutColor.a = 1;
        FadeInImage.color = fadeOutColor;
    }
}