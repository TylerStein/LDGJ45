using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Sound_Controller : MonoBehaviour
{

    public AudioClip Attack, Die;
    AudioSource audioSource;
    
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayAttack() {
        audioSource.PlayOneShot(Attack);
    }

    public void PlayDie() {
        audioSource.PlayOneShot(Die);
    }
}
