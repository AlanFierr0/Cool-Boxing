using UnityEngine;

/// <summary>
/// Un snapshot de una muestra grabada para el ghost punch.
/// Se guarda en espacio local del playerRoot / XR Origin.
/// </summary>
[System.Serializable]
public struct VRGhostFrame
{
    public float time;

    public Vector3 headLocalPosition;
    public Quaternion headLocalRotation;

    public Vector3 leftHandLocalPosition;
    public Quaternion leftHandLocalRotation;

    public Vector3 rightHandLocalPosition;
    public Quaternion rightHandLocalRotation;

    public VRGhostFrame(
        float time,
        Vector3 headLocalPosition,
        Quaternion headLocalRotation,
        Vector3 leftHandLocalPosition,
        Quaternion leftHandLocalRotation,
        Vector3 rightHandLocalPosition,
        Quaternion rightHandLocalRotation)
    {
        this.time = time;
        this.headLocalPosition = headLocalPosition;
        this.headLocalRotation = headLocalRotation;
        this.leftHandLocalPosition = leftHandLocalPosition;
        this.leftHandLocalRotation = leftHandLocalRotation;
        this.rightHandLocalPosition = rightHandLocalPosition;
        this.rightHandLocalRotation = rightHandLocalRotation;
    }
}

