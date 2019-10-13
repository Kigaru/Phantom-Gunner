using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float health = 100;
    public float attack = 25;
    public GameObject drop;

    public float getHealth()
    {
        return health;
    }

    public void hurt(float damage)
    {
        if(health > damage)
        {
            health -= damage;
        }
        else
        {
            if(drop != null)
            {
                Instantiate(drop, gameObject.transform.position, gameObject.transform.rotation);
            }
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<Player>().hurt(attack);
        }   
    }
}
