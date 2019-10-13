using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private GameManager gm;

    private float health;
    private static float maxHealth = 0;
    private static float cappedHealth;
    private GameObject[] inventory;

    
    // Start is called before the first frame update
    void Start()
    {
        gm = GameManager.gameManager;
        if (gm.getPlayer() != null)
        {
            GameObject player = gm.getPlayer();
            player.transform.SetPositionAndRotation(gameObject.transform.position, gameObject.transform.rotation);
            gm.updateHealthText(player.GetComponent<Player>().health);
            if(player.GetComponent<FPSBody>().getGun() != null)
            {
                Weapon playerWeapon = player.GetComponent<FPSBody>().getGun().GetComponent<Weapon>();
                gm.updateAmmoText(playerWeapon.getAmmoInMag(), 30); //TODO need to refactor total ammo when the inventory is finally implemented
            }
            Destroy(gameObject);
        }
        else
        {
            maxHealth = 0;
        }

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

    private void Update()
    {
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
                gm.killPlayer(gameObject);
                break;
            case "Win":
                gm.loadLevel(2);
                break;
            default:
                break;
        }
    }

}
