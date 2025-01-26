using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadBob : MonoBehaviour
{
    [Range(0.01f, 0.1f)] public float Amount = 0.002f;
    [Range(1f, 30f)] public float Frequency = 10.0f;
    [Range(10f, 100f)] public float Smooth = 10.0f;
    
    [Range(0.01f, 0.1f)] public float idleAmount = 0.002f;
    [Range(1f, 30f)] public float idleFrequency = 10.0f;
    [Range(10f, 100f)] public float idleSmooth = 10.0f;
    
    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.localPosition;
    }

    private void Update()
    {
        CheckForHeadbobTrigger();
        StopHeadBob();
    }

    void CheckForHeadbobTrigger()
    {
        float inputMagnitude = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).magnitude;

        if ((inputMagnitude > 0f))
        {
            StartHeadBob();
        }
        else if (inputMagnitude == 0f)
        {
            IdleHeadBob();
        }
    }

    Vector3 StartHeadBob()
    {
        Vector3 pos = Vector3.zero;
        
        pos.y += Mathf.Lerp(pos.y, Mathf.Sin(Time.time * Frequency) * Amount * 1.4f, Smooth * Time.deltaTime);
        pos.x += Mathf.Lerp(pos.x, Mathf.Sin(Time.time * Frequency / 2f) * Amount * 1.6f, Smooth * Time.deltaTime);
        transform.localPosition += pos;

        return pos;
    }
    
    Vector3 IdleHeadBob()
    {
        Vector3 pos = Vector3.zero;
        
        pos.y += Mathf.Lerp(pos.y, Mathf.Sin(Time.time * idleFrequency) * idleAmount * 1.4f, idleSmooth * Time.deltaTime);
        pos.x += Mathf.Lerp(pos.x, Mathf.Sin(Time.time * idleFrequency / 2f) * idleAmount * 1.6f, idleSmooth * Time.deltaTime);
        transform.localPosition += pos;

        return pos;
    }

    void StopHeadBob()
    {
        if (transform.localPosition == startPos) return;
        transform.localPosition = Vector3.Lerp(transform.localPosition, startPos, Smooth * Time.deltaTime);
    }
    
}
