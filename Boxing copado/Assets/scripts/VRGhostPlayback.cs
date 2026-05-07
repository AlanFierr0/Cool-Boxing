using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Reproduce la grabación como ghost, convirtiendo de espacio local a mundo y suavizando entre frames.
/// </summary>
public class VRGhostPlayback : MonoBehaviour
{
    [Header("Fuente de datos")]
    [SerializeField] private VRGhostRecorder recorder;
    [SerializeField] private bool autoPlayOnRecordingStopped = true;

    [Header("Rig ghost")]
    [SerializeField] private Transform playerRoot;
    [SerializeField] private Transform ghostHead;
    [SerializeField] private Transform ghostLeftHand;
    [SerializeField] private Transform ghostRightHand;

    [Header("Playback")]
    [SerializeField, Min(0f)] private float playbackSpeed = 1f;
    [SerializeField] private bool loopPlayback;

    public event Action PlaybackStarted;
    public event Action PlaybackStopped;

    private readonly List<VRGhostFrame> frames = new List<VRGhostFrame>(512);
    private bool isPlaying;
    private float playbackTime;
    private int cachedFrameIndex;

    public bool IsPlaying => isPlaying;
    public IReadOnlyList<VRGhostFrame> Frames => frames;
    public float PlaybackTime => playbackTime;

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
        if (recorder != null)
        {
            recorder.RecordingStopped += HandleRecordingStopped;
        }
    }

    private void OnDisable()
    {
        if (recorder != null)
        {
            recorder.RecordingStopped -= HandleRecordingStopped;
        }
    }

    private void Update()
    {
        if (!isPlaying || frames.Count == 0)
        {
            return;
        }

        float duration = frames[frames.Count - 1].time;
        if (duration <= 0f)
        {
            ApplyFrame(frames[0]);
            return;
        }

        playbackTime += Time.unscaledDeltaTime * playbackSpeed;

        if (playbackTime > duration)
        {
            if (loopPlayback)
            {
                playbackTime %= duration;
                cachedFrameIndex = 0;
            }
            else
            {
                ApplyFrame(frames[frames.Count - 1]);
                Stop();
                return;
            }
        }

        ApplyFrameAtTime(playbackTime);
    }

    public void LoadFrames(IReadOnlyList<VRGhostFrame> sourceFrames)
    {
        frames.Clear();

        if (sourceFrames == null)
        {
            return;
        }

        for (int i = 0; i < sourceFrames.Count; i++)
        {
            frames.Add(sourceFrames[i]);
        }

        frames.Sort((a, b) => a.time.CompareTo(b.time));
        cachedFrameIndex = 0;
        playbackTime = 0f;
    }

    public void Play()
    {
        if (frames.Count == 0)
        {
            Debug.LogWarning("VRGhostPlayback: no hay frames cargados para reproducir.");
            return;
        }

        if (playerRoot == null)
        {
            playerRoot = transform;
        }

        isPlaying = true;
        playbackTime = 0f;
        cachedFrameIndex = 0;
        ApplyFrameAtTime(0f);
        PlaybackStarted?.Invoke();
    }

    public void Stop()
    {
        if (!isPlaying)
        {
            return;
        }

        isPlaying = false;
        PlaybackStopped?.Invoke();
    }

    public void PlayFromRecorder()
    {
        if (recorder == null)
        {
            Debug.LogWarning("VRGhostPlayback: no hay recorder asignado.");
            return;
        }

        LoadFrames(recorder.Frames);
        Play();
    }

    private void HandleRecordingStopped(IReadOnlyList<VRGhostFrame> recordedFrames)
    {
        LoadFrames(recordedFrames);

        if (autoPlayOnRecordingStopped)
        {
            Play();
        }
    }

    private void ApplyFrameAtTime(float time)
    {
        if (frames.Count == 0)
        {
            return;
        }

        if (frames.Count == 1)
        {
            ApplyFrame(frames[0]);
            return;
        }

        if (time <= frames[0].time)
        {
            cachedFrameIndex = 0;
            ApplyFrame(frames[0]);
            return;
        }

        int lastIndex = frames.Count - 1;
        if (time >= frames[lastIndex].time)
        {
            cachedFrameIndex = lastIndex - 1;
            ApplyFrame(frames[lastIndex]);
            return;
        }

        cachedFrameIndex = Mathf.Clamp(cachedFrameIndex, 0, frames.Count - 2);

        while (cachedFrameIndex < frames.Count - 2 && frames[cachedFrameIndex + 1].time < time)
        {
            cachedFrameIndex++;
        }

        while (cachedFrameIndex > 0 && frames[cachedFrameIndex].time > time)
        {
            cachedFrameIndex--;
        }

        VRGhostFrame a = frames[cachedFrameIndex];
        VRGhostFrame b = frames[cachedFrameIndex + 1];
        float t = Mathf.InverseLerp(a.time, b.time, time);

        ApplyInterpolatedFrame(a, b, t);
    }

    private void ApplyInterpolatedFrame(in VRGhostFrame a, in VRGhostFrame b, float t)
    {
        ApplyPose(ghostHead, Vector3.Lerp(a.headLocalPosition, b.headLocalPosition, t), Quaternion.Slerp(a.headLocalRotation, b.headLocalRotation, t));
        ApplyPose(ghostLeftHand, Vector3.Lerp(a.leftHandLocalPosition, b.leftHandLocalPosition, t), Quaternion.Slerp(a.leftHandLocalRotation, b.leftHandLocalRotation, t));
        ApplyPose(ghostRightHand, Vector3.Lerp(a.rightHandLocalPosition, b.rightHandLocalPosition, t), Quaternion.Slerp(a.rightHandLocalRotation, b.rightHandLocalRotation, t));
    }

    private void ApplyFrame(in VRGhostFrame frame)
    {
        ApplyPose(ghostHead, frame.headLocalPosition, frame.headLocalRotation);
        ApplyPose(ghostLeftHand, frame.leftHandLocalPosition, frame.leftHandLocalRotation);
        ApplyPose(ghostRightHand, frame.rightHandLocalPosition, frame.rightHandLocalRotation);
    }

    private void ApplyPose(Transform target, Vector3 localPosition, Quaternion localRotation)
    {
        if (target == null)
        {
            return;
        }

        if (playerRoot == null)
        {
            playerRoot = transform;
        }

        Vector3 worldPosition = playerRoot.TransformPoint(localPosition);
        Quaternion worldRotation = playerRoot.rotation * localRotation;
        target.SetPositionAndRotation(worldPosition, worldRotation);
    }
}

