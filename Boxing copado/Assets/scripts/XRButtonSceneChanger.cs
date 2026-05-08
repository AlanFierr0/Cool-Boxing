using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Cambia de escena cuando se interactúa con un XRSimpleInteractable.
/// Usa XR Interaction Toolkit.
/// Asigna este script al mismo GameObject que tiene XRSimpleInteractable.
/// </summary>
public class XRButtonSceneChanger : MonoBehaviour
{
    [SerializeField] private string sceneName = "gameScene";
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable interactable;

    private void OnEnable()
    {
        interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();

        if (interactable == null)
        {
            Debug.LogError("XRButtonSceneChanger: No hay XRSimpleInteractable en este GameObject.");
            return;
        }

        // Conectar evento
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
        Debug.Log($"XRButtonSceneChanger: Boton presionado. Cambiando a '{sceneName}'...");
        SceneManager.LoadScene(sceneName);
    }
}

