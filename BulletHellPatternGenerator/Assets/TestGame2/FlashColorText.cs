using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FlashColorText : MonoBehaviour
{
    public TextMeshProUGUI Text;
    public Color MinColor;
    public Color MaxColor;

    public float FlashRate = 1;
    private float timer = 0;

    // Update is called once per frame
    void Update()
    {
        Text.color = Color.Lerp(MinColor,MaxColor,(timer % FlashRate) / FlashRate);

        timer += Time.deltaTime;
    }
}
