using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private GameManager gm;

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
            player.transform.SetPositionAndRotation(gameObject.transform.position, gameObject.transform.rotation);
            if(player.GetComponent<Player>().health <= 0)
            {
                player.GetComponent<Player>().health = cappedHealth;
            }
            gm.updateHealthText(player.GetComponent<Player>().health);
            if (player.GetComponent<FPSBody>().getGun() != null)
            {
                Weapon playerWeapon = player.GetComponent<FPSBody>().getGun().GetComponent<Weapon>();
                gm.updateAmmoText(playerWeapon.getAmmoInMag(), 30); //TODO need to refactor total ammo when the inventory is finally implemented
            }
            else
            {
                gm.updateAmmoText(0, 0);
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
        if (maxHealth == 0)
        {
            maxHealth = 100;
            cappedHealth = maxHealth;
        }

        health = cappedHealth;
        gm.updateHealthText(health);
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
            case "Win":
                gm.loadLevel(2);
                break;
            default:
                break;
        }
    }

}
