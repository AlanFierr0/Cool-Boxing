using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tracking
{
    [Serializable]
    public class MotionRecording
    {
        public MotionFrame[] Frames;

        public static MotionRecording FromList(List<MotionFrame> list)
        {
            return new MotionRecording { Frames = list.ToArray() };
        }

        public string ToJson(bool pretty = false)
        {
            return JsonUtility.ToJson(this, pretty);
        }

        public static MotionRecording FromJson(string json)
        {
            if (string.IsNullOrEmpty(json)) return null;
            try
            {
                return JsonUtility.FromJson<MotionRecording>(json);
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to parse MotionRecording JSON: " + e);
                return null;
            }
        }
    }
}

