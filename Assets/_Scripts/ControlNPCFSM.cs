using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ControlNPCFSM : MonoBehaviour
{

    public enum NPC_TYPE
    {
        EASY = 0,
        NORMAL = 1,
        HARD = 2,
        SUPER_HARD = 3
    }


    public NPC_TYPE myType;

    Animator anim;
    AnimatorStateInfo info;
    bool canSeePlayer, withinAttackRange;
    Ray rayFromNPC;
    RaycastHit hit;
    public GameObject NPCHead;
    public float NPCFOV = 50;
    GameObject[] waypoints;
    int currentWaypoint;
    float lastSeenPlayer;
    public float npcSpeed = 0.66f;


    // Start is called before the first frame update
    void Start()
    {
        lastSeenPlayer = 0;
        currentWaypoint = 0;
        waypoints = GameObject.FindGameObjectsWithTag("Waypoint");
        canSeePlayer = false;
        anim = GetComponent<Animator>();
        GetComponent<NavMeshAgent>().isStopped = false;

        switch(myType)
        {
            case NPC_TYPE.EASY:
                GetComponent<NavMeshAgent>().speed = npcSpeed;
                break;
            case NPC_TYPE.NORMAL:
                GetComponent<NavMeshAgent>().speed = npcSpeed * 2;
                break;
            case NPC_TYPE.HARD:
                GetComponent<NavMeshAgent>().speed = npcSpeed * 4;
                break;
            case NPC_TYPE.SUPER_HARD:
                GetComponent<NavMeshAgent>().speed = npcSpeed * 8;
                break;
            default:
                GetComponent<NavMeshAgent>().speed = npcSpeed;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(rayFromNPC.origin, rayFromNPC.direction * NPCFOV, Color.blue);
        
        handleVision();
        handlePatrol();
        if (GameManager.gameManager.getPlayer().activeSelf)
        {
            handleFollow();
            handleAttack();
        }
    }

    void handleVision()
    {
        rayFromNPC = new Ray(NPCHead.transform.position, NPCHead.transform.forward);
        info = anim.GetCurrentAnimatorStateInfo(0);
        
        if (Physics.Raycast(rayFromNPC, out hit, NPCFOV, LayerMask.GetMask("Player")))
        {
            RaycastHit wallHit;
            bool wallInFrontOfPlayer = false;
            if (Physics.Raycast(rayFromNPC, out wallHit, NPCFOV, LayerMask.GetMask("Wall")))
            {
                if (Vector3.Distance(NPCHead.transform.position, wallHit.point) < Vector3.Distance(NPCHead.transform.position, hit.point))
                {
                    wallInFrontOfPlayer = true;
                }
            }

            if (!wallInFrontOfPlayer)
            {
                lastSeenPlayer = Time.fixedTime;
                anim.SetBool("canSeePlayer", true);
                GetComponent<NavMeshAgent>().destination = GameObject.Find("FPSBody").transform.position;
            }
        }
    }

    void handlePatrol()
    {
        if (info.IsName("Patrol"))
        {
            GetComponent<NavMeshAgent>().destination = waypoints[currentWaypoint].transform.position;

            if (Vector3.Distance(waypoints[currentWaypoint].transform.position, NPCHead.transform.position) < 2)
            {
                if (currentWaypoint < waypoints.Length - 1)
                {
                    currentWaypoint++;
                }
                else
                {
                    currentWaypoint = 0;
                }
            }
        }
    }


    void handleFollow()
    {
        if (info.IsName("Follow_Player"))
        {
            GetComponent<NavMeshAgent>().destination = GameObject.Find("FPSBody").transform.position;

            if (Vector3.Distance(GameObject.Find("FPSBody").transform.position, NPCHead.transform.position) < 2)
            {
                anim.SetBool("withinAttackRange", true);
                print("aaa");
            }

            else if (Time.fixedTime - lastSeenPlayer > 10)
            {
                anim.SetBool("canSeePlayer", false);
            }
        }
    }

    void handleAttack()
    {
        if (info.IsName("Attack_Player"))
        {
            if (Vector3.Distance(GameObject.Find("FPSBody").transform.position, transform.position) > 3)
            {
                anim.SetBool("withinAttackRange", false);
            }
        }
    }

    public int getDifficulty()
    {
        return (int)myType;
    }
}
