using UnityEngine;

/// <summary>
/// Trackea la pose del jugador VR en tiempo real usando XR Origin.
/// Obtiene posición y rotación de cabeza y manos en espacio local.
/// </summary>
public class VRPlayerTracker : MonoBehaviour
{
    [Header("VR Player Setup")]
    [SerializeField] private Transform playerRoot;
    [SerializeField] private Transform headTransform;
    [SerializeField] private Transform leftHandTransform;
    [SerializeField] private Transform rightHandTransform;

    public Transform PlayerRoot => playerRoot;
    public Transform HeadTransform => headTransform;
    public Transform LeftHandTransform => leftHandTransform;
    public Transform RightHandTransform => rightHandTransform;

    private void OnEnable()
    {
        ValidateTransforms();
    }

    /// <summary>
    /// Valida que todos los transforms estén asignados.
    /// </summary>
    private void ValidateTransforms()
    {
        if (playerRoot == null)
            Debug.LogError("VRPlayerTracker: playerRoot no asignado (debería ser XR Origin).");

        if (headTransform == null)
            Debug.LogError("VRPlayerTracker: headTransform no asignado (debería ser Main Camera).");

        if (leftHandTransform == null)
            Debug.LogError("VRPlayerTracker: leftHandTransform no asignado.");

        if (rightHandTransform == null)
            Debug.LogError("VRPlayerTracker: rightHandTransform no asignado.");
    }

    /// <summary>
    /// Obtiene la pose actual del jugador VR en espacio local.
    /// </summary>
    public MotionFrame GetCurrentFrame()
    {
        MotionFrame frame = new MotionFrame();
        frame.time = Time.time; // Tiempo global

        if (playerRoot == null)
        {
            return frame;
        }

        if (headTransform != null)
        {
            frame.headPositionLocal = playerRoot.InverseTransformPoint(headTransform.position);
            frame.headRotationLocal = Quaternion.Inverse(playerRoot.rotation) * headTransform.rotation;
        }

        if (leftHandTransform != null)
        {
            frame.leftHandPositionLocal = playerRoot.InverseTransformPoint(leftHandTransform.position);
            frame.leftHandRotationLocal = Quaternion.Inverse(playerRoot.rotation) * leftHandTransform.rotation;
        }

        if (rightHandTransform != null)
        {
            frame.rightHandPositionLocal = playerRoot.InverseTransformPoint(rightHandTransform.position);
            frame.rightHandRotationLocal = Quaternion.Inverse(playerRoot.rotation) * rightHandTransform.rotation;
        }

        return frame;
    }

    /// <summary>
    /// Retorna la posición en espacio mundo de una parte específica.
    /// </summary>
    public Vector3 GetWorldPosition(MotionFrame frameLocal, string limb)
    {
        if (playerRoot == null)
            return Vector3.zero;

        return limb switch
        {
            "head" => playerRoot.TransformPoint(frameLocal.headPositionLocal),
            "leftHand" => playerRoot.TransformPoint(frameLocal.leftHandPositionLocal),
            "rightHand" => playerRoot.TransformPoint(frameLocal.rightHandPositionLocal),
            _ => Vector3.zero
        };
    }
}

