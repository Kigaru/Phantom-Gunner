using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSBody : MonoBehaviour
{
    public KeyCode forwardKey = KeyCode.W;
    public KeyCode backwardKey = KeyCode.S;
    public KeyCode leftKey = KeyCode.A;
    public KeyCode rightKey = KeyCode.D;
    public KeyCode useKey = KeyCode.E;
    public KeyCode dropKey = KeyCode.G;
    public KeyCode reloadKey = KeyCode.R;
    public KeyCode hideWeaponKey = KeyCode.H;
    public KeyCode fireKey = KeyCode.Mouse0;
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    //TO BE DELETED
    public KeyCode recordKey = KeyCode.P;
    public KeyCode playBackKey = KeyCode.O;
    public KeyCode volUpKey = KeyCode.RightBracket;
    public KeyCode volDownKey = KeyCode.LeftBracket;
    
    [SerializeField]
    private float playerSpeed = 2;
    [SerializeField]
    private float jumpSpeed = 200;
    [SerializeField]
    private float sprintFactor = 3;
    [SerializeField]
    private float sensitivity = 2;
    [SerializeField]
    private float pickUpDistance = 2.2f;
    [SerializeField]
    [Tooltip("Vertical view clamp value must be between 0 and 90 inclusive.")]
    private float vClamp = 90; 
    private bool grounded;
    public GameObject weaponPivot; //for correct rotation
    private GameObject gun;
    private GameObject prop;

    private GameObject interactableObject;
    private Rigidbody rb;
    private GameManager gm;
    private Camera eyes;
    Ray rayFromPlayer;
    RaycastHit hit;

    private float forwardMovement, sideMovement;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        gm = GameManager.gameManager;
        eyes = gameObject.GetComponentInChildren<Camera>();

        forwardMovement = 0;
        sideMovement = 0;

        gm.unPauseGame(); //TODO this design decision is being done because I want the game manager to handle the pause UI in the future

    }

    void Update()
    {
        if (!gm.isPaused())
        {
            //global keys
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                gm.pauseGame();
            }

            handleMovement();
            handleView();
            handleWeapons();
            handlePlayerAction();

        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                gm.unPauseGame();
            }
        }

        //TODO TO REFACTOR
        if(Input.GetKeyDown(volDownKey))
        {
            gm.changeSFXVolume(-5);
        }
        if(Input.GetKeyDown(volUpKey))
        {
            gm.changeSFXVolume(5);
        }
        if (Input.GetKeyDown(recordKey))
        {
            GetComponent<Player>().capturePositionRecording();
        }
        if (Input.GetKeyDown(playBackKey))
        {
            gm.setGhostTransform(GetComponent<Player>().getOldestRecordedPosition());
        }

    }

    void handleMovement()
    {
        forwardMovement = 0;
        sideMovement = 0;

        if (Input.GetKey(forwardKey))
        {
            forwardMovement += 1 * playerSpeed * Time.deltaTime;
        }
        if (Input.GetKey(backwardKey))
        {
            forwardMovement += -1 * playerSpeed * Time.deltaTime;
        }
        if (Input.GetKey(leftKey))
        {
            sideMovement += -1 * playerSpeed * Time.deltaTime;
        }
        if (Input.GetKey(rightKey))
        {
            sideMovement += 1 * playerSpeed * Time.deltaTime;
        }
        
        if (Input.GetKeyDown(jumpKey))
        {
            //GameObject a = new GameObject();
            //Instantiate(a, transform.position - new Vector3((GetComponent<Collider>().bounds.size.x / 2) * -transform.forward.x, GetComponent<Collider>().bounds.size.y - 0.4f, (GetComponent<Collider>().bounds.size.z / 2) * -transform.forward.z) , Quaternion.identity);
            //Destroy(a);
            if (Physics.Raycast(transform.position - new Vector3(0, GetComponent<Collider>().bounds.size.y - 0.2f, 0), Vector3.down, 0.3f) || 
                Physics.Raycast(transform.position - new Vector3((GetComponent<Collider>().bounds.size.x / 2) * -transform.forward.x, GetComponent<Collider>().bounds.size.y - 0.4f, (GetComponent<Collider>().bounds.size.z / 2) * -transform.forward.z), Vector3.down, 0.3f))
            {
                rb.AddForce(new Vector3(0, jumpSpeed, 0));
            }
        }


        if (Input.GetKey(sprintKey))
        {
            forwardMovement *= sprintFactor;
            sideMovement *= sprintFactor;
        }

        rb.MovePosition(transform.position + (rb.transform.forward * forwardMovement) + (rb.transform.right * sideMovement));
        
    }

    void handleView()
    {
        rb.transform.RotateAround(rb.transform.position, rb.transform.up, Input.GetAxis("Mouse X") * sensitivity * 25 * Time.deltaTime); //rightleft

        float lookDelta = -Input.GetAxis("Mouse Y") * sensitivity * 25 * Time.deltaTime;
        if (eyes.transform.localEulerAngles.x + lookDelta > 360 - vClamp || eyes.transform.localEulerAngles.x + lookDelta < 0 + vClamp)
        {
            eyes.transform.RotateAround(eyes.transform.position, eyes.transform.right, lookDelta); // updown
        }
        
        if(gm.iAmSilverOneMode)
        {
            if(eyes.transform.localEulerAngles.x < 180)
            {
                eyes.transform.RotateAround(eyes.transform.position, eyes.transform.right, -5 * Time.deltaTime);
            }
        }
        rb.transform.rotation = Quaternion.Euler(rb.transform.eulerAngles.x, rb.transform.eulerAngles.y, 0);
        rayFromPlayer = eyes.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));


    }

    void handleWeapons()
    {
        if (gun != null)
        {
            if (Input.GetKeyDown(fireKey))
            {
                if (gun.gameObject.activeInHierarchy && !gm.IsPlayerReloading()) 
                {
                    if (gun.GetComponent<Weapon>().hasAmmo()) {
                        gun.GetComponent<Weapon>().fire();
                        if (Physics.Raycast(rayFromPlayer, out hit, gun.GetComponent<Weapon>().getRange()))
                        {
                            Instantiate(gun.GetComponent<Weapon>().getBulletParticles(), hit.point, Quaternion.LookRotation(hit.normal));

                            if (Physics.Raycast(rayFromPlayer, out hit, gun.GetComponent<Weapon>().getRange(), LayerMask.GetMask("Enemy")))
                            {
                                //TODO COMMENTED SECTION IS FOR CHECKING WALLS, ALMOST DONE, JUST NEED TO MATCH EACH POINT OF ENTRY/EXIT OF EACH WALL.

                                //Ray rayFromEnemy = new Ray(hit.point, -rayFromPlayer.direction); //checking for walls

                                //if (Physics.Raycast(rayFromPlayer, hit.distance, LayerMask.GetMask("Wall")))
                                //{
                                //    RaycastHit[] hitWallsFromPlayer = Physics.RaycastAll(rayFromPlayer, hit.distance, LayerMask.GetMask("Wall"));
                                //    RaycastHit[] hitWallsFromEnemy = Physics.RaycastAll(rayFromEnemy, hit.distance, LayerMask.GetMask("Wall"));
                                //    //TODO put those two above in order

                                //    float totalLength = 0;
                                //    for(int i = 0; i < hitWallsFromPlayer.Length; i++)
                                //    {
                                //        totalLength += Vector3.Distance(hitWallsFromPlayer[i].point, hitWallsFromEnemy[i].point);
                                //    }
                                //    print("total distance of the walls: " + totalLength);
                                //}
                                //else
                                //{
                                //}

                                hit.collider.gameObject.GetComponent<Enemy>().hurt(gun.GetComponent<Weapon>().getDamage());
                            }
                        }
                    }
                }
                else
                {
                    gun.gameObject.SetActive(true);
                }
            }

            if (Input.GetKeyDown(hideWeaponKey))
            {
                if(gun.gameObject.activeInHierarchy)
                {
                    gm.stopReloading();
                    gun.gameObject.SetActive(false);
                }
                else
                {
                    gun.gameObject.SetActive(true);
                }
            }
        }
    }

    void handlePlayerAction()
    {
        Physics.Raycast(rayFromPlayer, out hit, pickUpDistance);
        if (hit.collider != null)
        {
            // Multiple checks for guns & buttons in a single line
            if (hit.collider.gameObject.tag == "Gun" ||
                (hit.collider.gameObject.tag == "Movable" && prop == null) || 
                hit.collider.gameObject.tag == "Ammo") 
            {
                interactableObject = hit.collider.gameObject;
            }
        }
        else
        {
            interactableObject = null;
        }

        if (Input.GetKeyDown(useKey))
        {
            if (interactableObject != null)
            {
                switch (interactableObject.tag)
                {
                    case "Gun":
                        if (gun == null)
                        {
                            //ADD GUN TO INVENTORY AND SELECT IT AS CURRENTLY ACTIVE GUN
                            gun = interactableObject;
                            interactableObject = null;
                            gun.GetComponent<Collider>().enabled = false;
                            gun.GetComponent<Rigidbody>().isKinematic = true;
                            gun.transform.SetParent(weaponPivot.transform);
                            gun.transform.localPosition = Vector3.zero;
                            gun.transform.localEulerAngles = Vector3.zero;
                            gm.updateAmmoText(gun.GetComponent<Weapon>().getAmmoInMag(), gun.GetComponent<Weapon>().getAmmoInReserve());
                        }
                        else
                        {
                            //INVENTORY STUFF AAAA
                        }
                        break;
                    case "Movable": 
                        if(prop == null)
                        {
                            prop = interactableObject;
                            interactableObject = null;
                            prop.GetComponent<Rigidbody>().useGravity = false;
                            prop.transform.SetParent(eyes.transform);
                        }
                        break;
                    case "Ammo":
                        if(gun!=null)
                        {
                            Weapon weapon = gun.GetComponent<Weapon>();
                            if (weapon.getAmmoInReserve() != 999)
                            {
                                weapon.increaseAmmoInReserve(weapon.getAmmoCapacity());
                                Destroy(interactableObject);
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            else if(prop != null)
            {
                prop.transform.SetParent(null);
                prop.GetComponent<Rigidbody>().useGravity = true;
                prop = null;
            }
        }
        if(Input.GetKey(dropKey))
        {
            if(gun != null && gun.activeSelf)
            {
                gun.transform.SetParent(null);
                gun.GetComponent<Collider>().enabled = true;
                gun.GetComponent<Rigidbody>().isKinematic = false;
                gun = null;
                gm.updateAmmoText(-1, -1);
                gm.stopReloading();
            }
        }
        if (Input.GetKey(reloadKey))
        {
            if (gun != null && gun.activeSelf)
            {
                Weapon weapon = gun.GetComponent<Weapon>();
                if (weapon.getAmmoInMag() != weapon.getAmmoCapacity() && weapon.getAmmoInReserve() != 0 && !gm.IsPlayerReloading())
                {
                    gm.beginWeaponReload();
                }

            }
        }

        if(prop != null && Vector3.Distance(rayFromPlayer.origin,hit.point) > pickUpDistance) //check if the held item is too far away from the player
        {
            prop.transform.SetParent(null);
            prop.GetComponent<Rigidbody>().useGravity = true;
            prop = null;
        }
    }

    public GameObject getGun()
    {
        return gun;
    }

    public void deleteGun()
    {
        Destroy(gun);
    }
}
