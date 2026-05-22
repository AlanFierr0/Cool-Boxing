using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Component to attach to RobotKyle (or a parent). Receives hit notifications from hitbox
/// colliders and drives the Animator to play hit reactions. Configure trigger/parameters
/// to match your Animator Controller.
/// </summary>
public class RobotHitReceiver : MonoBehaviour
{
    [System.Serializable]
    public class HitboxMapping
    {
        public string hitboxName = "Head";
        public int areaIndex;
    }

    [Header("Animator / parameters")]
    [SerializeField] private Animator animator;
    [Tooltip("Trigger param name used to play a generic hit reaction.")]
    [SerializeField] private string hitTrigger = "Hit";
    [Tooltip("Optional float param name for hit intensity (0..1)")]
    [SerializeField] private string intensityParam = "HitPower";
    [Tooltip("Optional int param to indicate hit area (head=0, torso=1, left=2, right=3)")]
    [SerializeField] private string areaParam = "HitArea";

    [Header("Idle State Management")]
    [Tooltip("Name of the boolean parameter that controls idle state")]
    [SerializeField] private string idleBoolParam = "isIdle";
    [Tooltip("Duration (in seconds) to keep isIdle OFF during hit reaction")]
    [SerializeField] private float hitReactionDuration = 0.8f;

    [Header("Hitbox Area Mapping")]
    [SerializeField] private List<HitboxMapping> hitboxMappings = new List<HitboxMapping>
    {
        new HitboxMapping { hitboxName = "Head", areaIndex = 0 },
        new HitboxMapping { hitboxName = "Torso", areaIndex = 1 },
        new HitboxMapping { hitboxName = "LeftArm", areaIndex = 2 },
        new HitboxMapping { hitboxName = "RightArm", areaIndex = 3 }
    };

    private Dictionary<string, int> _hitboxToAreaMap;

    [Header("Impact Meter (optional)")]
    [Tooltip("Referencia a un ImpactMeter que medirá la velocidad del guante después de que se detecte un hit. Si está asignado, el RobotHitReceiver pedirá una muestra y colocará el parámetro 'HitVelocity' en el Animator.")]
    [SerializeField] private ImpactMeter impactMeter;
    [Tooltip("Duración del muestreo (s) que se le pedirá al ImpactMeter si está presente.")]
    [SerializeField] private float impactSampleDuration = 0.12f;
    [Tooltip("Intervalo de muestreo (s) que se le pedirá al ImpactMeter si está presente.")]
    [SerializeField] private float impactSampleInterval = 0.02f;

    private void OnEnable()
    {
        BuildHitboxMap();
    }

    private void BuildHitboxMap()
    {
        _hitboxToAreaMap = new Dictionary<string, int>(System.StringComparer.OrdinalIgnoreCase);
        foreach (var mapping in hitboxMappings)
        {
            _hitboxToAreaMap[mapping.hitboxName] = mapping.areaIndex;
        }
    }

    /// <summary>
    /// Called by a hitbox when it detects a collision/overlap.
    /// </summary>
    public void OnHit(GameObject hitter, Vector3 contactPoint, Transform hitboxTransform, float intensity)
    {
        if (animator == null)
        {
            Debug.LogWarning($"{name}: Received hit from {hitter.name} but no Animator assigned.");
            return;
        }

        if (animator.runtimeAnimatorController == null)
        {
            Debug.LogWarning($"{name}: Animator has no RuntimeAnimatorController assigned. The Animator Controller must be set in the Inspector.");
            return;
        }

        // clamp intensity
        intensity = Mathf.Clamp01(intensity);

        // Turn OFF isIdle to allow hit reaction animations to play
        if (!string.IsNullOrEmpty(idleBoolParam))
            animator.SetBool(idleBoolParam, false);

        if (!string.IsNullOrEmpty(intensityParam))
            animator.SetFloat(intensityParam, intensity);

        if (!string.IsNullOrEmpty(areaParam))
            animator.SetInteger(areaParam, GetAreaIndex(hitboxTransform.name));

        if (!string.IsNullOrEmpty(hitTrigger))
            animator.SetTrigger(hitTrigger);

        Debug.Log($"Hit received from {hitter.name} on {hitboxTransform.name} (area: {GetAreaIndex(hitboxTransform.name)}, power: {intensity:F2})");

        // Si hay un ImpactMeter asignado, pedimos que mida la velocidad del guante del 'hitter'
        if (impactMeter != null && hitter != null)
        {
            Transform handTransform = hitter.transform;
            impactMeter.StartSampling(handTransform, impactSampleDuration, impactSampleInterval, (measuredSpeed) =>
            {
                // Guardar velocidad directamente en el Animator como parámetro HitVelocity (float)
                if (animator != null)
                {
                    animator.SetFloat("HitVelocity", measuredSpeed);
                    Debug.Log($"RobotHitReceiver: ImpactMeter measured speed={measuredSpeed:F2} m/s and set Animator HitVelocity.");
                }
            });
        }

        // Schedule isIdle to turn back ON after the hit reaction duration
        StopCoroutine(ResumeIdleAfterDelay());
        StartCoroutine(ResumeIdleAfterDelay());
    }

    private IEnumerator ResumeIdleAfterDelay()
    {
        yield return new WaitForSeconds(hitReactionDuration);
        
        // Validate animator is still valid and has a controller
        if (animator == null)
        {
            Debug.LogWarning("RobotHitReceiver: Animator was destroyed or null during ResumeIdle.");
            yield break;
        }

        if (!animator.gameObject.activeInHierarchy)
        {
            Debug.LogWarning("RobotHitReceiver: Animator GameObject is inactive during ResumeIdle.");
            yield break;
        }

        if (animator.runtimeAnimatorController == null)
        {
            Debug.LogWarning("RobotHitReceiver: Animator has no RuntimeAnimatorController assigned.");
            yield break;
        }

        if (!string.IsNullOrEmpty(idleBoolParam))
            animator.SetBool(idleBoolParam, true);
    }

    private int GetAreaIndex(string hitboxName)
    {
        if (_hitboxToAreaMap == null || _hitboxToAreaMap.Count == 0)
            BuildHitboxMap();

        if (_hitboxToAreaMap != null && _hitboxToAreaMap.TryGetValue(hitboxName, out int areaIndex))
            return areaIndex;

        Debug.LogWarning($"RobotHitReceiver: Hitbox '{hitboxName}' not found in mapping. Make sure it's added to Hitbox Area Mapping in the Inspector.");
        return -1;
    }
}

