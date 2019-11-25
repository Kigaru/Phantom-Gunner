using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField]
    private string weaponName;
    [SerializeField]
    private float range;
    [SerializeField]
    private float damage;
    [SerializeField]
    private int magazineSize;
    [SerializeField]
    private int ammoInMag;
    [SerializeField]
    private int ammoInReserve;
    [SerializeField]
    private GameObject bulletParticles;

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

    public void reload()
    {
        if(ammoInReserve <= magazineSize-ammoInMag)
        {
            ammoInMag += ammoInReserve;
            ammoInReserve = 0;
        }
        else
        {
            ammoInReserve -= magazineSize-ammoInMag;
            ammoInMag = magazineSize;

        }
        GameManager.gameManager.updateAmmoText(ammoInMag, ammoInReserve);
    }

    public int getAmmoInReserve()
    {
        return ammoInReserve;
    }

    public void increaseAmmoInReserve(int amnt)
    {
        ammoInReserve += amnt;
        if (ammoInReserve > 999) ammoInReserve = 999;
        GameManager.gameManager.updateAmmoText(ammoInMag, ammoInReserve);
    }

    public void fire()
    {
        ammoInMag--;
        GameManager.gameManager.updateAmmoText(ammoInMag, ammoInReserve);

    }
}
