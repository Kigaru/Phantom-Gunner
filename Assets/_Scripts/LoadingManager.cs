using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameManager.gameManager.loadLevel(GameManager.gameManager.currentLevel());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
