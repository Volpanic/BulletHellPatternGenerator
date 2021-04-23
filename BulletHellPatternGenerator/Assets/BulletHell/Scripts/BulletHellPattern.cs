using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BulletHellGenerator;

[CreateAssetMenu(fileName = "NewBulletHellPattern")]
public class BulletHellPattern : ScriptableObject 
{
    public BulletBase Bullet;
    public TimingBase Timing;
    public PatternBase Pattern;

}
