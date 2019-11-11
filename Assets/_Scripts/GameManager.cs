using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager;
    public Text healthText;
    public Text ammoText;
    public AudioMixer mixer;
    private GameObject player;
    public GameObject ghost;
    int levelNumber = 0;
    bool paused;
    bool retryRun;
    void Awake()
    {
        if (gameManager == null)
        {
            gameManager = this;
        }
        else
        {
            if (healthText != null)
            {
                print(gameManager.retryRun);
                gameManager.ghost = ghost;
                if (gameManager.retryRun)
                {
                    gameManager.ghost = Instantiate(gameManager.ghost, gameManager.player.transform.position, gameManager.player.transform.rotation);
                    gameManager.ghost.transform.position = gameManager.player.transform.position;
                    gameManager.ghost.transform.rotation = gameManager.player.transform.rotation;
                }
                
                gameManager.healthText = healthText;
                gameManager.ammoText = ammoText;
            }
            else
            {
                gameManager.healthText = null; // splash screen etc.
                gameManager.ammoText = null;
            }
            gameManager.mixer = mixer; // will always be there
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameManager);
    }

    private void Start()
    {
        gameManager.retryRun = false;
        gameManager.paused = true;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void pauseGame()
    {
        Cursor.lockState = CursorLockMode.None;
        gameManager.paused = true;
    }

    public void unPauseGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        gameManager.paused = false;
    }

    public bool isPaused()
    {
        return paused;
    }

    public void killPlayer(GameObject player)
    {
        gameManager.retryRun = true;
        player.SetActive(false);
        pauseGame();
        SceneManager.LoadScene("Death");
    }

    public void updateHealthText(float health)
    {
        //NEED THIS FOR RELOAD
        //gameManager.healthBar.GetComponent<RectTransform>().sizeDelta = new Vector2(health * 2, 50);
        gameManager.healthText.text = health.ToString();
    }

    public void updateAmmoText(int ammo, int ammoCapacity)
    {
        if(ammoCapacity != 0)
        {
            ammoText.text = ammo + "/" + ammoCapacity;
        }
        else
        {
            ammoText.text = "";
        }
    }

    public void enterBuilding(string name)
    {
        gameManager.retryRun = false;

    }

    public void loadLevel(int levelNumber)
    {
        if (levelNumber == 0)
        {
            Application.Quit();
            //UnityEditor.EditorApplication.isPlaying = false;
        }
        else if(SceneManager.GetActiveScene().name == "Loading")
        {
            SceneManager.LoadScene("Level"+levelNumber);
        }
        else
        {
            gameManager.levelNumber = levelNumber;
            if (gameManager.player != null)
            {
                snapshotPlayer();
            }
            SceneManager.LoadScene("Loading");
        }
    }

    public void setGhostTransform(GhostPosContainer ghostpos)
    {
         gameManager.ghost.transform.position = ghostpos.Pos;
         gameManager.ghost.transform.rotation = ghostpos.Rot;
    }

    public void reloadLevel()
    {
        SceneManager.LoadScene("Loading");
    }

    public void loadMainMenu()
    {
        gameManager.levelNumber = 0;
        //gameManager.player.GetComponent<Player>().resetPlayer();
        SceneManager.LoadScene("Splash");
    }

    public void snapshotPlayer()
    {
        if (GameObject.FindGameObjectWithTag("Player"))
        {
            gameManager.player = GameObject.FindGameObjectWithTag("Player");
        }
    }

    public GameObject getPlayer()
    {
        return player;
    }
    public int currentLevel()
    {
        return levelNumber;
    }

    public void changeSFXVolume(int level)
    {
        float currentLevel;
        mixer.GetFloat("SFX", out currentLevel);
        mixer.SetFloat("SFX", level + currentLevel);
    }
}