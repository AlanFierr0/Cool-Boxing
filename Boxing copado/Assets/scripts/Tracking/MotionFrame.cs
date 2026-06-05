using System;
using UnityEngine;

namespace Tracking
{
    [Serializable]
    public class MotionFrame
    {
        public float time; // seconds since start of recording
        public Vector3 leftPos;
        public Vector3 rightPos;
        public Quaternion leftRot;
        public Quaternion rightRot;
    }
}

