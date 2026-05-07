using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Graba snapshots del headset y los controladores reales en espacio local del playerRoot.
/// </summary>
public class VRGhostRecorder : MonoBehaviour
{
    [Header("Rig real")]
    [SerializeField] private Transform playerRoot;
    [SerializeField] private Transform headTransform;
    [SerializeField] private Transform leftHandTransform;
    [SerializeField] private Transform rightHandTransform;

    [Header("Grabación")]
    [SerializeField, Min(0.001f)] private float sampleInterval = 0.02f;
    [SerializeField] private bool recordOnEnable;

    public event Action RecordingStarted;
    public event Action<IReadOnlyList<VRGhostFrame>> RecordingStopped;

    private readonly List<VRGhostFrame> _frames = new List<VRGhostFrame>(512);
    private bool _isRecording;
    private float _nextSampleTime;
    private float _recordingTime;

    public bool IsRecording => _isRecording;
    public IReadOnlyList<VRGhostFrame> Frames => _frames;
    public float SampleInterval => sampleInterval;

    // Alias opcional por si quieres leer el estado con un nombre más tipo inspector.
    public bool isRecording => _isRecording;
    public IReadOnlyList<VRGhostFrame> frames => _frames;
    public float sampleIntervalValue => sampleInterval;

    private void Reset()
    {
        playerRoot = transform;
    }

    private void Awake()
    {
        if (playerRoot == null)
        {
            playerRoot = transform;
        }
    }

    private void OnEnable()
    {
        if (recordOnEnable)
        {
            StartRecording();
        }
    }

    private void Update()
    {
        if (!_isRecording)
        {
            return;
        }

        _recordingTime += Time.unscaledDeltaTime;

        while (_recordingTime >= _nextSampleTime)
        {
            CaptureFrame(_nextSampleTime);
            _nextSampleTime += sampleInterval;
        }
    }

    public void StartRecording()
    {
        if (_isRecording)
        {
            return;
        }

        if (playerRoot == null)
        {
            playerRoot = transform;
        }

        _frames.Clear();
        _isRecording = true;
        _recordingTime = 0f;
        _nextSampleTime = 0f;

        CaptureFrame(0f);
        _nextSampleTime = sampleInterval;

        RecordingStarted?.Invoke();
    }

    public void StopRecording()
    {
        if (!_isRecording)
        {
            return;
        }

        _isRecording = false;
        RecordingStopped?.Invoke(_frames);
    }

    public void ToggleRecording()
    {
        if (_isRecording)
        {
            StopRecording();
        }
        else
        {
            StartRecording();
        }
    }

    public void ClearRecording()
    {
        _frames.Clear();
        _recordingTime = 0f;
        _nextSampleTime = 0f;
    }

    private void CaptureFrame(float time)
    {
        if (playerRoot == null)
        {
            playerRoot = transform;
        }

        Transform headSource = headTransform != null ? headTransform : Camera.main != null ? Camera.main.transform : null;
        Transform leftSource = leftHandTransform;
        Transform rightSource = rightHandTransform;

        Vector3 headLocalPosition = SampleLocalPosition(headSource);
        Quaternion headLocalRotation = SampleLocalRotation(headSource);

        Vector3 leftLocalPosition = SampleLocalPosition(leftSource);
        Quaternion leftLocalRotation = SampleLocalRotation(leftSource);

        Vector3 rightLocalPosition = SampleLocalPosition(rightSource);
        Quaternion rightLocalRotation = SampleLocalRotation(rightSource);

        _frames.Add(new VRGhostFrame(
            time,
            headLocalPosition,
            headLocalRotation,
            leftLocalPosition,
            leftLocalRotation,
            rightLocalPosition,
            rightLocalRotation));
    }

    private Vector3 SampleLocalPosition(Transform source)
    {
        if (source == null)
        {
            return Vector3.zero;
        }

        return playerRoot.InverseTransformPoint(source.position);
    }

    private Quaternion SampleLocalRotation(Transform source)
    {
        if (source == null)
        {
            return Quaternion.identity;
        }

        return Quaternion.Inverse(playerRoot.rotation) * source.rotation;
    }
}

