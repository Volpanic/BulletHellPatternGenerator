using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class BulletHellPatternGenerator : MonoBehaviour
{
    public List<BulletHellPattern> Patterns;

    public GameObject Bullet;
    [Range(0.1f, 24f)]
    public float TimeBetween = 0.1f;

    [Range(4,128)]
    public int BulletAmount = 4;

    public MinMaxCurve Curve;

    private float Timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Patterns != null)
        {
            for(int i = 0; i < Patterns.Count; i++)
            {
                Patterns[i].Pattern.UpdatePattern(this,Patterns[i]);
            }
        }
    }
}
