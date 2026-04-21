using UnityEngine;
using System.Collections.Generic;

public class Collision : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";

    private readonly HashSet<Collider> _activeHits = new HashSet<Collider>();

    private void OnTriggerEnter(Collider other)
    {
        TryLogHit(other, "Trigger Enter");
    }

    private void OnTriggerStay(Collider other)
    {
        // XR tracked objects sometimes start already overlapping; stay ensures we still detect them.
        TryLogHit(other, "Trigger Stay");
    }

    private void OnTriggerExit(Collider other)
    {
        _activeHits.Remove(other);
    }

    private void TryLogHit(Collider other, string hitType)
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

        LogHit(hitObject, hitType);
    }

    private void LogHit(GameObject hitObject, string hitType)
    {
        Debug.Log($"{hitType} detected with: {hitObject.name}");
        Debug.Log($"Collided with {playerTag} via {hitType}!");
    }
}
