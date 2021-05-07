using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public BossScript Target;
    public Image BarImage;

    private float originalWidth = 0;
    private Vector2 barDelta;

    // Start is called before the first frame update
    void Start()
    {
        originalWidth = BarImage.rectTransform.sizeDelta.x;
        barDelta = BarImage.rectTransform.sizeDelta;

        if (Target == null)
        {
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        barDelta.x = originalWidth * ((float)Target.CurrentHp / Target.MaxHp);
        BarImage.rectTransform.sizeDelta = barDelta;
    }
}