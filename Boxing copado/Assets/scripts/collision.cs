using UnityEngine;
using System.Collections.Generic;

public class Collision : MonoBehaviour
{
    [SerializeField] private string playerTag = "Hand";

    private readonly HashSet<Collider> _activeHits = new HashSet<Collider>();

    private void OnTriggerEnter(Collider other)
    {
        TryLogHit(other);
    }

    private void OnTriggerStay(Collider other)
    {
        // XR tracked objects sometimes start already overlapping; stay ensures we still detect them.
        TryLogHit(other);
    }

    private void OnTriggerExit(Collider other)
    {
        _activeHits.Remove(other);
    }

    private void TryLogHit(Collider other)
    {
        GameObject hitObject = other.gameObject;

        if (!hitObject.CompareTag(playerTag))
        {
            return;
        }

        if (!_activeHits.Add(other))
        {
            return;
        }

        // compute a reasonable contact point on the collider
        Vector3 contactPoint = other.ClosestPoint(transform.position);

        // estimate intensity from the hitter's Rigidbody velocity if available
        float intensity = 1f;
        var hitterRb = other.attachedRigidbody;
        if (hitterRb != null)
        {
            // scale velocity magnitude to a 0..1 intensity. Tweak divisor to your feel (2f = strong at 2 m/s)
            intensity = Mathf.Clamp01(hitterRb.linearVelocity.magnitude / 2f);
        }

        // attempt to forward the hit to a RobotHitReceiver located on this hitbox or a parent
        var receiver = GetComponentInParent<RobotHitReceiver>();
        if (receiver != null)
        {
            receiver.OnHit(hitObject, contactPoint, transform, intensity);
            return;
        }

        // fallback: just log if no receiver is present
        Debug.Log($"{gameObject.name} collided with {hitObject.name}!");
    }
}
