using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Sound_controller : MonoBehaviour
{
    public AudioClip jump, punch, shoot, takeDamage, die;
    AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayJump() {
        audioSource.PlayOneShot(jump);
    }

    public void PlayPunch() {
        audioSource.PlayOneShot(punch);
    }

    public void PlayShoot() {
        audioSource.PlayOneShot(shoot);
    }

    public void PlayTakeDamage() {
        audioSource.PlayOneShot(takeDamage);
    }

    public void PlayDie() {
        audioSource.PlayOneShot(die);
    }
}
