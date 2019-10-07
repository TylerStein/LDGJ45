using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteVFXController : MonoBehaviour
{
    [SerializeField] public GameObject swipeVFX;

    public void SpawnSwipeVFX(Vector3 worldPosition, Vector3 rotation, Color spriteColor) {
        GameObject vfx = Instantiate(swipeVFX, worldPosition, Quaternion.Euler(rotation));
        vfx.GetComponent<SpriteRenderer>().color = spriteColor;
        //Debug.Log("Created VFX", vfx);
    }
}
