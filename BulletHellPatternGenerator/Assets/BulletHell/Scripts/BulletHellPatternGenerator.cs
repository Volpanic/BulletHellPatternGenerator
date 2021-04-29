using BulletHellGenerator;
using BulletHellGenerator.Heatmap;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class BulletHellPatternGenerator : MonoBehaviour
{
    public List<BulletHellPattern> Patterns;
    public MinMaxCurve Curve;
    public BH_HeatmapGenerator HeatmapGen;

    public AngleRange range;

    private List<BH_Bullet> SpawnedBullets = new List<BH_Bullet>();

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

    #region // Bullet Creation

    public BH_Bullet CreateBulletAtDirection(Vector3 position, float Speed, float Angle, GameObject BulletPrefab)
    {
        if (BulletPrefab == null) return null;
        Vector3 dir = new Vector3(Mathf.Cos(Angle), Mathf.Sin(Angle), 0);

        BH_Bullet pulse = Instantiate(BulletPrefab, position, Quaternion.identity).GetComponent<BH_Bullet>();
        pulse.Direction = dir;
        pulse.MoveSpeed = Speed;
        pulse.RelativeDirection = transform.rotation;
        pulse.Creator = this;

        SpawnedBullets.Add(pulse);
        return pulse;
    }
    #endregion
}
