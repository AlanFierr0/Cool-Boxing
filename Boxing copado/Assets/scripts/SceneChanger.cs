using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Script simple para cambiar de escena.
/// Asigna este script a un Button en el Inspector.
/// </summary>
public class SceneChanger : MonoBehaviour
{
    /// <summary>
    /// Nombre de la escena a cargar.
    /// Por defecto: "gameScene"
    /// </summary>
    [SerializeField] private string sceneName = "gameScene";

    /// <summary>
    /// Cambia a la escena especificada.
    /// Llama este metodo desde el onClick del Button.
    /// </summary>
    public void ChangeScene()
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("SceneChanger: sceneName no configurado.");
            return;
        }

        Debug.Log($"SceneChanger: Cambiando a escena '{sceneName}'...");
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Cambia a una escena especifica por nombre.
    /// </summary>
    public void ChangeScene(string scene)
    {
        if (string.IsNullOrEmpty(scene))
        {
            Debug.LogError("SceneChanger: nombre de escena vacio.");
            return;
        }

        Debug.Log($"SceneChanger: Cambiando a escena '{scene}'...");
        SceneManager.LoadScene(scene);
    }
}

