using System;
using UnityEngine;

namespace com.technical.test
{
    [Serializable]
    public struct RotationSettings
    {
        public Transform ObjectToRotate;
        public Vector3 AngleRotation;
        public float TimeToRotateInSeconds;
    }
}
