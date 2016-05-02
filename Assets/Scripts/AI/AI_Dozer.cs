using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class AI_Dozer : MonoBehaviour {

    //Waypoints to visit
    public List<Vector3> Waypoints;
    Transform player;
    CharControls playerScript;
    PolyNavAgent agent;
    Hibernation hibernationScript;
    Animator anim;

    //List to encapsulate certain variables across other enemies
    AI_SharedVariables AIVars;

    public float RotationSpeed;
    //Apprehension variables
    public bool Apprehending = false;
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


    void Awake()
    {
        //Avoid blood splatter animation from staring when spawning
        foreach (ParticleSystem ps in GetComponentsInChildren<ParticleSystem>())
        {
            ps.loop = false;
            ps.Stop();
        }
    }

	// Use this for initialization
	void Start () {
        //Instantiate pathfinding agent
        agent = GetComponent<PolyNavAgent>();
        Patrolling = true;

        //To reference players' variables
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerScript = player.GetComponent<CharControls>();
        hibernationScript = GetComponent<Hibernation>();
        AIVars = GetComponent<AI_SharedVariables>();
        anim = GetComponent<Animator>();
        NavigateNextWayPoint();
	}


	// Update is called once per frame
    void FixedUpdate()
    {
        if (IsHibernating)
            return;

        DecideActions();
    }

    void DecideActions()
    {
        if(Apprehending && !playerScript.Dead)
        {
            agent.SetDestination(player.position);
            agent.maxSpeed = ApprehensionSpeed - ApprehensionSpeed / 100 * AIVars.MovementSpeedPenalty;
        }
        else
        {
            Apprehending = false;
            GoToWayPoint();
            //Keep movement speed penalty into account
            agent.maxSpeed = NormalSpeed - NormalSpeed / 100 * AIVars.MovementSpeedPenalty;
        }

        anim.speed = 1 - AIVars.MovementSpeedPenalty / 100;
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
        if (Waypoints.Count == 1)
        {
            agent.Stop();
            hibernationScript.PowerOff(float.MaxValue);
            return;
        }

        NavigateNextWayPoint();
    }

    void SplatterBlood()
    {
        foreach (ParticleSystem ps in GetComponentsInChildren<ParticleSystem>())
        {
            ps.Play(true);
        }

        GetComponent<SpawnBlood>().MakeBlood();
    }

    void OnCollisionEnter2D(Collision2D collider)
    {
        if(collider.gameObject.tag == "Player" && !playerScript.Dead)
        {
            playerScript.Splatter();
            //Spawn blood on self as well
            SplatterBlood();
            player.position = transform.Find("PlayerStartPosition").transform.position;
            player.rotation = Direction;
            playerScript.Dead = true;
        }
    }

    //Returns the angle to this object from the players current position. Used to make player face the dozer when getting...dozed
    Quaternion Direction
    {
        get
        {
            return Quaternion.Euler(0, 0, Mathf.Atan2(transform.position.y - player.position.y, transform.position.x - player.position.x) * Mathf.Rad2Deg - 270);
        }
    }
}
