using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteVisualEffect : MonoBehaviour
{
    [SerializeField] public Animator animator;
    [SerializeField] public bool autoDestroy = true;
    [SerializeField] public float destroyAfterSeconds = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        if (!animator) animator = GetComponent<Animator>();
        PlayAnimation();
        Destroy(gameObject, destroyAfterSeconds);
    }

    void PlayAnimation() {
        animator.SetTrigger("play");
    }
}
