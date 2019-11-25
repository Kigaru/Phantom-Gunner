using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun : MonoBehaviour
{    
    void Update()
    {
        transform.RotateAround(transform.position, transform.right, 0.1f * Time.deltaTime);
    }
}
