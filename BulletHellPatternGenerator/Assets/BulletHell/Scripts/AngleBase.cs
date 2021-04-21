using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BulletHellGenerator
{
    [System.Serializable]
    public class AngleBase : MonoBehaviour
    {
        public virtual Vector3 GetAngle(float deltaTime,bool is3D)
        {
            return Vector3.zero;
        }
    }
}