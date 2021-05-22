using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BulletHellGenerator;
using static UnityEngine.ParticleSystem;

[System.Serializable]
[CreateAssetMenu(fileName = "NewBulletHellPattern")]
public class BulletHellPattern : ScriptableObject 
{
    [System.Serializable]
    public class PatternLayer
    {
        [SerializeReference]
        public BulletBase Bullet;

        [SerializeReference]
        public TimingBase Timing;

        [SerializeReference]
        public PatternBase Pattern;

        public string LayerName;
    }

    public List<PatternLayer> PatternLayers = new List<PatternLayer>();

    public float PatternDuration = 5;

    public static object PatternEditor { get; private set; }

    private void OnValidate()
    {
        PatternDuration = Mathf.Max(0.01f, PatternDuration);

    }

}
