﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxWater : MonoBehaviour
{
    private float length, startpos;
    public GameObject cam;
    public float parallaxEffect;

    // Start is called before the first frame update
    void Start()
    {
        startpos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
        parallaxEffect = 1 - parallaxEffect; //inverting the effect (not necessary, just invert the effect value in each sprite).
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float temp = cam.transform.position.x * parallaxEffect + Time.realtimeSinceStartup*parallaxEffect;
        float dist = cam.transform.position.x * (1-parallaxEffect);

        transform.position = new Vector3(startpos + dist - Time.realtimeSinceStartup*parallaxEffect, transform.position.y, transform.position.z);

        if (temp > startpos + length) startpos += length;
        else if (temp < startpos - length) startpos -= length;
    }
}
