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

    // Update is called once per frame
    void Update()
    {

    }
}
