using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Partical_System_script : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<ParticleSystem>().GetComponent<Renderer>().material.renderQueue = 4000;
        Debug.Log(GetComponent<ParticleSystem>());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
