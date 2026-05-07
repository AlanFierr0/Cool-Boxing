using System.Collections;
using UnityEngine;

/// <summary>
/// Ejemplo de uso: Control del sistema de motion matching.
/// Samplea un movimiento del robot, deja que el jugador imite, y muestra un score.
/// 
/// CÓMO USAR:
/// 1. Asigna este script a un GameObject
/// 2. Arrastra el MotionScoreSystem asignado en el Inspector
/// 3. Arrastra el Animator del robot
/// 4. En Update() de tu gameloop, llama a methods aquí
/// </summary>
public class MotionMatchingExample : MonoBehaviour
{
    [SerializeField] private MotionScoreSystem scoreSystem;
    [SerializeField] private Animator robotAnimator;
    [SerializeField] private float animationDuration = 2f;
    [SerializeField] private float imitationTimeWindow = 4f;

    private bool isRunning = false;

    // Escucha eventos
    private void OnEnable()
    {
        if (scoreSystem != null)
        {
            scoreSystem.SamplingStarted += OnSamplingStarted;
            scoreSystem.SamplingEnded += OnSamplingEnded;
            scoreSystem.ComparisonStarted += OnComparisonStarted;
            scoreSystem.ComparisonEnded += OnComparisonEnded;
            scoreSystem.ComparisonTick += OnComparisonTick;
        }
    }

    private void OnDisable()
    {
        if (scoreSystem != null)
        {
            scoreSystem.SamplingStarted -= OnSamplingStarted;
            scoreSystem.SamplingEnded -= OnSamplingEnded;
            scoreSystem.ComparisonStarted -= OnComparisonStarted;
            scoreSystem.ComparisonEnded -= OnComparisonEnded;
            scoreSystem.ComparisonTick -= OnComparisonTick;
        }
    }

    // ============================================
    // Flujo Principal: Tutorial de Golpe
    // ============================================

    public void StartPunchTutorial(string punchName = "PunchLeft")
    {
        if (isRunning)
        {
            Debug.LogWarning("Tutorial ya está en ejecución.");
            return;
        }

        StartCoroutine(PunchTutorialRoutine(punchName));
    }

    private IEnumerator PunchTutorialRoutine(string punchName)
    {
        isRunning = true;

        Debug.Log($"<b>TUTORIAL: {punchName}</b>");
        Debug.Log("1. Robot va a demostrar el golpe...");

        // ========================================
        // FASE 1: SAMPLEO DEL ROBOT
        // ========================================
        scoreSystem.StartRobotSampling();
        
        // Trigger animación del robot
        if (robotAnimator != null)
        {
            robotAnimator.SetTrigger(punchName);
        }

        // Esperar a que termine
        yield return new WaitForSeconds(animationDuration);

        // Detener sampleo
        scoreSystem.StopRobotSampling();

        Debug.Log("2. Movimiento grabado. ¡Ahora tu turno!");
        yield return new WaitForSeconds(1f);

        // ========================================
        // FASE 2: COMPARACIÓN DEL JUGADOR
        // ========================================
        scoreSystem.StartComparison();

        // Esperar a que el jugador imite (ventana de tiempo)
        yield return new WaitForSeconds(imitationTimeWindow);

        // Detener evaluación
        scoreSystem.StopComparison();

        // ========================================
        // FASE 3: RESULTADO
        // ========================================
        float score = scoreSystem.FinalScore;
        float error = scoreSystem.AverageError;

        PrintScoreResult(score, error);

        isRunning = false;
    }

    // ============================================
    // Métodos de Evaluación
    // ============================================

    public void EvaluatePlayerPunch(float durationSeconds = 3f)
    {
        if (isRunning)
        {
            return;
        }

        StartCoroutine(EvaluateRoutine(durationSeconds));
    }

    private IEnumerator EvaluateRoutine(float duration)
    {
        isRunning = true;

        scoreSystem.StartComparison();
        yield return new WaitForSeconds(duration);
        scoreSystem.StopComparison();

        isRunning = false;
    }

    // ============================================
    // Event Handlers
    // ============================================

    private void OnSamplingStarted()
    {
        Debug.Log("[Sampleo] Iniciado. La animación del robot está siendo capturada.");
    }

    private void OnSamplingEnded()
    {
        int frames = scoreSystem != null ? scoreSystem.ErrorSamples.Count : 0;
        Debug.Log($"[Sampleo] Finalizado. {frames} frames capturados.");
    }

    private void OnComparisonStarted(float playerElapsedTime, float normalizedProgress)
    {
        Debug.Log("[Comparación] Iniciada. El sistema está evaluando tu movimiento.");
        Debug.Log("  ¡Imita el movimiento del robot lo más fiel posible!");
    }

    private void OnComparisonTick(float currentScore, float currentError)
    {
        // Se llama en cada frame de comparación
        // Mostrar aquí si quieres HUD en tiempo real
    }

    private void OnComparisonEnded(float finalScore, float averageError)
    {
        PrintScoreResult(finalScore, averageError);
    }

    private void PrintScoreResult(float score, float error)
    {
        Debug.Log("═════════════════════════════════════");
        Debug.Log($"📊 RESULTADO FINAL");
        Debug.Log($"   Score:        {score:F1} / 100");
        Debug.Log($"   Error prom:   {error:F4} m");
        Debug.Log("═════════════════════════════════════");

        // Feedback cualitativo
        if (score >= 90f)
            Debug.Log("🌟 ¡Perfecto! Seguimiento exacto.");
        else if (score >= 75f)
            Debug.Log("⭐ ¡Excelente! Muy bien ejecutado.");
        else if (score >= 60f)
            Debug.Log("👍 ¡Bien! Necesita algo de ajuste.");
        else if (score >= 40f)
            Debug.Log("📌 Decente. Apunta mejor a los movimientos.");
        else
            Debug.Log("💪 Intenta de nuevo. Requiere más práctica.");
    }

    // ============================================
    // Métodos de Control Manual (Para el Inspector)
    // ============================================

    [ContextMenu("Start Sampling")]
    public void ContextStartSampling()
    {
        scoreSystem.StartRobotSampling();
    }

    [ContextMenu("Stop Sampling")]
    public void ContextStopSampling()
    {
        scoreSystem.StopRobotSampling();
    }

    [ContextMenu("Start Comparison")]
    public void ContextStartComparison()
    {
        scoreSystem.StartComparison();
    }

    [ContextMenu("Stop Comparison")]
    public void ContextStopComparison()
    {
        scoreSystem.StopComparison();
    }

    [ContextMenu("Simulate Quick Test")]
    public void ContextSimulateTest()
    {
        StartCoroutine(SimulateQuickTest());
    }

    private IEnumerator SimulateQuickTest()
    {
        Debug.Log("Simulando test rápido...");
        
        scoreSystem.StartRobotSampling();
        yield return new WaitForSeconds(1f);
        scoreSystem.StopRobotSampling();

        yield return new WaitForSeconds(0.5f);

        scoreSystem.StartComparison();
        yield return new WaitForSeconds(2f);
        scoreSystem.StopComparison();

        Debug.Log("Test completado.");
    }
}

