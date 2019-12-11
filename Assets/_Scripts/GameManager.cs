using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager;
    public Text healthText;
    public Text ammoText;
    public Text scoreText;
    public GameObject reloadBar;
    public GameObject pauseMenu;
    public AudioMixer mixer;
    private GameObject player;
    public GameObject ghost;
    public int levelNumber = 0;
    bool paused;
    bool retryRun, didPlayerHaveGunAtStart;
    bool isPlayerReloading;
    public bool iAmSilverOneMode;
    float reloadProgress;
    void Awake()
    {
        if (gameManager == null)
        {
            gameManager = this;
        }
        else
        {
            gameManager.isPlayerReloading = false;
            gameManager.reloadProgress = 0;
            if (healthText != null)
            {
                reloadBar.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 30);
                reloadBar.transform.parent.gameObject.SetActive(false);
                gameManager.ghost = ghost;
                if (gameManager.retryRun)
                {
                    gameManager.ghost = Instantiate(gameManager.ghost, gameManager.player.transform.position, gameManager.player.transform.rotation);
                    gameManager.ghost.transform.position = -100 * gameManager.player.transform.position;
                    gameManager.ghost.transform.rotation = gameManager.player.transform.rotation;

                    if (!didPlayerHaveGunAtStart)
                    {
                        gameManager.player.GetComponent<FPSBody>().deleteGun();
                        updateAmmoText(-1, -1);
                    }
                    else
                    {
                        Weapon playerWeapon = gameManager.player.GetComponent<FPSBody>().getGun().GetComponent<Weapon>();
                        updateAmmoText(playerWeapon.getAmmoInMag(), playerWeapon.getAmmoInReserve());
                    }
                }
                else
                {
                    if (gameManager.player)
                    {
                        if (gameManager.player.GetComponent<FPSBody>().getGun())
                        {
                            Weapon playerWeapon = gameManager.player.GetComponent<FPSBody>().getGun().GetComponent<Weapon>();
                            updateAmmoText(playerWeapon.getAmmoInMag(), playerWeapon.getAmmoInReserve());
                        }
                        else
                        {
                            updateAmmoText(-1, -1);
                        }
                    }
                    else if(ammoText)
                    {
                        updateAmmoText(-1, -1);
                    }
                }
                
                gameManager.healthText = healthText;
                gameManager.ammoText = ammoText;
                gameManager.scoreText = scoreText;
                gameManager.reloadBar = reloadBar;
                gameManager.pauseMenu = pauseMenu;
            }
            else
            {
                gameManager.healthText = null; // splash screen etc.
                gameManager.ammoText = null;
                gameManager.scoreText = null;
                gameManager.reloadBar = null;
                gameManager.pauseMenu = null;
            }
            gameManager.mixer = mixer; // will always be there
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameManager);
    }

    private void Start()
    {
        snapshotPlayer();
        gameManager.retryRun = false;
        gameManager.paused = true;
        gameManager.isPlayerReloading = false;
        gameManager.reloadProgress = 0;
        gameManager.didPlayerHaveGunAtStart = false;
    }

    // Update is called once per frame
    void Update()
    {

        if(isPlayerReloading)
        {
            reloadProgress += Time.deltaTime;
            gameManager.reloadBar.GetComponent<RectTransform>().sizeDelta = new Vector2(reloadProgress * 200, 30);

            if(gameManager.reloadBar.GetComponent<RectTransform>().sizeDelta.x > 400)
            {
                isPlayerReloading = false;
                gameManager.reloadBar.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 30);
                reloadBar.transform.parent.gameObject.SetActive(false);
                player.GetComponent<FPSBody>().getGun().GetComponent<Weapon>().reload();
            }
        }

    }

    public void pauseGame()
    {
        Cursor.lockState = CursorLockMode.None;
        gameManager.paused = true;
        pauseMenu.SetActive(true);
    }

    public void unPauseGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        gameManager.paused = false;
        pauseMenu.SetActive(false);
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

    public void win()
    {
        player.SetActive(false);
        pauseGame();
        SceneManager.LoadScene("Win");
    }

    public void updateHealthText(float health)
    {
        if (gameManager.healthText != null)
        {
            gameManager.healthText.text = health.ToString();
        }
    }

    public void updateAmmoText(int ammo, int ammoInReserve)
    {
        if (gameManager.ammoText != null)
        {
            if (ammoInReserve != -1)
            {
                ammoText.text = ammo + "/" + ammoInReserve;
            }
            else
            {
                ammoText.text = "";
            }
        }
    }

    public void updateScoreText(int score)
    {
        if (gameManager.scoreText != null)
        {
            gameManager.scoreText.text = score.ToString();
        }
    }

    public void beginWeaponReload()
    {
        reloadBar.transform.parent.gameObject.SetActive(true);
        reloadProgress = 0;
        isPlayerReloading = true;
    }

    public void stopReloading() 
    {
        gameManager.reloadBar.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 30);
        reloadBar.transform.parent.gameObject.SetActive(false);
        reloadProgress = 0;
        isPlayerReloading = false;
    }

    public void enterBuilding(int levelNumber)
    {
        gameManager.retryRun = false;
        loadLevel(levelNumber);

    } 
    
    public void exitBuilding(int levelNumber)
    {
        gameManager.retryRun = false;
        switch (levelNumber)
        {
            case 2:
                loadLevel(1);
                break;
            default:
                break;
        }

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
        retryRun = false;
        gameManager.player.GetComponent<Player>().resetPlayer();
        Destroy(gameManager.player);
        SceneManager.LoadScene("Splash");
    }

    public bool retrying()
    {
        return retryRun;
    }

    public void snapshotPlayer()
    {
        if (GameObject.FindGameObjectWithTag("Player"))
        {
            gameManager.player = GameObject.FindGameObjectWithTag("Player");
            didPlayerHaveGunAtStart = gameManager.player.GetComponent<FPSBody>().getGun() ? true : false;
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

    public bool IsPlayerReloading()
    {
        return isPlayerReloading;
    }

}