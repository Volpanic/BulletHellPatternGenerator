using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BulletHellGenerator
{
    [System.Serializable]
    public abstract class BulletBase
    {
        //Edits the prefab instance to contain the bullet script
        private void SetupPrefab()
        {

        }

        //Returns a bullet
        public virtual GameObject GetBullet()
        {
            return null;
        }
    }

    [System.Serializable]
    public class BulletTest : BulletBase
    {

    }
}