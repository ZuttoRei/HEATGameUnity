using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class AI_Orby : MonoBehaviour {

    //Waypoints to visit
    public List<Vector3> Waypoints;
    Transform player;
    Animator anim;
    CharControls playerScript;
    PolyNavAgent agent;
    Hibernation hibernationScript;
    VisionScan scanner;
    FootprintTracker FootTracker;
    public float RotationSpeed;
    public float ZapDuration;
    bool KillingSequenceStarted = false;

    public int ID;

    //List to encapsulate certain variables across other enemies
    AI_SharedVariables AIVars;

    //Apprehension variables
    public float NormalSpeed;
    public float ApprehensionSpeed;

    //Bool to keep track whether we're subscribed to the OnDestinationReached event
    private bool _Patrolling;
    public bool Patrolling
    {
        get { return _Patrolling; }
        set 
        {
            _Patrolling = value; 
            if(value)
            {
                agent.OnDestinationReached += OnDestinationReached;
            }
            else
            {
                agent.OnDestinationReached -= OnDestinationReached;
            }
        }
    }
    

    public bool IsHibernating
    {
        get { return hibernationScript.IsHibernating; }
    }

    //The waypoint the AI is currently traveling to
    int currentWayPoint = 0;


	// Use this for initialization
	void Start () {
        //Instantiate pathfinding agent
        agent = GetComponent<PolyNavAgent>();
        Patrolling = true;
        //To reference players' variables
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerScript = player.GetComponent<CharControls>();
        //Get the child scanner of this object
        scanner = GetComponentInChildren<VisionScan>();
        hibernationScript = GetComponent<Hibernation>();
        anim = GetComponent<Animator>();
        FootTracker = GetComponent<FootprintTracker>();
        AIVars = GetComponent<AI_SharedVariables>();
        ID = GetInstanceID();

        //Start patrolling
        NavigateNextWayPoint();
	}

    float sightLevel;

	// Update is called once per frame
    void FixedUpdate()
    {
        if (IsHibernating)
            return;

        sightLevel = scanner.GetSightLevel(false);
        agent.maxSpeed = NormalSpeed;
        
        //Observe player as soon as they're in line of sight
        if (sightLevel >= scanner.VisionTreshold)
        {
            anim.SetBool("Focused", true);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, playerScript.PlayerDirection(transform.position), RotationSpeed);
        }
        else
        {
            //If focused, set to false
            if (anim.GetBool("Focused") == true)
                anim.SetBool("Focused", false);
        }

        DecideActions();
        
    }
    
    void DecideActions()
    {
        //And this bot is assigned to trace the footprints
        bool canTrack = (playerScript.CurrentFootPrintTracer == ID || playerScript.CurrentFootPrintTracer == 0);

        //If foot prints are detected, follow them. (If player is not dead and we are not AIVars.Apprehending) 
        if (scanner.HasDetectedFootPrints && !playerScript.Dead && !AIVars.Apprehending && canTrack)
        {
            //Stop patrolling
            if (Patrolling)
            {
                anim.SetBool("Investigating", true);
                Patrolling = false;

                //Take task as primary foot print tracker
                playerScript.CurrentFootPrintTracer = ID;
            }

            FootTracker.TrackFootPrints();
        }
        else if (AIVars.Apprehending && !playerScript.Dead)
        {
            agent.SetDestination(player.position);
            agent.maxSpeed = ApprehensionSpeed;
            agent.rotateSpeed = agent.rotateSpeed * 2;

            //If he focuses the player, zap right away. no questions asked
            if (anim.GetBool("Focused"))
            {
                StartCoroutine(BeamPlayer());
            }

        }
        else //Otherwise continue pathing as normal
        {
            AIVars.Apprehending = false;
            agent.maxSpeed = NormalSpeed;
            //Continue patrolling
            if (!Patrolling)
            {
                anim.SetBool("Investigating", false);
                Patrolling = true;
            }

            GoToWayPoint();
        }

        if (scanner.Detected && sightLevel >= scanner.VisionTreshold)
        {
            if (!KillingSequenceStarted)
            {
                StartCoroutine(BeamPlayer());
            }
        }
    }


    IEnumerator BeamPlayer()
    {
        playerScript.Dead = true;
        KillingSequenceStarted = true;
        this.transform.rotation = playerScript.PlayerDirection(transform.position);
        agent.enabled = false;
        agent.Stop();
        playerScript.Disintegrate();
        anim.SetBool("Attacking", true);
        yield return new WaitForSeconds(ZapDuration);
        agent.enabled = true;
        anim.SetBool("Attacking", false);
        GetComponentInChildren<LightningStrike>().DestroyThis();
    }

    void NavigateNextWayPoint()
    {       
        //Make sure waypoints exist
        if (Waypoints.Count == 0)
            return;

        //If final waypoint reached, navigate back to the first, else move on to the next
        currentWayPoint = currentWayPoint >= Waypoints.Count ? 1 : currentWayPoint + 1;

        //Move to current waypoint
        GoToWayPoint();
    }

    void GoToWayPoint()
    {
        agent.SetDestination(Waypoints[currentWayPoint - 1]);
    }

    void OnDestinationReached()
    {
        NavigateNextWayPoint();
    }

    void OnCollisionEnter2D(Collision2D collider)
    {
        if (!hibernationScript.Hibernating)
        {
            if (collider.gameObject.tag == "Player" && !playerScript.Dead)
            {
                if (!KillingSequenceStarted)
                {
                    StartCoroutine(BeamPlayer());
                }
            }
        }
    }
}
