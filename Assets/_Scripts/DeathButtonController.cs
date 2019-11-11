using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathButtonController : MonoBehaviour
{
    

    public void mainMenu()
    {
        GameManager.gameManager.loadMainMenu();
    }

    public void retry()
    {
        GameManager.gameManager.reloadLevel();
    }
}
