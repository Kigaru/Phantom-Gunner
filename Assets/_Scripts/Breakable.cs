using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour
{
    public float health = 100;

    public void hurt(float damage)
    {
        if (health > damage)
        {
            health -= damage;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
