using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BulletHellGenerator;

[System.Serializable]
public struct PatternData
{
    public BulletBase Bullet;
    public TimingBase Timing;
    public AngleBase   Angle;
}

[CreateAssetMenu(fileName = "NewBulletHellPattern")]
public class BulletHellPattern : ScriptableObject 
{
    [SerializeReference]
    public List<PatternData> Patterns;
}
