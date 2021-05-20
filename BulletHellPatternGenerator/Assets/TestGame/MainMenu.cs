using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public FadeIn Fade;

    private int scnToChange = -1;

    public void ChangeScene(int sceneIndex)
    {
        scnToChange = sceneIndex;
        Fade.Reverse = true;
    }

    public void Update()
    {
        if (Fade != null && scnToChange >= 0 && Fade.TransitionComplete) SceneManager.LoadScene(scnToChange);
    }
}
