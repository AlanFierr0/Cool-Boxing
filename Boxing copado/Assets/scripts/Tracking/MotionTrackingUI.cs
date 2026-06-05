using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controlador simple para UI: botones llaman estos métodos para grabar/guardar/cargar/compare.
/// Asignar en inspector: recorder y scoreDisplay.
/// </summary>
namespace Tracking
{
    public class MotionTrackingUI : MonoBehaviour
    {
        public MotionRecorder recorder;
        public MotionScoreDisplay scoreDisplay;
        public Text statusText;
        public string referenceName = "reference1";
        public float recordingDelaySeconds = 3f;
        public float statusMessageSeconds = 2f;

        MotionRecording _loadedReference;
        Coroutine _startRecordRoutine;
        Coroutine _statusRoutine;

        public void StartRecord()
        {
            if (recorder == null) { Debug.LogError("No MotionRecorder assigned"); return; }

            if (_startRecordRoutine != null)
            {
                StopCoroutine(_startRecordRoutine);
            }

            _startRecordRoutine = StartCoroutine(StartRecordDelayedRoutine());
        }

        public void StopAndSaveReference()
        {
            if (recorder == null) { Debug.LogError("No MotionRecorder assigned"); return; }
            _loadedReference = recorder.StopRecordingAndGet(referenceName);
            ShowTemporaryStatus($"Referencia guardada: {referenceName}");
        }

        public void StopAndKeepAttempt()
        {
            if (recorder == null) { Debug.LogError("No MotionRecorder assigned"); return; }
            // stop but don't save to disk
            MotionRecording attempt = recorder.StopRecordingAndGet(string.Empty);
            if (_loadedReference == null) _loadedReference = recorder.LoadRecordingFromFile(referenceName);
            if (_loadedReference == null)
            {
                Debug.LogError("No reference loaded or saved yet");
                return;
            }
            float score = MotionComparer.CompareDtw(_loadedReference, attempt);
            if (scoreDisplay != null) scoreDisplay.ShowScore(score);
            ShowTemporaryStatus($"Comparación lista. Score: {score:F1}");
        }

        public void LoadReferenceFromFile()
        {
            if (recorder == null) { Debug.LogError("No MotionRecorder assigned"); return; }
            _loadedReference = recorder.LoadRecordingFromFile(referenceName);
            if (_loadedReference != null)
            {
                Debug.Log("Reference loaded: " + referenceName);
                ShowTemporaryStatus($"Referencia cargada: {referenceName}");
            }
        }

        IEnumerator StartRecordDelayedRoutine()
        {
            ShowTemporaryStatus($"Grabación en {recordingDelaySeconds:0} segundos...");

            float elapsed = 0f;
            while (elapsed < recordingDelaySeconds)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }

            recorder.StartRecording();
            ShowTemporaryStatus("¡Grabando!");
            _startRecordRoutine = null;
        }

        void ShowTemporaryStatus(string message)
        {
            if (_statusRoutine != null)
            {
                StopCoroutine(_statusRoutine);
            }

            if (statusText != null)
            {
                statusText.text = message;
                _statusRoutine = StartCoroutine(ClearStatusAfterDelay());
            }
            else
            {
                Debug.Log(message);
            }
        }

        IEnumerator ClearStatusAfterDelay()
        {
            yield return new WaitForSeconds(statusMessageSeconds);

            if (statusText != null)
            {
                statusText.text = string.Empty;
            }

            _statusRoutine = null;
        }
    }
}

