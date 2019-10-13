using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public string weaponName;
    public float range;
    public float damage;
    public int magazineSize;
    public int ammoInMag;
    public GameObject bulletParticles;

    public string getWeaponName()
    {
        return weaponName;
    }
    public float getRange()
    {
        return range;
    }
    public float getDamage()
    {
        return damage;
    }

    public GameObject getBulletParticles()
    {
        return bulletParticles;
    }

    public bool hasAmmo()
    {
        return ammoInMag > 0; 
    }

    public int getAmmoInMag()
    {
        return ammoInMag;
    }

    public int getAmmoCapacity()
    {
        return magazineSize;
    }

    public void fire()
    {
        ammoInMag--;
        GameManager.gameManager.updateAmmoText(ammoInMag, magazineSize);

    }
}
