using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trialrenderer : MonoBehaviour
{
    private TrailRenderer tr;

    void Start()
    {
        tr = GetComponent<TrailRenderer>();
        tr.material = tr.material;
        tr.startWidth = 1f;
        tr.endWidth = 0.1f;
        
    }
}
