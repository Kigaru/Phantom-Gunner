using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseHandler : MonoBehaviour
{

    public void ContinueGame()
    {
        GameManager.gameManager.unPauseGame();
    }

    public void ClickMainMenu()
    {
        GameManager.gameManager.loadMainMenu();
    }
}
