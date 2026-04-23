using UnityEngine;
using System.Collections.Generic;

public class Collision : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";

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

        LogHit(hitObject);
    }

    private void LogHit(GameObject hitObject)
    {
        string sourceObjectName = gameObject.name;
        Debug.Log($"{sourceObjectName} collided with {hitObject.name}!");
    }
}
