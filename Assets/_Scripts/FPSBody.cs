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
    public KeyCode debugKey = KeyCode.P;
    public KeyCode volUpKey = KeyCode.RightBracket;
    public KeyCode volDownKey = KeyCode.LeftBracket;
    

    public float playerSpeed = 10;
    public float jumpSpeed = 350;
    public float sprintFactor = 3;
    public float sensitivity = 2;
    public float pickUpDistance = 2.2f;
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
        if (gun == null)
        {
            gm.updateAmmoText(0,0);
        }
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
            if (Input.GetKey(KeyCode.Escape))
            {
                gm.pauseGame();
            }

            handleMovement();
            handleView();
            handleWeapons();
            handlePlayerAction();

        }

        //TODO TO REFACTOR
        if(Input.GetKey(fireKey))
        {
            gm.unPauseGame();
        }
        if(Input.GetKeyDown(volDownKey))
        {
            gm.changeSFXVolume(-5);
        }
        if(Input.GetKeyDown(volUpKey))
        {
            gm.changeSFXVolume(5);
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
            rb.AddForce(new Vector3(0, jumpSpeed, 0));
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
        rb.transform.RotateAround(rb.transform.position, rb.transform.up, Input.GetAxis("Mouse X") * sensitivity * 25 * Time.deltaTime);
        eyes.transform.RotateAround(eyes.transform.position, eyes.transform.right, -Input.GetAxis("Mouse Y") * sensitivity * 25 * Time.deltaTime);
        rb.transform.rotation = Quaternion.Euler(rb.transform.eulerAngles.x, rb.transform.eulerAngles.y, 0);
        rayFromPlayer = eyes.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

    }

    void handleWeapons()
    {
        if (gun != null)
        {
            if (Input.GetKeyDown(fireKey))
            {
                if (gun.gameObject.activeInHierarchy)
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
                (hit.collider.gameObject.tag == "Movable" && prop == null)) 
            {
                interactableObject = hit.collider.gameObject;
            }
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
                            gm.updateAmmoText(gun.GetComponent<Weapon>().getAmmoInMag(), gun.GetComponent<Weapon>().getAmmoCapacity());
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
                gm.updateAmmoText(0, 0);
            }
        }
        if(prop != null && Vector3.Distance(rayFromPlayer.origin,hit.point) > pickUpDistance)
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
}
