using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private GameManager gm;
    private int score;
    private float health;
    private static float maxHealth = 0;
    private static float cappedHealth;
    private List<GhostPosContainer> deathPositions;
    private List<GhostPosContainer> alivePositions;
    private GameObject[] inventory;


    private void Awake()
    {
        gm = GameManager.gameManager;
        if (gm.getPlayer() != null)
        {
            GameObject player = gm.getPlayer();
            player.GetComponent<Player>().deathPositions = player.GetComponent<Player>().alivePositions;
            player.GetComponent<Player>().alivePositions = new List<GhostPosContainer>();
            player.SetActive(true);
            if (gm.retrying())
            {
                player.transform.SetPositionAndRotation(gameObject.transform.position, gameObject.transform.rotation);
            }
            if (player.GetComponent<Player>().health <= 0)
            {
                player.GetComponent<Player>().health = cappedHealth;
            }
            gm.updateHealthText(player.GetComponent<Player>().health);
            gm.updateScoreText(player.GetComponent<Player>().score);
            if (player.GetComponent<FPSBody>().getGun() && !gm.retrying())
            {
                gm.updateAmmoText(player.GetComponent<FPSBody>().getGun().GetComponent<Weapon>().getAmmoInMag(), player.GetComponent<FPSBody>().getGun().GetComponent<Weapon>().getAmmoInReserve());
            }
            else
            {
                gm.updateAmmoText(-1, -1);
            }
            Destroy(gameObject);
        }
        else
        {
            gm.snapshotPlayer();
            alivePositions = new List<GhostPosContainer>();
            maxHealth = 0;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        score = 0;
        if (maxHealth == 0)
        {
            maxHealth = 100;
            cappedHealth = maxHealth;
        }

        health = cappedHealth;
        gm.updateHealthText(health);
        gm.updateAmmoText(-1, -1);
        gm.updateScoreText(score);
        inventory = new GameObject[10];
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
    }

    public void capturePositionRecording()
    {

        GhostPosContainer item = new GhostPosContainer(transform.position, transform.rotation);
        alivePositions.Add(item);
    }

    public GhostPosContainer getOldestRecordedPosition()
    {
        if (deathPositions != null)
        {
            if (deathPositions.Count != 0)
            {
                GhostPosContainer toReturn = deathPositions[0];
                deathPositions.RemoveAt(0);
                return toReturn;
            }
        }
        return null;
    }


    public void resetPlayer()
    {
        maxHealth = 0;
    }

    public void hurt(float hp)
    {
        health -= hp;
        if(health<=0)
        {
            gm.killPlayer(gameObject);
            cappedHealth = (int) (cappedHealth/2);
            if (cappedHealth < 1)
            {
                cappedHealth = 1;
            }
        }
        gm.updateHealthText(health);
    }
    public void heal(float amnt)
    {
        if (health + amnt > cappedHealth)
        {
            health = cappedHealth;
        }
        else health += amnt;
        gm.updateHealthText(health);
    }
    public void fixHP(float amnt)
    {
        if (cappedHealth + amnt > maxHealth)
        {
            cappedHealth = maxHealth;
        }
        else cappedHealth += amnt;
    }

    public void addScore(int amnt)
    {
        score += amnt;
        gm.updateScoreText(score);
    }

    public int getScore()
    {
        return score;
    }
    public float getHealth()
    {
        return health;
    }

    private void OnTriggerEnter(Collider col)
    {
        switch(col.tag)
        {
            case "Explosion":
                Instantiate(col.GetComponent<Explosion>().getExplosion(),col.gameObject.transform.position,col.gameObject.transform.rotation);
                hurt(col.GetComponent<Explosion>().getDamage());
                break;
            case "KillPlane":
                hurt(health);
                break;
            case "BuildingEnter":
                gm.enterBuilding(col.gameObject.GetComponent<Building>().LevelID);
                break;
            case "BuildingExit":
                gm.exitBuilding(col.gameObject.GetComponent<Building>().LevelID);
                break;
            default:
                break;
        }
    }

}
