using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AwarenessProvider : MonoBehaviour
{
    [SerializeField] public Transform playerTransform;
    [SerializeField] private RaycastHit2D[] _raycastHits = new RaycastHit2D[4];

    public void Start() {
        if (!playerTransform) playerTransform = FindObjectOfType<PlayerController>().transform;
    }

    public int GetHorizontalDirectionToPlayer() {
        return (int)Mathf.Sign(GetHorizontalDistanceToPlayer());
    }

    public float GetHorizontalDistanceToPlayer() {
        return (playerTransform.position - transform.position).x;
    }

    public bool HorizontalDistanceToNearestCollider(out float distance, float direction, float radius = 0.99f, int layerMask = 1 << 0, float maxDistance = 100f) {
        int hits = Physics2D.RaycastNonAlloc(transform.position, Vector2.right * direction, _raycastHits, maxDistance, layerMask);
        if (hits == 0) {
            distance = 0;
            return false;
        } else {
            distance = _raycastHits[0].distance;
            return true;
        }
    }
}
