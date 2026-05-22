using UnityEngine;

/// <summary>
/// StateMachineBehaviour ejemplo para leer el parámetro "HitVelocity" que pone
/// `HandImpactSensor` y adaptar la reproducción de la animación de reacción.
/// 
/// Cómo usar:
/// 1. En tu Animator Controller, crea un parámetro float llamado "HitVelocity".
/// 2. Selecciona el estado de reacción (p.ej. "HitReaction") y pulsa "Add Behaviour" -> "HitReactionBehaviour".
/// 3. Ajusta los campos minSpeed/maxSpeed y el nombre del parámetro de salida (HitIntensity) si lo deseas.
/// 4. En las transiciones o BlendTree puedes usar "HitIntensity" (0..1) para controlar variantes.
/// 
/// Este behaviour opcionalmente ajusta la velocidad del Animator durante el estado para dar más dinamismo.
/// </summary>
public class HitReactionBehaviour : StateMachineBehaviour
{
    [Tooltip("Velocidad (m/s) mínima que corresponde a intensidad 0.")]
    public float minSpeed = 1.0f;
    [Tooltip("Velocidad (m/s) a la cual la intensidad llega a 1.")]
    public float maxSpeed = 5.0f;

    [Tooltip("Nombre del parámetro float que se escribirá con la intensidad normalizada (0..1).")]
    public string intensityParam = "HitIntensity";

    [Tooltip("Si true, modifica temporariamente Animator.speed en base a la intensidad.")]
    public bool modifyAnimatorSpeed = true;
    [Tooltip("Velocidad mínima del animator (cuando intensidad=0)")]
    public float animatorSpeedMin = 0.9f;
    [Tooltip("Velocidad máxima del animator (cuando intensidad=1)")]
    public float animatorSpeedMax = 1.25f;

    // almacenamiento temporal del speed anterior
    private float _prevAnimatorSpeed = 1f;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // leer el parámetro HitVelocity (puede venir de HandImpactSensor)
        float hitVel = animator.GetFloat("HitVelocity");

        // normalizar a 0..1
        float intensity = Mathf.InverseLerp(minSpeed, maxSpeed, hitVel);

        // escribir intensidad en Animator para que transitions/blendtrees la usen
        if (!string.IsNullOrEmpty(intensityParam))
            animator.SetFloat(intensityParam, intensity);

        // opcional: ajustar velocidad del animator para que la animación sea más rápida en golpes fuertes
        if (modifyAnimatorSpeed)
        {
            _prevAnimatorSpeed = animator.speed;
            animator.speed = Mathf.Lerp(animatorSpeedMin, animatorSpeedMax, intensity);
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // restaurar velocidad original si la modificamos
        if (modifyAnimatorSpeed)
        {
            animator.speed = _prevAnimatorSpeed;
        }
    }
}

