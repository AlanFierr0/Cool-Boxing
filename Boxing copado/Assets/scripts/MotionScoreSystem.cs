using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sistema completo de motion matching scoring para juego VR.
/// Orquesta: sampleo de robot, tracking de jugador, comparación y score final.
/// </summary>
public class MotionScoreSystem : MonoBehaviour
{
    [Header("Componentes")]
    [SerializeField] private RobotMotionSampler robotSampler;
    [SerializeField] private VRPlayerTracker playerTracker;
    [SerializeField] private MotionComparer motionComparer;

    [Header("Scoring")]
    [SerializeField] private float maxAllowedError = 0.5f; // Si error > esto, score = 0
    [SerializeField] private bool autoStartSampling = false;

    [Header("Debug")]
    [SerializeField] private bool debugLogs = true;

    // Estados
    private bool isComparing = false;
    private float comparisonStartTime = 0f;
    private List<float> errorSamples = new List<float>();
    private float finalScore = 0f;
    private float averageError = 0f;

    // Eventos
    public event Action SamplingStarted;
    public event Action SamplingEnded;
    public event Action<float, float> ComparisonStarted; // playerElapsedTime, normalizedProgress
    public event Action<float, float> ComparisonTick;     // currentScore, averageError
    public event Action<float, float> ComparisonEnded;    // finalScore, averageError

    public bool IsComparing => isComparing;
    public float FinalScore => finalScore;
    public float AverageError => averageError;
    public List<float> ErrorSamples => errorSamples;

    private void OnEnable()
    {
        if (autoStartSampling)
        {
            StartRobotSampling();
        }
    }

    /// <summary>
    /// Inicia el sampleo de la animación del robot.
    /// </summary>
    public void StartRobotSampling()
    {
        if (robotSampler == null)
        {
            Debug.LogError("MotionScoreSystem: robotSampler no asignado.");
            return;
        }

        robotSampler.StartRecording();
        SamplingStarted?.Invoke();

        if (debugLogs)
            Debug.Log("MotionScoreSystem: Sampleo de robot iniciado.");
    }

    /// <summary>
    /// Detiene el sampleo de la animación del robot.
    /// </summary>
    public void StopRobotSampling()
    {
        if (robotSampler == null)
            return;

        robotSampler.StopRecording();
        SamplingEnded?.Invoke();

        if (debugLogs)
            Debug.Log($"MotionScoreSystem: Sampleo de robot detenido. Frames: {robotSampler.RecordedFrames.Count}");
    }

    /// <summary>
    /// Inicia la comparación de movimiento del jugador con el robot.
    /// El jugador tiene que imitar el movimiento grabado.
    /// </summary>
    public void StartComparison()
    {
        if (robotSampler == null || robotSampler.RecordedFrames.Count == 0)
        {
            Debug.LogError("MotionScoreSystem: No hay movimiento de robot grabado.");
            return;
        }

        isComparing = true;
        comparisonStartTime = Time.time;
        errorSamples.Clear();
        finalScore = 0f;
        averageError = 0f;

        ComparisonStarted?.Invoke(0f, 0f);

        if (debugLogs)
            Debug.Log("MotionScoreSystem: Comparación iniciada. Imita el movimiento del robot.");
    }

    /// <summary>
    /// Detiene la comparación y calcula el score final.
    /// </summary>
    public void StopComparison()
    {
        if (!isComparing)
        {
            return;
        }

        isComparing = false;
        CalculateFinalScore();

        if (debugLogs)
        {
            Debug.Log($"MotionScoreSystem: Comparación finalizada.");
            Debug.Log($"  Score Final: {finalScore:F1}/100");
            Debug.Log($"  Error Promedio: {averageError:F4}m");
            Debug.Log($"  Muestras: {errorSamples.Count}");
        }

        ComparisonEnded?.Invoke(finalScore, averageError);
    }

    private void Update()
    {
        if (!isComparing || motionComparer == null)
        {
            return;
        }

        float playerElapsedTime = Time.time - comparisonStartTime;

        // Calcular error actual
        float currentError = motionComparer.CalculateError(playerElapsedTime, out float normalizedProgress);

        // Almacenar muestra
        errorSamples.Add(currentError);

        // Calcular score actual (provisional)
        float currentScore = CalculateScore(currentError);

        // Disparo de evento de tick
        ComparisonTick?.Invoke(currentScore, currentError);

        // Debug visual
        if (motionComparer != null)
        {
            motionComparer.DebugDrawFrames(playerElapsedTime, Color.white, Color.cyan);
        }
    }

    /// <summary>
    /// Calcula el score basado en el error promedio.
    /// </summary>
    private float CalculateScore(float error)
    {
        // Script lineal: 1 - (error / maxAllowedError)
        // error = 0 → score = 100
        // error = maxAllowedError → score = 0
        return Mathf.Clamp01(1f - (error / maxAllowedError)) * 100f;
    }

    /// <summary>
    /// Calcula el score final basado en todas las muestras capturadas.
    /// </summary>
    private void CalculateFinalScore()
    {
        if (errorSamples.Count == 0)
        {
            finalScore = 0f;
            averageError = 0f;
            return;
        }

        // Error promedio de todas las muestras
        averageError = 0f;
        foreach (float error in errorSamples)
        {
            averageError += error;
        }
        averageError /= errorSamples.Count;

        // Score final
        finalScore = CalculateScore(averageError);
    }

    /// <summary>
    /// Retorna información del progreso actual durante la comparación.
    /// </summary>
    public void GetComparisonStatus(out float elapsedTime, out float normalizedProgress, out float currentScore, out float currentError)
    {
        elapsedTime = 0f;
        normalizedProgress = 0f;
        currentScore = 0f;
        currentError = 0f;

        if (!isComparing)
        {
            return;
        }

        elapsedTime = Time.time - comparisonStartTime;
        float robotDuration = robotSampler.DurationSeconds;
        normalizedProgress = robotDuration > 0f ? Mathf.Clamp01(elapsedTime / robotDuration) : 0f;

        currentError = motionComparer.CalculateError(elapsedTime, out _);
        currentScore = CalculateScore(currentError);
    }

    /// <summary>
    /// Resetea completamente el sistema.
    /// </summary>
    public void ResetSystem()
    {
        StopComparison();
        robotSampler?.GetComponent<RobotMotionSampler>()?.StopRecording();
        errorSamples.Clear();
        finalScore = 0f;
        averageError = 0f;
    }
}

