using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float damage = 50;
    public GameObject explosionParticle;

    public float getDamage()
    {
        return damage;
    }

    public GameObject getExplosion()
    {
        return explosionParticle;
    }

}
