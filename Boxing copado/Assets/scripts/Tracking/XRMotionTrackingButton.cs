using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Tracking
{
    /// <summary>
    /// Botón XR para disparar acciones de tracking sin teclado ni UI tradicional.
    /// Asignar este script al mismo GameObject que tenga un XRSimpleInteractable.
    /// </summary>
    public class XRMotionTrackingButton : MonoBehaviour
    {
        public enum ActionType
        {
            StartRecord,
            StopAndSaveReference,
            LoadReferenceFromFile,
            StopAndCompare
        }

        [SerializeField] private ActionType actionType = ActionType.StartRecord;
        [SerializeField] private MotionTrackingUI trackingUI;

        private UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable interactable;

        private void OnEnable()
        {
            interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();

            if (interactable == null)
            {
                Debug.LogError("XRMotionTrackingButton: No hay XRSimpleInteractable en este GameObject.");
                return;
            }

            interactable.selectEntered.AddListener(OnButtonPressed);
        }

        private void OnDisable()
        {
            if (interactable != null)
            {
                interactable.selectEntered.RemoveListener(OnButtonPressed);
            }
        }

        private void OnButtonPressed(SelectEnterEventArgs args)
        {
            if (trackingUI == null)
            {
                Debug.LogError("XRMotionTrackingButton: No hay MotionTrackingUI asignado.");
                return;
            }

            switch (actionType)
            {
                case ActionType.StartRecord:
                    trackingUI.StartRecord();
                    break;
                case ActionType.StopAndSaveReference:
                    trackingUI.StopAndSaveReference();
                    break;
                case ActionType.LoadReferenceFromFile:
                    trackingUI.LoadReferenceFromFile();
                    break;
                case ActionType.StopAndCompare:
                    trackingUI.StopAndKeepAttempt();
                    break;
            }

            Debug.Log($"XRMotionTrackingButton: Ejecutada acción {actionType}.");
        }
    }
}

