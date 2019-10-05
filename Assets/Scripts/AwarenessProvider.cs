using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AwarenessProvider : MonoBehaviour
{
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private RaycastHit2D[] _raycastHits = new RaycastHit2D[4];

    public PlayerController Player { get { return _playerController; } }

    public void Awake() {
        if (!_playerController) _playerController = FindObjectOfType<PlayerController>();
    }

    public int GetHorizontalDirectionToPlayer() {
        return (int)Mathf.Sign(GetHorizontalDistanceToPlayer());
    }

    public float GetHorizontalDistanceToPlayer() {
        return (_playerController.transform.position - transform.position).x;
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
