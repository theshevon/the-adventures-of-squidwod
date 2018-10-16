using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySystem : MonoBehaviour
{
    private ParticleSystem ps;
    // Use this for initialization
    void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }
    
    void Update () {
        if(ps)
        {
            if(!ps.IsAlive())
            {
                Destroy(gameObject);
            }
        }
    }
}
