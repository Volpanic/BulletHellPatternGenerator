using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BulletHellGenerator;

[System.Serializable]
[CreateAssetMenu(fileName = "NewBulletHellPattern")]
public class BulletHellPattern : ScriptableObject 
{
    [SerializeReference]
    public BulletBase Bullet;

    [SerializeReference]
    public TimingBase Timing;

    [SerializeReference]
    public PatternBase Pattern;
}
