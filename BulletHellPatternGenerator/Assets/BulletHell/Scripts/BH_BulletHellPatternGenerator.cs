using BulletHellGenerator;
using BulletHellGenerator.Heatmap;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    private BH_BulletObjectPooler pooler;

    //Prev REv
    //With Pooling 70 - 90 FPS
    //Wout Pooling 

    // Start is called before the first frame update
    void Start()
    {
        string poolName = SceneManager.GetActiveScene().name + "bulletGen_pooler";
        //Search if object exsists, so we don't have multiple poolers, GameObject.Find is expensive but only runs in start
        GameObject oPool = GameObject.Find(poolName);
        if(oPool != null)
        {
            pooler = oPool.GetComponent<BH_BulletObjectPooler>();
        }
        else
        {
            //Create Object
            pooler = new GameObject(poolName).AddComponent<BH_BulletObjectPooler>();
        }
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
                if (Patterns[i] == null || Patterns[i].PatternLayers == null || Patterns[i].PatternLayers.Count == 0)
                    continue;

                for (int j = 0; j < Patterns[i].PatternLayers.Count; j++)
                {
                    if (Patterns[i].PatternLayers[j].Pattern == null) continue;
                    Patterns[i].PatternLayers[j].Pattern.UpdatePattern(this, Patterns[i],Patterns[i].PatternLayers[j]);
                }
            }
        }
    }

    public void ClearBullets()
    {
        if (pooler == null) return;

        for(int i = 0; i < pooler.transform.childCount; i++)
        {
            pooler.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void ClearBullets(Action<Transform> bulletClearEvent)
    {
        if (pooler == null) return;

        for (int i = 0; i < pooler.transform.childCount; i++)
        {
            var child = pooler.transform.GetChild(i);
            if(child.gameObject.activeSelf) bulletClearEvent(child);
            child.gameObject.SetActive(false);
        }
    }

    #region // Bullet Creation

    public BH_Bullet CreateBulletAtDirection(Vector3 position, float Speed, float Angle, GameObject BulletPrefab)
    {
        if (BulletPrefab == null) return null;
        Vector3 dir = new Vector3(Mathf.Cos(Angle), Mathf.Sin(Angle), 0);

        BH_Bullet pulse = pooler.PoolInstansiate(BulletPrefab, position, Quaternion.identity);
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

        BH_Bullet pulse = pooler.PoolInstansiate(BulletPrefab, position, Quaternion.identity);
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

        BH_Bullet pulse = pooler.PoolInstansiate(BulletPrefab, position, Quaternion.identity);
        pulse.Direction = dir;
        pulse.MoveSpeed = Speed / M;
        pulse.RelativeDirection = transform.rotation;
        pulse.Target = Target;

        SpawnedBullets.Add(pulse);
        return pulse;
    }

    public void CreateBulletAtDirectionStack(Vector3 position, float MinSpeed, float MaxSpeed, int StackCount,float Angle, GameObject BulletPrefab)
    {
        for(int i = 0; i < StackCount; i++)
        {
            float speed = Mathf.Lerp(MinSpeed, MaxSpeed, (float)i / (float)StackCount);
            CreateBulletAtDirection(position,speed,Angle,BulletPrefab);
        }
    }

    public void CreateBulletAtDirectionOctStack(Vector3 position, float MinSpeed, float MaxSpeed, int StackCount, float Angle, GameObject BulletPrefab)
    {
        for (int i = 0; i < StackCount; i++)
        {
            float speed = Mathf.Lerp(MinSpeed, MaxSpeed, (float)i / (float)StackCount);
            CreateBulletAtDirectionOct(position, speed, Angle, BulletPrefab);
        }
    }

    public void CreateBulletAtDirectionSquareStack(Vector3 position, float MinSpeed, float MaxSpeed, int StackCount, float Angle, GameObject BulletPrefab)
    {
        for (int i = 0; i < StackCount; i++)
        {
            float speed = Mathf.Lerp(MinSpeed, MaxSpeed, (float)i / (float)StackCount);
            CreateBulletAtDirectionSquare(position, speed, Angle, BulletPrefab);
        }
    }

    #endregion
}
