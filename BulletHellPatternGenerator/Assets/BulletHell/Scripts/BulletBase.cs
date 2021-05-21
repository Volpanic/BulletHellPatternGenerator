using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace BulletHellGenerator
{
    [System.Serializable]
    public abstract class BulletBase
    {
        //Returns a bullet
        public virtual GameObject GetBullet()
        {
            return null;
        }
    }

    #region Alternating Bullets
    [System.Serializable]
    public class AlternatingBullets : BulletBase
    {
        public List<GameObject> Bullets;

        private int ToPick = 0;

        public AlternatingBullets()
        {
            //Creates a new lis if bullets == null
            Bullets = Bullets ?? new List<GameObject>();
        }

        public override GameObject GetBullet()
        {
            if (Bullets.Count == 0) return null;
            return Bullets[ToPick++ % Bullets.Count];
        }
    }
    #endregion

    #region Alternating Bullets
    [System.Serializable]
    public class RandomBullets : BulletBase
    {
        public List<GameObject> Bullets;

        private int ToPick = 0;

        public RandomBullets()
        {
            //Creates a new lis if bullets == null
            Bullets = Bullets ?? new List<GameObject>();
        }

        public override GameObject GetBullet()
        {
            //Make randomness consistant
            Random.InitState(ToPick++);
            return Bullets[Random.Range(0, Bullets.Count)];
        }

    }
    #endregion
}