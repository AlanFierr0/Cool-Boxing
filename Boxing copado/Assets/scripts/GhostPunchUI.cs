using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI simple para mostrar estado de grabación, reproducción y score.
/// Opcional: usado solo si querés visualizar el score en el headset sin logcat.
/// </summary>
public class GhostPunchUI : MonoBehaviour
{
    [SerializeField] private VRGhostRecorder recorder;
    [SerializeField] private VRGhostPlayback playback;
    [SerializeField] private GhostPunchScorer scorer;

    [Header("UI")]
    [SerializeField] private Text statusText;
    [SerializeField] private Text scoreText;

    private void OnEnable()
    {
        if (recorder != null)
        {
            recorder.RecordingStarted += OnRecordingStarted;
            recorder.RecordingStopped += OnRecordingStopped;
        }

        if (playback != null)
        {
            playback.PlaybackStarted += OnPlaybackStarted;
            playback.PlaybackStopped += OnPlaybackStopped;
        }

        if (scorer != null)
        {
            scorer.ScoreCompleted += OnScoreCompleted;
        }
    }

    private void OnDisable()
    {
        if (recorder != null)
        {
            recorder.RecordingStarted -= OnRecordingStarted;
            recorder.RecordingStopped -= OnRecordingStopped;
        }

        if (playback != null)
        {
            playback.PlaybackStarted -= OnPlaybackStarted;
            playback.PlaybackStopped -= OnPlaybackStopped;
        }

        if (scorer != null)
        {
            scorer.ScoreCompleted -= OnScoreCompleted;
        }
    }

    private void Update()
    {
        UpdateStatusText();

        if (playback != null && playback.IsPlaying && scorer != null)
        {
            if (scoreText != null)
            {
                scoreText.text = $"Error: {scorer.averageError:0.000}m";
            }
        }
    }

    private void UpdateStatusText()
    {
        if (statusText == null)
        {
            return;
        }

        string status = "READY";

        if (recorder != null && recorder.IsRecording)
        {
            status = "RECORDING...";
        }
        else if (playback != null && playback.IsPlaying)
        {
            status = $"PLAYBACK {playback.PlaybackTime:0.0}s";
        }

        statusText.text = status;
    }

    private void OnRecordingStarted()
    {
        if (statusText != null)
        {
            statusText.text = "RECORDING";
        }

        if (scoreText != null)
        {
            scoreText.text = "";
        }
    }

    private void OnRecordingStopped(System.Collections.Generic.IReadOnlyList<VRGhostFrame> frames)
    {
        if (statusText != null)
        {
            statusText.text = $"STOPPED ({frames.Count} frames)";
        }
    }

    private void OnPlaybackStarted()
    {
        if (statusText != null)
        {
            statusText.text = "PLAYBACK";
        }
    }

    private void OnPlaybackStopped()
    {
        if (statusText != null)
        {
            statusText.text = "READY";
        }
    }

    private void OnScoreCompleted(float score, float avgError)
    {
        if (scoreText != null)
        {
            scoreText.text = $"SCORE: {score:0.0}/100";
        }
    }
}


