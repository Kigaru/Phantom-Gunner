using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashButtonController : MonoBehaviour
{

    GameManager gm;


    private void Start()
    {
        gm = GameManager.gameManager;
    }
    public void startGame()
    {
        gm.loadLevel(1);
    }

    public void exitGame()
    {
        gm.loadLevel(0);
    }
}
