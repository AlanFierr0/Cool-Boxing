using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Detecta impactos de la mano contra hitboxes enemigos y calcula la velocidad lineal
/// de la mano en el momento del impacto. Llama a RobotHitReceiver.OnHit con una
/// intensidad normalizada basada en la velocidad.
/// 
/// Recomendaciones:
/// - El GameObject de la mano debe tener un Collider (Is Trigger = true) y un Rigidbody kinemático
///   (Add Component -> Rigidbody, set isKinematic = true) para que funcionen los callbacks de trigger.
/// - Las hitboxes enemigas deberían tener colliders y el componente RobotHitReceiver en un parent.
/// </summary>
[DisallowMultipleComponent]
public class HandImpactSensor : MonoBehaviour
{
    [Header("Detection")]
    [Tooltip("Tag (o layer) de los objetos que se consideran hitboxes enemigos. Si vacio, detecta cualquier collider.")]
    [SerializeField] private string targetTag = "";
    [Tooltip("Tiempo minimo entre impactos registrados (segundos) para evitar multi-trigger).")]
    [SerializeField] private float hitCooldownSeconds = 0.25f;

    [Header("Speed")]
    [Tooltip("Velocidad minima (m/s) para considerar un impacto con efecto.")]
    [SerializeField] private float minSpeedForHit = 1.0f;

    [Header("Smoothing")]
    [Tooltip("Número de muestras para suavizar la velocidad estimada. 1 = sin suavizado.")]
    [SerializeField, Range(1, 8)] private int smoothingSamples = 3;

    [Header("Recording")]
    [Tooltip("Guardar historial de velocidades de impacto para análisis.")]
    [SerializeField] private bool recordHistory = true;
    [Tooltip("Número maximo de impactos a mantener en el historial.")]
    [SerializeField] private int maxRecordedImpacts = 64;

    // estado interno
    private Vector3 _lastPosition;
    private Queue<Vector3> _velocitySamples = new Queue<Vector3>();
    private float _lastHitTime = -10f;
    private List<float> _recordedSpeeds = new List<float>();

    public float LastImpactSpeed { get; private set; }

    public IReadOnlyList<float> RecordedSpeeds => _recordedSpeeds;


    private void Start()
    {
        _lastPosition = transform.position;
        LastImpactSpeed = 0f;
    }

    private void Update()
    {
        // estimar velocidad por diferencia de posición (world space)
        float dt = Time.deltaTime;
        if (dt <= 0f)
            return;

        Vector3 currentPos = transform.position;
        Vector3 instVelocity = (currentPos - _lastPosition) / dt;
        _lastPosition = currentPos;

        // push sample
        _velocitySamples.Enqueue(instVelocity);
        if (_velocitySamples.Count > smoothingSamples)
            _velocitySamples.Dequeue();
    }

    private Vector3 GetSmoothedVelocity()
    {
        if (_velocitySamples.Count == 0)
            return Vector3.zero;

        Vector3 sum = Vector3.zero;
        foreach (var v in _velocitySamples)
            sum += v;
        return sum / _velocitySamples.Count;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!string.IsNullOrEmpty(targetTag) && !other.CompareTag(targetTag))
            return;

        float now = Time.time;
        if (now - _lastHitTime < hitCooldownSeconds)
            return; // cooldown

        // compute impact speed magnitude
        Vector3 vel = GetSmoothedVelocity();
        float speed = vel.magnitude;

        // si la velocidad es menor que el umbral, ignoramos (evitar rozes)
        if (speed < minSpeedForHit)
        {
            return;
        }


        _lastHitTime = now;
        LastImpactSpeed = speed;

        // record history
        if (recordHistory)
        {
            _recordedSpeeds.Add(speed);
            if (_recordedSpeeds.Count > maxRecordedImpacts)
                _recordedSpeeds.RemoveAt(0);
        }

        Debug.Log($"HandImpactSensor: Impact detected on '{other.name}' speed={speed:F2} m/s");
    }

    // También permitimos colisiones físicas si el hand usa non-trigger collider
    private void OnCollisionEnter(Collision collision)
    {
        // reutilizar la misma lógica que en trigger
        Collider other = collision.GetComponent<Collider>();
        OnTriggerEnter(other);
    }
}
