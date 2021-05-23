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
        public BH_BulletList Bullets = new BH_BulletList();

        private int ToPick = 0;

        public AlternatingBullets()
        {
            //Creates a new list if bullets == null
            Bullets = Bullets ?? new BH_BulletList();
        }

        public override GameObject GetBullet()
        {
            if (Bullets.Bullets.Length == 0) return null;
            return Bullets.Bullets[ToPick++ % Bullets.Bullets.Length];
        }
    }
    #endregion

    #region Alternating Bullets
    [System.Serializable]
    public class RandomBullets : BulletBase
    {
        public BH_BulletList Bullets = new BH_BulletList();

        private int ToPick = 0;

        public RandomBullets()
        {
            //Creates a new lis if bullets == null
            Bullets = Bullets ?? new BH_BulletList();
        }

        public override GameObject GetBullet()
        {
            //Make randomness consistant
            Random.InitState(ToPick++);
            return Bullets.Bullets[Random.Range(0, Bullets.Bullets.Length)];
        }

    }
    #endregion
}