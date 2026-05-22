using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Objeto "medidor" neutro que, cuando se le indica, muestrea la posición de un transform
/// (p. ej. el guante) durante un breve intervalo y calcula una estimación de la velocidad
/// de impacto en m/s.
/// 
/// Uso: llamar StartSampling(handTransform, duration, interval, callback)
/// El callback recibirá la velocidad (float, m/s) medida.
/// Soporta múltiples manos simultáneas (usa un diccionario de coroutines por transform).
/// </summary>
[DisallowMultipleComponent]
public class ImpactMeter : MonoBehaviour
{
    [Header("Defaults")]
    [Tooltip("Duración por defecto de muestreo en segundos.")]
    [SerializeField] private float defaultSampleDuration = 0.12f;
    [Tooltip("Intervalo por defecto entre samples (segundos).")]
    [SerializeField] private float defaultSampleInterval = 0.02f;

    // active coroutines por transform
    private Dictionary<Transform, Coroutine> _active = new Dictionary<Transform, Coroutine>();

    /// <summary>
    /// Inicia el muestreo de la posición del transform dado. Cuando finaliza, llama al callback
    /// con la velocidad estimada (m/s).
    /// </summary>
    public void StartSampling(Transform handTransform, Action<float> onComplete)
    {
        StartSampling(handTransform, defaultSampleDuration, defaultSampleInterval, onComplete);
    }

    /// <summary>
    /// Inicia el muestreo con parámetros personalizados.
    /// </summary>
    public void StartSampling(Transform handTransform, float duration, float interval, Action<float> onComplete)
    {
        if (handTransform == null)
        {
            Debug.LogWarning("ImpactMeter: handTransform null, no se inicia muestreo.");
            onComplete?.Invoke(0f);
            return;
        }

        // si ya hay un muestreo activo para esta mano, lo reemplazamos
        if (_active.TryGetValue(handTransform, out var existing))
        {
            StopCoroutine(existing);
            _active.Remove(handTransform);
        }

        Coroutine c = StartCoroutine(SampleCoroutine(handTransform, duration, interval, onComplete));
        _active[handTransform] = c;
    }

    private IEnumerator SampleCoroutine(Transform hand, float duration, float interval, Action<float> onComplete)
    {
        float elapsed = 0f;
        List<Vector3> positions = new List<Vector3>();

        // primera muestra inmediata
        positions.Add(hand.position);
        elapsed += 0f;

        while (elapsed < duration)
        {
            yield return new WaitForSeconds(interval);
            positions.Add(hand.position);
            elapsed += interval;
        }

        // calcular velocidades instantaneas entre samples
        if (positions.Count < 2)
        {
            onComplete?.Invoke(0f);
            _active.Remove(hand);
            yield break;
        }

        float maxSpeed = 0f;
        for (int i = 1; i < positions.Count; i++)
        {
            // velocidad aproximada entre posiciones
            float dt = interval;
            // por seguridad, si dt==0 usamos 1
            if (dt <= 0f) dt = Time.deltaTime;
            float speed = Vector3.Distance(positions[i], positions[i - 1]) / dt;
            if (speed > maxSpeed) maxSpeed = speed;
        }

        // limpieza
        _active.Remove(hand);

        // devolver la máxima velocidad detectada en la ventana (m/s)
        onComplete?.Invoke(maxSpeed);
    }
}

