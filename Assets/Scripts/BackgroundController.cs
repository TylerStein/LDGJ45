using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    [SerializeField] public float ScrollX = 0.0f;
    [SerializeField] public float ScrollY = 0.0f;
    private SpriteRenderer rend;
    private Vector2 offset;
    private float glowValue;
    private Vector4 glowColour;

    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        glowColour.x = 255.0f;
        glowColour.y = 255.0f;
        glowColour.z = 255.0f;
    }

    // Update is called once per frame
    void Update()
    {
        //Glow
        //glowColour.w = Mathf.Cos(Time.time);
        //rend.material.SetColor(Shader.PropertyToID("_EmissionColor"), glowColour);

        //Scrolling
        offset.x = Time.time * ScrollX;
        offset.y = Time.time * ScrollY;
        rend.material.mainTextureOffset = offset; 
    }
}
