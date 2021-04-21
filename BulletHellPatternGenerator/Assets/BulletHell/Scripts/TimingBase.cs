using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BulletHellGenerator
{
    [System.Serializable]
    public abstract class TimingBase
    {
        public virtual bool CheckTime(float deltaTime)
        {
            return true;
        }
    }
}