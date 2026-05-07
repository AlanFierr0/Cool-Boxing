using System;
using UnityEngine;

/// <summary>
/// Calcula un score básico comparando la distancia entre manos reales y ghost durante el playback.
/// </summary>
public class GhostPunchScorer : MonoBehaviour
{
    [Header("Fuente")]
    [SerializeField] private VRGhostPlayback playback;
    [SerializeField] private bool autoScoreOnPlayback = true;

    [Header("Manos reales")]
    [SerializeField] private Transform realLeftHand;
    [SerializeField] private Transform realRightHand;

    [Header("Manos ghost")]
    [SerializeField] private Transform ghostLeftHand;
    [SerializeField] private Transform ghostRightHand;

    [Header("Scoring")]
    [SerializeField, Min(0.001f)] private float maxErrorDistance = 0.35f;
    [SerializeField] private bool includeLeftHand = true;
    [SerializeField] private bool includeRightHand = true;

    public event Action<float, float> ScoreCompleted;

    public bool isScoring { get; private set; }
    public float averageError => _elapsedScoreTime > 0f ? _weightedErrorSum / _elapsedScoreTime : 0f;
    public float finalScore { get; private set; }

    private float _weightedErrorSum;
    private float _elapsedScoreTime;

    private void OnEnable()
    {
        if (playback != null)
        {
            playback.PlaybackStarted += HandlePlaybackStarted;
            playback.PlaybackStopped += HandlePlaybackStopped;
        }
    }

    private void OnDisable()
    {
        if (playback != null)
        {
            playback.PlaybackStarted -= HandlePlaybackStarted;
            playback.PlaybackStopped -= HandlePlaybackStopped;
        }
    }

    private void Update()
    {
        if (!isScoring || playback == null || !playback.IsPlaying)
        {
            return;
        }

        float sampleError = CalculateSampleError();
        float dt = Time.unscaledDeltaTime;
        _weightedErrorSum += sampleError * dt;
        _elapsedScoreTime += dt;
    }

    public void BeginScoring()
    {
        ResetScore();
        isScoring = true;
    }

    public void StopScoring()
    {
        if (!isScoring)
        {
            return;
        }

        isScoring = false;
        FinalizeScore();
    }

    public void ResetScore()
    {
        _weightedErrorSum = 0f;
        _elapsedScoreTime = 0f;
        finalScore = 0f;
    }

    private void HandlePlaybackStarted()
    {
        if (!autoScoreOnPlayback)
        {
            return;
        }

        BeginScoring();
    }

    private void HandlePlaybackStopped()
    {
        if (!isScoring)
        {
            return;
        }

        StopScoring();
    }

    private float CalculateSampleError()
    {
        float total = 0f;
        int count = 0;

        if (includeLeftHand && realLeftHand != null && ghostLeftHand != null)
        {
            total += Vector3.Distance(realLeftHand.position, ghostLeftHand.position);
            count++;
        }

        if (includeRightHand && realRightHand != null && ghostRightHand != null)
        {
            total += Vector3.Distance(realRightHand.position, ghostRightHand.position);
            count++;
        }

        if (count == 0)
        {
            return 0f;
        }

        return total / count;
    }

    private void FinalizeScore()
    {
        if (_elapsedScoreTime <= 0f)
        {
            finalScore = 0f;
            Debug.LogWarning("GhostPunchScorer: no se registraron muestras de score.");
            ScoreCompleted?.Invoke(finalScore, averageError);
            return;
        }

        float avgError = averageError;
        finalScore = Mathf.Clamp01(1f - (avgError / maxErrorDistance)) * 100f;

        Debug.Log($"Ghost punch score final: {finalScore:0.0}/100 | error promedio: {avgError:0.000} m | tiempo evaluado: {_elapsedScoreTime:0.000} s");
        ScoreCompleted?.Invoke(finalScore, avgError);
    }
}

