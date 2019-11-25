using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float health = 100;
    public float attack = 25;
    public GameObject drop;
    public static int enemyCount;

    private void Awake()
    {
        enemyCount++;
    }
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
            Player player = GameManager.gameManager.getPlayer().GetComponent<Player>();
            if (drop != null)
            {
                Instantiate(drop, gameObject.transform.position, gameObject.transform.rotation);
                player.addScore((GetComponent<ControlNPCFSM>().getDifficulty() + 1) * 2);
            }
            player.fixHP(10);
            player.heal(5);
            enemyCount--;
            Destroy(gameObject);
        }
    }
}
