using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Escucha un InputActionReference para alternar la grabación del ghost punch.
/// </summary>
[DisallowMultipleComponent]
public class VRRecorderInput : MonoBehaviour
{
    [SerializeField] private VRGhostRecorder recorder;
    [SerializeField] private InputActionReference toggleRecordingAction;

    private void Reset()
    {
        recorder = GetComponent<VRGhostRecorder>();
    }

    private void Awake()
    {
        if (recorder == null)
        {
            recorder = GetComponent<VRGhostRecorder>();
        }
    }

    private void OnEnable()
    {
        if (toggleRecordingAction?.action == null)
        {
            return;
        }

        toggleRecordingAction.action.performed += OnTogglePerformed;
        toggleRecordingAction.action.Enable();
    }

    private void OnDisable()
    {
        if (toggleRecordingAction?.action == null)
        {
            return;
        }

        toggleRecordingAction.action.performed -= OnTogglePerformed;
        toggleRecordingAction.action.Disable();
    }

    public void ToggleRecording()
    {
        if (recorder == null)
        {
            Debug.LogWarning("VRRecorderInput: no hay VRGhostRecorder asignado.");
            return;
        }

        recorder.ToggleRecording();
    }

    private void OnTogglePerformed(InputAction.CallbackContext context)
    {
        ToggleRecording();
    }
}

