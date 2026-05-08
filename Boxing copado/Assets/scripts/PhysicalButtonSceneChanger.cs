using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Detecta cuando un objeto entra al trigger del boton fisico.
/// Cambia de escena cuando se presiona.
/// Asigna este script al GameObject del boton fisico.
/// </summary>
public class PhysicalButtonSceneChanger : MonoBehaviour
{
    [SerializeField] private string sceneName = "gameScene";
    [SerializeField] private float cooldownSeconds = 1f; // Evita multiples pulsaciones en corto tiempo

    private float lastPressTime = 0f;

    private void OnTriggerEnter(Collider other)
    {
        // Solo si es la mano del jugador
        if (!other.CompareTag("Hand") && !other.CompareTag("Controller"))
        {
            return;
        }

        // Cooldown para evitar multiples pulsaciones
        if (Time.time - lastPressTime < cooldownSeconds)
        {
            return;
        }

        lastPressTime = Time.time;

        Debug.Log($"PhysicalButtonSceneChanger: Boton presionado. Cambiando a '{sceneName}'...");
        SceneManager.LoadScene(sceneName);
    }
}

