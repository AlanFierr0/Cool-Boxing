using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Samplea la animación de un robot humanoid durante su ejecución.
/// Extrae automáticamente bones del Animator y captura poses cada X segundos.
/// </summary>
public class RobotMotionSampler : MonoBehaviour
{
    [Header("Robot Setup")]
    [SerializeField] private Transform robotRoot;
    [SerializeField] private Animator robotAnimator;

    [Header("Sampleo")]
    [SerializeField] private float sampleInterval = 0.02f; // 50 Hz

    // Estados internos
    private Transform robotHeadBone;
    private Transform robotLeftHandBone;
    private Transform robotRightHandBone;

    private List<MotionFrame> recordedFrames = new List<MotionFrame>();
    private float timeSinceLastSample = 0f;
    private bool isRecording = false;

    public List<MotionFrame> RecordedFrames => recordedFrames;
    public bool IsRecording => isRecording;
    public float DurationSeconds => recordedFrames.Count > 0 
        ? recordedFrames[recordedFrames.Count - 1].time 
        : 0f;

    private void OnEnable()
    {
        InitializeBones();
    }

    /// <summary>
    /// Extrae automáticamente los bones humanoid del Animator.
    /// </summary>
    private void InitializeBones()
    {
        if (robotAnimator == null)
        {
            Debug.LogError("RobotMotionSampler: robotAnimator no asignado.");
            return;
        }

        robotHeadBone = robotAnimator.GetBoneTransform(HumanBodyBones.Head);
        robotLeftHandBone = robotAnimator.GetBoneTransform(HumanBodyBones.LeftHand);
        robotRightHandBone = robotAnimator.GetBoneTransform(HumanBodyBones.RightHand);

        if (robotHeadBone == null || robotLeftHandBone == null || robotRightHandBone == null)
        {
            Debug.LogError("RobotMotionSampler: no se encontraron algunos bones humanoid. Verifica que el Animator sea humanoid.");
        }
    }

    /// <summary>
    /// Inicia la grabación de movimiento.
    /// </summary>
    public void StartRecording()
    {
        recordedFrames.Clear();
        timeSinceLastSample = 0f;
        isRecording = true;
        Debug.Log("RobotMotionSampler: grabación iniciada.");
    }

    /// <summary>
    /// Detiene la grabación.
    /// </summary>
    public void StopRecording()
    {
        isRecording = false;
        Debug.Log($"RobotMotionSampler: grabación detenida. Frames capturados: {recordedFrames.Count}, duración: {DurationSeconds:0.000}s");
    }

    private void Update()
    {
        if (!isRecording || robotRoot == null)
        {
            return;
        }

        timeSinceLastSample += Time.deltaTime;

        // Samplear cada sampleInterval segundos
        if (timeSinceLastSample >= sampleInterval)
        {
            CaptureMotionFrame();
            timeSinceLastSample = 0f;
        }
    }

    /// <summary>
    /// Captura un frame de movimiento del robot en espacio local.
    /// </summary>
    private void CaptureMotionFrame()
    {
        MotionFrame frame = new MotionFrame();

        float totalRecordedTime = recordedFrames.Count > 0 
            ? recordedFrames[recordedFrames.Count - 1].time 
            : 0f;
        frame.time = totalRecordedTime + sampleInterval;

        // Validar que los bones existan
        if (robotHeadBone != null)
        {
            frame.headPositionLocal = robotRoot.InverseTransformPoint(robotHeadBone.position);
            frame.headRotationLocal = Quaternion.Inverse(robotRoot.rotation) * robotHeadBone.rotation;
        }

        if (robotLeftHandBone != null)
        {
            frame.leftHandPositionLocal = robotRoot.InverseTransformPoint(robotLeftHandBone.position);
            frame.leftHandRotationLocal = Quaternion.Inverse(robotRoot.rotation) * robotLeftHandBone.rotation;
        }

        if (robotRightHandBone != null)
        {
            frame.rightHandPositionLocal = robotRoot.InverseTransformPoint(robotRightHandBone.position);
            frame.rightHandRotationLocal = Quaternion.Inverse(robotRoot.rotation) * robotRightHandBone.rotation;
        }

        recordedFrames.Add(frame);
    }

    /// <summary>
    /// Retorna la pose del robot en un momento específico (en espacio local).
    /// </summary>
    public MotionFrame GetFrameAtTime(float time)
    {
        if (recordedFrames.Count == 0)
        {
            return new MotionFrame();
        }

        // Buscar el frame más cercano
        int closestIndex = 0;
        float minDelta = Mathf.Abs(recordedFrames[0].time - time);

        for (int i = 1; i < recordedFrames.Count; i++)
        {
            float delta = Mathf.Abs(recordedFrames[i].time - time);
            if (delta < minDelta)
            {
                minDelta = delta;
                closestIndex = i;
            }
        }

        return recordedFrames[closestIndex];
    }

    /// <summary>
    /// Retorna dos frames adyacentes para interpolación.
    /// </summary>
    public bool GetInterpolatedFrame(float time, out MotionFrame frameA, out MotionFrame frameB, out float t)
    {
        frameA = null;
        frameB = null;
        t = 0f;

        if (recordedFrames.Count < 2)
        {
            frameA = recordedFrames.Count > 0 ? recordedFrames[0] : new MotionFrame();
            frameB = frameA;
            return false;
        }

        // Clamear tiempo al rango válido
        time = Mathf.Clamp(time, 0f, DurationSeconds);

        // Encontrar los dos frames
        for (int i = 0; i < recordedFrames.Count - 1; i++)
        {
            if (recordedFrames[i].time <= time && time <= recordedFrames[i + 1].time)
            {
                frameA = recordedFrames[i];
                frameB = recordedFrames[i + 1];
                float range = frameB.time - frameA.time;
                t = range > 0f ? (time - frameA.time) / range : 0f;
                return true;
            }
        }

        // Si no encontró, retorna el último frame
        frameA = recordedFrames[recordedFrames.Count - 1];
        frameB = frameA;
        return false;
    }
}

