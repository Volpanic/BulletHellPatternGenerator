using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BulletHellGenerator
{
    [System.Serializable]
    public class PatternBase : MonoBehaviour
    {
        public virtual Vector3 GetPattern(float deltaTime)
        {
            return Vector3.zero;
        }
    }

    [System.Serializable]
    public class PatternTest : PatternBase
    {

    }
}