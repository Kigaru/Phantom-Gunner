using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletParticle : MonoBehaviour
{
    public float TTL;

    void Update()
    {
        if(TTL < 0)
        {
            Destroy(gameObject);
        }
        else
        {
            TTL -= Time.deltaTime;
        }
    }
}
