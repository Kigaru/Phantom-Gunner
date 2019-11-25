using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyTemplate;
    private IEnumerator coroutine;

    private IEnumerator spawnEveryGivenSeconds()
    {
        while (true)
        {
            yield return new WaitForSeconds(1); //GRACE PERIOD, BECAUSE GAME DOESN'T LIKE THINGS LOADING TOO FAST
            if (Enemy.enemyCount < 5)
            {
                Instantiate(enemyTemplate, gameObject.transform.position, Quaternion.identity, gameObject.transform);
            }
            yield return new WaitForSeconds(100 / GameManager.gameManager.getPlayer().GetComponent<Player>().getHealth()); // 
        }
    }

    private void Awake()
    {
        Enemy.enemyCount = 0;
        coroutine = spawnEveryGivenSeconds();
        StartCoroutine(coroutine);
    }
}


