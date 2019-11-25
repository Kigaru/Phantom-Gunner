using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    private void OnDestroy()
    {
        if(GetComponent<Enemy>().dead)
            GameManager.gameManager.win();
    }
}
