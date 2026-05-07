using UnityEngine;

/// <summary>
/// Compara poses del jugador VR con poses del robot.
/// Usa "normalized progress" para permitir timing flexible.
/// El jugador puede ir más lento o más rápido, pero la trayectoria importa.
/// </summary>
public class MotionComparer : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private RobotMotionSampler robotSampler;
    [SerializeField] private VRPlayerTracker playerTracker;

    [Header("Ponderación")]
    [SerializeField] private float leftHandWeight = 0.4f;
    [SerializeField] private float rightHandWeight = 0.4f;
    [SerializeField] private float headWeight = 0.2f;

    [Header("Debug")]
    [SerializeField] private bool drawDebugGizmos = true;
    [SerializeField] private float gizmoSphereSize = 0.05f;

    /// <summary>
    /// Calcula el error promedio ponderado entre jugador y robot.
    /// Usa normalized progress para tolerancia de timing.
    /// </summary>
    public float CalculateError(float playerElapsedTime, out float normalizedProgress)
    {
        normalizedProgress = 0f;

        if (robotSampler == null || playerTracker == null || robotSampler.RecordedFrames.Count == 0)
        {
            return float.MaxValue;
        }

        // Calcular progreso normalizado (0 a 1)
        float robotDuration = robotSampler.DurationSeconds;
        normalizedProgress = robotDuration > 0f ? Mathf.Clamp01(playerElapsedTime / robotDuration) : 0f;

        // Obtener frame del robot en ese progreso normalizado
        float robotTimeAtProgress = robotDuration * normalizedProgress;
        MotionFrame robotFrame = robotSampler.GetFrameAtTime(robotTimeAtProgress);

        // Obtener frame actual del jugador
        MotionFrame playerFrame = playerTracker.GetCurrentFrame();

        // Calcular errores individuales
        float leftHandError = CalculateLimbError(robotFrame.leftHandPositionLocal, robotFrame.leftHandRotationLocal,
                                                  playerFrame.leftHandPositionLocal, playerFrame.leftHandRotationLocal);

        float rightHandError = CalculateLimbError(robotFrame.rightHandPositionLocal, robotFrame.rightHandRotationLocal,
                                                   playerFrame.rightHandPositionLocal, playerFrame.rightHandRotationLocal);

        float headError = CalculateLimbError(robotFrame.headPositionLocal, robotFrame.headRotationLocal,
                                             playerFrame.headPositionLocal, playerFrame.headRotationLocal);

        // Calcular error ponderado
        float totalWeight = leftHandWeight + rightHandWeight + headWeight;
        float weightedError = (leftHandError * leftHandWeight +
                               rightHandError * rightHandWeight +
                               headError * headWeight) / totalWeight;

        return weightedError;
    }

    /// <summary>
    /// Calcula el error (distancia) de una extremidad específica.
    /// Combina posición (80%) y rotación (20%) para penalizar desviaciones.
    /// </summary>
    private float CalculateLimbError(Vector3 robotPos, Quaternion robotRot,
                                     Vector3 playerPos, Quaternion playerRot)
    {
        // Error de posición (metros)
        float positionError = Vector3.Distance(robotPos, playerPos);

        // Error de rotación (en grados, normalizado a metros equivalentes)
        float rotationAngleDelta = Quaternion.Angle(robotRot, playerRot);
        float rotationErrorInMeters = (rotationAngleDelta / 180f) * 0.5f; // Mapea rotación a escala métrica

        // Combinar: posición es más importante (80%) que rotación (20%)
        float limbError = (positionError * 0.8f) + (rotationErrorInMeters * 0.2f);

        return limbError;
    }

    /// <summary>
    /// Crea frames de debug para visualización con gizmos.
    /// </summary>
    public void DebugDrawFrames(float playerElapsedTime, Color playerColor, Color robotColor)
    {
        if (!drawDebugGizmos)
        {
            return;
        }

        if (robotSampler == null || playerTracker == null || robotSampler.RecordedFrames.Count == 0)
        {
            return;
        }

        // Calcular progreso normalizado
        float robotDuration = robotSampler.DurationSeconds;
        float normalizedProgress = robotDuration > 0f ? Mathf.Clamp01(playerElapsedTime / robotDuration) : 0f;
        float robotTimeAtProgress = robotDuration * normalizedProgress;

        // Frames
        MotionFrame robotFrame = robotSampler.GetFrameAtTime(robotTimeAtProgress);
        MotionFrame playerFrame = playerTracker.GetCurrentFrame();

        // Convertir a mundo para debug (suponiendo que playerRoot y robotRoot son los mismos o similar)
        if (playerTracker.PlayerRoot != null)
        {
            Vector3 robotLeftHandWorld = playerTracker.PlayerRoot.TransformPoint(robotFrame.leftHandPositionLocal);
            Vector3 playerLeftHandWorld = playerTracker.PlayerRoot.TransformPoint(playerFrame.leftHandPositionLocal);

            Vector3 robotRightHandWorld = playerTracker.PlayerRoot.TransformPoint(robotFrame.rightHandPositionLocal);
            Vector3 playerRightHandWorld = playerTracker.PlayerRoot.TransformPoint(playerFrame.rightHandPositionLocal);

            Vector3 robotHeadWorld = playerTracker.PlayerRoot.TransformPoint(robotFrame.headPositionLocal);
            Vector3 playerHeadWorld = playerTracker.PlayerRoot.TransformPoint(playerFrame.headPositionLocal);

            // Dibujar esferas
            Debug.DrawLine(robotLeftHandWorld, playerLeftHandWorld, Color.yellow, Time.deltaTime);
            Debug.DrawLine(robotRightHandWorld, playerRightHandWorld, Color.yellow, Time.deltaTime);

            // Nota: DrawWireSphere no existe en Debug, solo Gizmos
            // Se usa en OnDrawGizmos
        }
    }

    private void OnDrawGizmos()
    {
        if (!drawDebugGizmos || Application.isPlaying == false)
        {
            return;
        }
    }
}

