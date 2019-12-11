using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SplashButtonController : MonoBehaviour
{

    GameManager gm;
    [SerializeField] private Button SilverButton;

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

    public void toggleSilverMode()
    {
        gm.iAmSilverOneMode = !gm.iAmSilverOneMode;

        if(gm.iAmSilverOneMode)
        {
            SilverButton.image.color = Color.green;
        }
        else
        {
            SilverButton.image.color = Color.red;

        }
    }
}
