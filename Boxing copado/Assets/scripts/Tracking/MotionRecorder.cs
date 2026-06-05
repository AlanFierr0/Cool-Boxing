using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Tracking
{
    /// <summary>
    /// Graba la trayectoria de dos transforms (guantes) y la guarda/lee como JSON.
    /// Uso: añadir a un GameObject en la escena y asignar leftGlove/rightGlove.
    /// Llamar a StartRecording/StopRecording/LoadRecording/CompareRecording desde UI o código.
    /// </summary>
    public class MotionRecorder : MonoBehaviour
    {
        [Header("References")]
        public Transform leftGlove;
        public Transform rightGlove;

        [Header("Recording")]
        public float sampleRate = 60f; // samples per second

        bool _recording;
        float _sampleTimer;
        List<MotionFrame> _frames = new List<MotionFrame>();
        float _startTime;

        public bool isRecording => _recording;

        void Update()
        {
            if (!_recording) return;

            _sampleTimer += Time.deltaTime;
            float interval = 1f / Mathf.Max(1f, sampleRate);
            while (_sampleTimer >= interval)
            {
                _sampleTimer -= interval;
                SampleFrame();
            }
        }

        void SampleFrame()
        {
            if (leftGlove == null || rightGlove == null) return;
            MotionFrame frame = new MotionFrame
            {
                time = Time.time - _startTime,
                leftPos = leftGlove.position,
                rightPos = rightGlove.position,
                leftRot = leftGlove.rotation,
                rightRot = rightGlove.rotation
            };
            _frames.Add(frame);
        }

        public void StartRecording()
        {
            _frames.Clear();
            _recording = true;
            _startTime = Time.time;
            _sampleTimer = 0f;
            // sample immediately
            SampleFrame();
            Debug.Log("MotionRecorder: started");
        }

        public MotionRecording StopRecordingAndGet(string recordingName = null)
        {
            _recording = false;
            MotionRecording recording = MotionRecording.FromList(_frames);
            if (!string.IsNullOrEmpty(recordingName)) SaveRecordingToFile(recording, recordingName);
            Debug.Log($"MotionRecorder: stopped, frames={_frames.Count}");
            return recording;
        }

        public void SaveRecordingToFile(MotionRecording rec, string recordingName)
        {
            string folder = Path.Combine(Application.persistentDataPath, "recordings");
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
            string path = Path.Combine(folder, recordingName + ".json");
            try
            {
                File.WriteAllText(path, rec.ToJson(true));
                Debug.Log("Saved recording to: " + path);
            }
            catch (System.Exception e)
            {
                Debug.LogError("Failed saving recording: " + e);
            }
        }

        public MotionRecording LoadRecordingFromFile(string recordingName)
        {
            string path = Path.Combine(Application.persistentDataPath, "recordings", recordingName + ".json");
            if (!File.Exists(path))
            {
                Debug.LogError("Recording file not found: " + path);
                return null;
            }
            try
            {
                string json = File.ReadAllText(path);
                MotionRecording recording = MotionRecording.FromJson(json);
                return recording;
            }
            catch (System.Exception e)
            {
                Debug.LogError("Failed loading recording: " + e);
                return null;
            }
        }
    }
}

