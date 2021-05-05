using BulletHellGenerator;
using BulletHellGenerator.Heatmap;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class BH_BulletHellPatternGenerator : MonoBehaviour
{
    public List<BulletHellPattern> Patterns;
    public MinMaxCurve Curve;
    public BH_HeatmapGenerator HeatmapGen;

    public float TargetAngle { get { return targetAngle; } }

    public Transform Target;

    private float targetAngle = 0;

    private List<BH_Bullet> SpawnedBullets = new List<BH_Bullet>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(Target != null)
        {
            Vector2 ang = Target.position - transform.position;
            targetAngle = Mathf.Atan2(ang.y,ang.x);
        }

        if(Patterns != null)
        {
            for(int i = 0; i < Patterns.Count; i++)
            {
                if (Patterns[i].PatternLayers == null || Patterns[i].PatternLayers.Count == 0)
                    continue;

                for (int j = 0; j < Patterns[i].PatternLayers.Count; j++)
                {
                    Patterns[i].PatternLayers[j].Pattern.UpdatePattern(this, Patterns[i],Patterns[i].PatternLayers[j]);
                }
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
        pulse.Target = Target;

        SpawnedBullets.Add(pulse);
        return pulse;
    }

    public BH_Bullet CreateBulletAtDirectionOct(Vector3 position, float Speed, float Angle, GameObject BulletPrefab)
    {
        if (BulletPrefab == null) return null;
        Vector3 dir = new Vector3(Mathf.Cos(Angle), Mathf.Sin(Angle), 0);

        BH_Bullet pulse = Instantiate(BulletPrefab, position, Quaternion.identity).GetComponent<BH_Bullet>();
        pulse.Direction = dir;

        if (Mathf.Abs(Mathf.Round(dir.x)) > Mathf.Abs(Mathf.Round(dir.y))) dir.x = Mathf.Round(dir.x);
        if (Mathf.Abs(Mathf.Round(dir.y)) > Mathf.Abs(Mathf.Round(dir.x))) dir.y = Mathf.Round(dir.y);

        pulse.MoveSpeed = Speed * (dir.magnitude);
        pulse.RelativeDirection = transform.rotation;
        pulse.Target = Target;


        SpawnedBullets.Add(pulse);
        return pulse;
    }

    public BH_Bullet CreateBulletAtDirectionSquare(Vector3 position, float Speed, float Angle, GameObject BulletPrefab)
    {
        if (BulletPrefab == null) return null;
        Vector3 dir = new Vector3(Mathf.Cos(Angle), Mathf.Sin(Angle), 0);

        float M = Mathf.Max(Mathf.Abs(dir.x),Mathf.Abs(dir.y));

        BH_Bullet pulse = Instantiate(BulletPrefab, position, Quaternion.identity).GetComponent<BH_Bullet>();
        pulse.Direction = dir;
        pulse.MoveSpeed = Speed / M;
        pulse.RelativeDirection = transform.rotation;
        pulse.Target = Target;

        SpawnedBullets.Add(pulse);
        return pulse;
    }
    #endregion
}
